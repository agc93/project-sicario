---
title: "Example Mod"
weight: 25
---

### Modifiers

So now you've seen one basic example, but just to expand on that, here's another very simple {{< shortName >}} patch to _unlock all currently locked modifiers_:

```json
{
    "_meta": {
        "DisplayName": "Unlock disabled modifiers",
        "Author": "agc93",
        "description": "Unlocks disabled modifiers. Note that these modifiers don't actually work in-game."
    },
    "AssetPatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/Modifiers/DB_Modifiers.uexp": [
            {
                "name": "Unlock modifiers",
                "patches": [
                    {
                        "description": "All modifiers available",
                        "template": "datatable:{'Available*'}.<BoolProperty='false'>",
                        "value": "BoolProperty:true",
                        "type": "propertyValue"
                    }
                ]
            }
        ]
    },
    "FilePatches": { }
}
```

You'll see that the basic structure is essentially the same, just with different values for the files, as well as the details of the specific patch being applied.

### MSTM Weapon

For a _dramatically_ more complex example, you can see a few more of the moving parts in action together:

```json
{
    "_meta": {
        "displayName": "MSTM: Multiple-Launch Standard Missiles",
        "author": "agc93",
        "description": "Adds a new MSTM weapon: multiple-fire STDMs."
    },
    "filePatches": {},
    "assetPatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/Weapons/DWeaponDB.uexp": [{
                "name": "Add new weapon",
                "patches": [{
                    "description": "Clone MSSL",
                    "template": "datatable:[*]",
                    "value": "'MSSL'>'MSTM'",
                    "type": "duplicateEntry"
                }]
            },
            {
                "name": "ID change",
                "patches": [{
                        "description": "Change mssl ID to mstm",
                        "template": "datatable:['MSTM'].[0].{'ID*'}.<StrProperty>",
                        "value": "StrProperty:'mstm'",
                        "type": "propertyValue"
                    },
                    {
                        "description": "Change STDM name to MSTM",
                        "template": "datatable:['MSTM'].[0].{'WeaponUIName*'}.<TextProperty>",
                        "value": "TextProperty:'MSTM'",
                        "type": "propertyValue"
                    },
                    {
                      "description": "Increases the MSTM max projectiles to 6",
                      "template": "datatable:['MSTM'].{'MaxProjectile*'}.<IntProperty=2>",
                      "value": "IntProperty:6",
                      "type": "propertyValue"
                    }]
            }
        ]
    }
}
```

> Of particular note, you can see that the patch _adding_ the MSTM is in its own patch set since they get processed set-by-set.
> 
> Otherwise, the later patches wouldn't match anything (since it wouldn't exist yet)