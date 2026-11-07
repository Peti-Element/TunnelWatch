# Device MQTT samples

Messages recorded from the TDC-E on 7 Nov 2026, for parser tests and for development without the device. CAN App's built-in simulator runs on the device, so the messages go through the real path: CAN bus → CAN App → MQTT broker.

| File | Scenario |
|---|---|
| `device-drill-rig.txt` | drill-rig, 4 signals |
| `device-truck.txt` | truck, 28 signals |

Each line is `<topic> <JSON payload>`, 20 payloads per file, topic `tdce-01/can0/j1939-default`.

## Recording

In cmd (Windows PowerShell writes UTF-16), in `Software/Backend`:

```bat
docker exec backend-mosquitto-1 mosquitto_sub -t "tdce-01/#" -v -C 20 > samples\device-truck.txt
```

## Format

```json
{"version":1,"port":"can0","profile":"j1939-default","messages":[{"id":217056256,"timestamp":1794024892428,"signals":{"engine_speed":{"value":1801.75,"unit":"rpm"}}}]}
```

- `unit` is missing for status signals (`currentgear`, `parking_brake_switch`).
- `value` can be an integer or a decimal for the same signal, so read it as a double.
- The lowest byte of `id` is the sender's address. Identify groups by PGN, `(id >> 8) & 0x3FFFF`, or by signal name.
- The number of messages per payload varies (14–48).

## Replay

To publish the samples to the local broker in a loop:

```bat
docker cp samples\device-truck.txt backend-mosquitto-1:/tmp/s.txt
docker exec backend-mosquitto-1 sh -c "while true; do while read -r t p; do mosquitto_pub -t \"$t\" -m \"$p\"; sleep 1; done < /tmp/s.txt; done"
```