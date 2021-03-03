---
title: "Example Mod"
weight: 22
---

So now you've seen one basic example, but just to expand on that, here's another very simple {{< shortName >}} patch to _unlock all currently locked modifiers_:

```json
{
    "_meta": {
        "DisplayName": "Unlock disabled modifiers",
        "Author": "agc93",
        "description": "Unlocks disabled modifiers. Note that these modifiers don't actually work in-game."
    },
    "FilePatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/Modifiers/DB_Modifiers.uexp": [
            {
                "name": "Unlock modifiers",
                "patches": [
                    {
                        "description": "All modifiers available",
                        "template": "00 00 1E",
                        "substitution": "01 00 1E",
                        "type": "inPlace"
                    }
                ]
            }
        ]
    }
}
```

You'll see that the basic structure is essentially the same, just with different values for the files, as well as the details of the specific patch being applied.