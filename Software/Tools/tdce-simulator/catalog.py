"""Program koji iz snimke CAN prometa izdvoji sve CAN ID-jeve i signale, te ih ispiše u obliku kataloga.
Služi za usporedbu simulatora s uređajem: ako je ispis za snimku simulatora isti kao za device-truck.txt, 
simulator šalje iste CAN ID-jeve, signale i jedinice."""

import json
import sys

# Podržava čitanje iz datoteke ili iz standardnog ulaza (stdin). Ako se kao argument navede "-", program čita snimku iz stdin-a.
def read_catalog(lines):
    catalog = {}

    # Svaki redak snimke je "<topic> <JSON payload>".
    for line in lines:
        line = line.strip()
        if not line:
            continue
        payload = line.split(" ", 1)[1]
        
        # example payload: {"messages":[{"id":123,"signals":{"signal1":{"unit":"V"},"signal2":{"unit":"A"}}}]}
        for msg in json.loads(payload)["messages"]:
            signals = catalog.setdefault(msg["id"], {})
            for name, signal in msg["signals"].items():
                signals.setdefault(name, signal.get("unit"))
                
    return catalog

def main():
    if len(sys.argv) != 2:
        sys.exit("Upotreba: python catalog.py <snimka.txt | ->")

    path = sys.argv[1]
    if path == "-":
        catalog = read_catalog(sys.stdin)
    else:
        with open(path, encoding="utf-8") as file:
            catalog = read_catalog(file)

    for can_id, signals in catalog.items():
        print(can_id)
        for name, unit in signals.items():
            print(f"    {name} {unit or '-'}")

    print(f"{len(catalog)} CAN ID-jeva, {sum(len(s) for s in catalog.values())} signala")

if __name__ == "__main__":
    main()