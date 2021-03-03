---
title: "Windowing matches"
weight: 23
anchor: "howto-windows"
---

Now while that will work fine for simple things, sometimes you might not want to change **every** appearance of a key with its substitution. This is where the special `window` object on the patch becomes important.

Here's what it looks like in action:

```json
{
    "_meta": {
        // removed for brevity
    },
    "FilePatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
            {
                "name": "Prez Unlock",
                "patches": [
                    {
                        "description": "Set Pilot count to 2",
                        "template": "00 00 00 00 1C 01",
                        "substitution": "02 00 00 00 1C 01",
                        "type": "inPlace",
                        "window": {
                            "after": "text:PW-Mk.I",
                            "before": "text:MG-21"
                        }
                    }
                ]
            }
        ]
    }
}
```

In short, the `window` object is a method of narrowing down what values should be replaced in a file. In the example above, we want to replace `00 00 00 00 1C 01` with `02 00 00 00 1C 01`, but **only** for the PW Mk.I. To do that, we add a `window` object and specify that we only want to match occurrences of `00 00 00 00 1C 01` if they come _after_ an appearance of the text "PW-Mk.I" **and** _before_ an appearance of the text "MG-21". This will narrow the search down to only the PW Mk.I's part of the blueprint, ensuring we only change the PilotCount (i.e. `00 00 00 1C 01`) for that one plane.

> {{< shortName >}} will automatically substitute a string value for its binary/hex representation if it starts with the special `text:` prefix. So you could add `text:F/E-4` and {{< shortName >}} will automatically change that to `46 2F 45 2D 34` for you. 

Note that you can also omit the `before` key and only use an `after` to narrow down the search, but the reverse is _not_ true! If you use a `before`, you must have an `after` to make a window.