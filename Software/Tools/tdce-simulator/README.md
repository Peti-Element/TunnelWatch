# tdce-simulator

Stands in for the TDC-E during development. It publishes MQTT messages in the CAN App format to a broker, with the same CAN IDs, signal names and units as the device, so the backend, Android and web apps can be developed and tested without the physical device.

## Requirements

- Python 3.9 or newer
- A running MQTT broker, e.g. the one from `Software/Backend`:

  ```bat
  cd Software\Backend
  docker compose up -d
  ```

## Install

In `Software/Tools/tdce-simulator`:

```bat
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
```

## Run

```bat
python tdce_sim.py
```

It publishes one payload per second to `tdce-sim/can0/j1939-default` until you press Ctrl+C, and prints a status line for every payload. If the broker is not running, it stops with `ConnectionRefusedError`.

To see the messages on the broker (cmd):

```bat
docker exec backend-mosquitto-1 mosquitto_sub -t "tdce-sim/#" -v
```

With the backend running, the simulated machine appears as `tdce-sim` at http://localhost:5161/api/gateways.

## Options

| Option | Default | Description |
|---|---|---|
| `--broker` | `127.0.0.1` | MQTT broker host |
| `--port` | `1883` | MQTT broker port |
| `--prefix` | `tdce-sim` | First part of the topic, used by the backend as the gateway ID. No `/`, `+` or `#`. |
| `--scenario` | `drill-rig` | `drill-rig` stays in place, `truck` also drives while working |
| `--fault` | `none` | `overheat`, `low-oil` or `low-fuel`, for alarm testing |
| `--fault-after` | `30` | Seconds after start when the fault begins |
| `--interval` | `1.0` | Seconds between payloads |
| `--duration` | `0` | Seconds to run, `0` runs until Ctrl+C |

Examples:

```bat
python tdce_sim.py --scenario truck
python tdce_sim.py --fault overheat --fault-after 30
python tdce_sim.py --broker 192.168.0.10 --prefix tdce-sim-2
```

## Behaviour

- The machine alternates between 60 s of idle and 60 s of work, starting with idle. Engine speed (about 770 rpm idle, 1800 rpm working), load, torque and exhaust temperature move gradually towards the values of the current phase.
- Engine hours and total fuel used keep increasing; idle hours and idle fuel increase while idle. Fuel use depends on load (about 6 L/h idle, 27 L/h working), and the fuel level drops with it.
- Coolant and oil temperatures move slowly towards operating temperature (88 °C and 95 °C).
- `truck`: while working, the truck accelerates towards 40 km/h, the gear follows the speed, the parking brake is off and the distance grows. While idle it slows down and stops, and the parking brake is on again.
- Faults start after `--fault-after` seconds:

| Fault | Signal | Behaviour |
|---|---|---|
| `overheat` | `engine_coolant_temperature` | rises 1 °C per second, up to 110 °C |
| `low-oil` | `engine_oil_pressure` | drops 15 kPa per second, down to 60 kPa |
| `low-fuel` | `fuel_level` | drops 2 % per second, down to 5 % |

## Differences from the device

- One payload per second with each of the 19 CAN messages once. The device sends about ten payloads per second with 14–48 messages each, often duplicated.
- Values come from a simple idle/work model, not from a real engine. Signals the model does not change keep fixed start values.
- Only port `can0` and profile `j1939-default`. There is no CAN bus and no CAN App REST or WebSocket API, only MQTT.
- Only the `drill-rig` and `truck` scenarios; CAN App's built-in simulator also has `tms`.
- The default prefix is `tdce-sim`, so simulated data does not mix with the real device (`tdce-01`).

CAN IDs, signal names (including CAN App's spelling, e.g. `total_idel_fuel_used`) and units are the same as in `Software/Backend/samples/device-truck.txt`. The simulator also sends `engine_reference_torque` (CAN ID 419357440), which the live device sends but the 20-message recording missed.

## Comparing with the device

`catalog.py` lists the CAN IDs, signals and units in MQTT messages, either from a recording file or from stdin (`-`):

```bat
python catalog.py ..\..\Backend\samples\device-truck.txt
docker exec backend-mosquitto-1 mosquitto_sub -t "tdce-sim/#" -v -C 3 | python catalog.py -
docker exec backend-mosquitto-1 mosquitto_sub -t "tdce-01/#" -v -C 20 | python catalog.py -
```

The first prints 18 CAN IDs and 28 signals, the simulator 19 and 29 (the extra one is `engine_reference_torque`). The last one reads live from the device when it publishes to the same broker.

## Files

| File | Content |
|---|---|
| `tdce_sim.py` | The simulator |
| `catalog.py` | Lists CAN IDs, signals and units in MQTT messages |
| `requirements.txt` | Python dependencies (`paho-mqtt`) |
