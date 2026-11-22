import sys
import json, time, argparse, random
import paho.mqtt.client as mqtt
import paho.mqtt.enums as mqtt_enums

PORT = "can0"
PROFILE = "j1939-default"

CATALOG = {
 217056256: {'drivers_demand_engine_percent_torque': '%',
             'engine_speed': 'rpm'},
 419361041: {'parking_brake_switch': None, 'wheelbased_vehicle_speed': 'km/h'},
 419361280: {'instantaneous_fuel_economy': 'km/L'},
 217056000: {'accelerator_pedal_position': '%',
             'engine_percent_load_at_current_speed': '%'},
 217056515: {'currentgear': None},
 418381839: {'actual_retarder_percent_torque': '%'},
 419360512: {'engine_coolant_level': '%',
             'engine_coolant_pressure': 'kPa',
             'engine_oil_level': '%',
             'engine_oil_pressure': 'kPa'},
 419362304: {'exhaust_gas_temperature': 'degC'},
 419275262: {'fuel_pressure': 'kPa'},
 419360256: {'engine_coolant_temperature': 'degC',
             'engine_oil_temperature': 'degC',
             'fuel_temperature': 'degC'},
 419362071: {'ambiant_air_temperature': 'degC'},
 419364070: {'fuel_level': '%'},
 419321344: {'ad_blue_level_catalyst_tank_level': '%'},
 419265280: {'diesel_particulate_filter_load_soot_level': '%'},
 419357952: {'engine_total_hours_of_operation': 'h'},
 419348736: {'total_vehicle_distance': 'm'},
 419358976: {'engine_total_fuel_used': 'L', 'fuel_consumption_total': 'L'},
 419355648: {'total_idel_fuel_used': 'L', 'total_idle_hours': 'h'},
 419357440: {"engine_reference_torque": "Nm"}, # nedostajao je u device-truck.txt, dodan je u katalog da bi simulator mogao slati reference torque
}

# Stroj naizmjence 60 s stoji u praznom hodu, pa 60 s radi.
PHASE_SECONDS = 60
IDLE = {"engine_speed": 770, "engine_percent_load_at_current_speed": 10,
        "drivers_demand_engine_percent_torque": 9, "exhaust_gas_temperature": 150}
WORK = {"engine_speed": 1800, "engine_percent_load_at_current_speed": 63,
        "drivers_demand_engine_percent_torque": 59, "exhaust_gas_temperature": 380}


def approach(current, target, factor):
    # Pomakni current prema target za neki factor
    return current + (target - current) * factor

def update(state, elapsed, dt, args):
    """Izračuna novo stanje. elapsed = s od starta, dt = s od prošlog takta. Vraća True ako stroj radi."""
    working = int(elapsed // PHASE_SECONDS) % 2 == 1
    for name, goal in (WORK if working else IDLE).items():
        factor = 0.05 if name == "exhaust_gas_temperature" else 0.15
        state[name] = approach(state[name], goal, factor)
    state["engine_speed"] += random.uniform(-2, 2)  # malo šuma da nije ravna crta

    # Motor se polako zagrijava prema radnoj temperaturi.
    overheating = args.fault == "overheat" and elapsed >= args.fault_after
    if not overheating:
        state["engine_coolant_temperature"] = approach(state["engine_coolant_temperature"], 88, 0.01)
    state["engine_oil_temperature"] = approach(state["engine_oil_temperature"], 95, 0.01)

    # Potrošnja ovisi o opterećenju: ~6 L/h u praznom hodu, ~27 L/h u radu.
    rate = 2 + 0.4 * state["engine_percent_load_at_current_speed"]   # L/h
    litres = rate * dt / 3600
    state["engine_total_hours_of_operation"] += dt / 3600
    state["engine_total_fuel_used"] += litres
    state["fuel_level"] -= litres / 400 * 100  # spremnik od 400 L
    if not working:
        state["total_idle_hours"] += dt / 3600
        state["total_idel_fuel_used"] += litres

    # Vožnja: samo truck u fazi rada, drill-rig stoji na mjestu.
    driving = args.scenario == "truck" and working
    target_speed = 40 if driving else 0
    state["wheelbased_vehicle_speed"] = approach(state["wheelbased_vehicle_speed"], target_speed, 0.1)
    if state["wheelbased_vehicle_speed"] < 0.5:
        state["wheelbased_vehicle_speed"] = 0   # približavanje nikad ne dođe do točne nule
    speed = state["wheelbased_vehicle_speed"]

    target_pedal = 40 if driving else 0
    state["accelerator_pedal_position"] = approach(state["accelerator_pedal_position"], target_pedal, 0.3)

    if speed > 0:
        state["currentgear"] = min(6, 1 + int(speed // 8))
        state["parking_brake_switch"] = 0
        state["instantaneous_fuel_economy"] = speed / rate # km/L
    else:
        state["currentgear"] = 0
        state["parking_brake_switch"] = 1
        state["instantaneous_fuel_economy"] = 0
    state["total_vehicle_distance"] += speed / 3.6 * dt # km/h u m/s, puta sekunde

    apply_fault(state, elapsed, dt, args)
    return working

def apply_fault(state, elapsed, dt, args):
    """Kvar za testiranje alarma. Ide zadnji, pa nadjača normalno ponašanje."""
    if args.fault == "none" or elapsed < args.fault_after:
        return

    if args.fault == "overheat":
        new_temp = state["engine_coolant_temperature"] + 1 * dt     # +1 °C u sekundi
        state["engine_coolant_temperature"] = min(new_temp, 110)
    elif args.fault == "low-oil":
        new_pressure = state["engine_oil_pressure"] - 15 * dt       # pada ispod 100 kPa
        state["engine_oil_pressure"] = max(new_pressure, 60)
    elif args.fault == "low-fuel":
        new_level = state["fuel_level"] - 2 * dt                    # pada ispod 10 %
        state["fuel_level"] = max(new_level, 5)

# Gradi payload u obliku JSON-a koji sadrži sve CAN ID-jeve i signale iz globalnog kataloga CATALOG, zajedno s njihovim vrijednostima i jedinicama.
def build_payload(values, now_ms):
    messages = []
    for can_id, signals in CATALOG.items():
        body = {}
        for name, unit in signals.items():
            value = values[name]

            # Ako je unit None, ne dodajemo ga u payload, inače dodajemo i value i unit.
            body[name] = {"value": value} if unit is None else {"value": value, "unit": unit}

        messages.append({"id": can_id, "timestamp": now_ms, "signals": body})
    return {"version": 1, "port": "can0", "profile": "j1939-default", "messages": messages}

def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Simulira TDC-E: šalje CAN App poruke na MQTT broker.")
    parser.add_argument("--broker", default="127.0.0.1", help="adresa MQTT brokera (default 127.0.0.1)")
    parser.add_argument("--port", type=int, default=1883, help="port MQTT brokera (default 1883)")
    parser.add_argument("--prefix", default="tdce-sim", help="prvi dio topica, tj. ID uređaja (default tdce-sim)")
    parser.add_argument("--scenario", choices=["drill-rig", "truck"], default="drill-rig",
                        help="drill-rig stoji na mjestu, truck u fazi rada vozi (default drill-rig)")
    parser.add_argument("--fault", choices=["none", "overheat", "low-oil", "low-fuel"], default="none",
                        help="kvar za testiranje alarma (default none)")
    parser.add_argument("--fault-after", type=float, default=30,
                        help="nakon koliko sekundi kvar počinje (default 30)")
    parser.add_argument("--interval", type=float, default=1.0,
                        help="sekunde između dva payloada (default 1.0)")
    parser.add_argument("--duration", type=float, default=0,
                        help="koliko sekundi slati, 0 = dok se ne prekine s Ctrl+C (default 0)")
    args = parser.parse_args()

    # Backend prihvaća samo topic od točno tri dijela
    if not args.prefix.strip() or any(ch in args.prefix for ch in "/+#"):
        parser.error("--prefix ne smije biti prazan ni sadržavati '/', '+' ili '#'")
    if args.interval <= 0:
        parser.error("--interval mora biti veći od 0")
    if args.fault_after < 0:
        parser.error("--fault-after ne smije biti negativan")
    return args

def main():
    args = parse_args()
    topic = f"{args.prefix}/{PORT}/{PROFILE}" # npr. "tdce/can0/j1939-default"

    # Početne vrijednosti signala. Ove vrijednosti se mijenjaju u update() funkciji.
    values = {
        'drivers_demand_engine_percent_torque': 50,
        'engine_speed': 2000,
        'parking_brake_switch': 1,
        'wheelbased_vehicle_speed': 0,
        'instantaneous_fuel_economy': 0,
        'accelerator_pedal_position': 0,
        'engine_percent_load_at_current_speed': 70,
        'currentgear': 0,
        'actual_retarder_percent_torque': 20,
        'engine_coolant_level': 75,
        'engine_coolant_pressure': 120,
        'engine_oil_level': 80,
        'engine_oil_pressure': 300,
        'exhaust_gas_temperature': 400,
        'fuel_pressure': 250,
        'engine_coolant_temperature': 90,
        'engine_oil_temperature': 85,
        'fuel_temperature': 60,
        'ambiant_air_temperature': 25,
        'fuel_level': 60,
        'ad_blue_level_catalyst_tank_level': 50,
        'diesel_particulate_filter_load_soot_level': 10,
        'engine_total_hours_of_operation': 5000,
        'total_vehicle_distance': 1500000,
        'engine_total_fuel_used': 20000,
        'fuel_consumption_total': 20000,
        'total_idel_fuel_used': 1000,
        'total_idle_hours': 1000,
        'engine_reference_torque': 400
    }
    client = mqtt.Client(mqtt_enums.CallbackAPIVersion.VERSION2)

    try: # da pomognem kolegama koji nisu pokrenuli broker, a pokušavaju se spojiti na njega >:3
        client.connect(args.broker, args.port)
    except OSError as error:
        sys.exit(f"Ne mogu se spojiti na {args.broker}:{args.port} ({error}). "
                "Jesi pokrenuo broker? U Software/Backend: docker compose up -d")
        
    client.loop_start()
    print(f"Saljem na {topic} svakih {args.interval} s. Ctrl+C za kraj.")

    state = dict(values) # početno stanje, koje se mijenja u update() funkciji
    start = time.monotonic()    # mjeri proteklo vrijeme tkd ne ovisi o satu računala
    next_tick = last = start 
    try:
        while True:
            now = time.monotonic() 
            elapsed = now - start
            dt = now - last
            last = now
            if args.duration and elapsed >= args.duration:
                break

            working = update(state, elapsed, dt, args)
            sent = {name: round(value, 2) for name, value in state.items()}
            payload = build_payload(sent, int(time.time() * 1000))
            client.publish(topic, json.dumps(payload, separators=(",", ":")))
            print(f"{elapsed:5.0f}s {'rad ' if working else 'idle'} "
                f"rpm={sent['engine_speed']:.0f} load={sent['engine_percent_load_at_current_speed']:.0f}% "
                f"hours={sent['engine_total_hours_of_operation']} fuel={sent['engine_total_fuel_used']}"
                f"speed={sent['wheelbased_vehicle_speed']:.0f} gear={sent['currentgear']} brake={sent['parking_brake_switch']} "
                f"coolant={sent['engine_coolant_temperature']:.0f} oil={sent['engine_oil_pressure']:.0f} fuel%={sent['fuel_level']}"
            ) 
            # čekaj do sljedećeg takta; sleep(interval) bi polako kasnio zbog vremena slanja
            next_tick += args.interval
            time.sleep(max(0, next_tick - time.monotonic()))
    except KeyboardInterrupt:
        print("Zaustavljeno.")
    finally:
        client.loop_stop()
        client.disconnect()

if __name__ == "__main__":
    main()