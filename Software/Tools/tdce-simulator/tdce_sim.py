import json, time


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

def main():
    # Primjer korištenja funkcije build_payload
    values = {
        'drivers_demand_engine_percent_torque': 50,
        'engine_speed': 2000,
        'parking_brake_switch': 1,
        'wheelbased_vehicle_speed': 80,
        'instantaneous_fuel_economy': 15.5,
        'accelerator_pedal_position': 30,
        'engine_percent_load_at_current_speed': 70,
        'currentgear': 4,
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
    now_ms = int(time.time() * 1000)
    payload = build_payload(values, now_ms)
    print(payload)

if __name__ == "__main__":
    main()