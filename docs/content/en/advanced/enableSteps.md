---
title: "Making patches conditional"
linkTitle: "Conditional Patches"
weight: 52
---

In some circumstances you might only want to run a certain step under certain conditions, or to provide options to skip certain patches in a mod.

> In general, you should _strongly_ preference using separate mod files for different patches. Conditional steps are only intended for **very** specific niche cases. Site owners _may remove mods_ if they are abusing inputs/conditionals.

Conditional patches allow you to only enable an entire patch set if a certain condition is met. The condition can be based on anything: constants, variables, inputs or any combination of those.

## Definining Conditions

Conditional patches are (a little counter-intuitively) defined completely separate from the patches they are controlling.

> The reasons for this are honestly sort of complicated but short version, this is how it works and will not be merged in future.

For the sakes of this document, let's look at this complete mod file:

```json5
{
    "_meta": {
        "displayName": "Demo-Style FE-18",
        "author": "agc93",
        "description": "Adds STDMs to all slots for the F/E-18 and enables multilock"
    },
    "_inputs": [
        {
            "id": "horneyMultilock",
            "type": "boolean",
            "message": "Include STDM multilock",
            "default": "true"
        }
    ],
    "_sicario": {
        "enableSteps": {
            "MaxMultiLock": "{{ inputs.horneyMultilock }}"
        }
    },
    "filePatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
            {
                "name": "Loadout",
                "patches": [
                    {
                        "description": "Add weapons to first slot",
                        "template": "{{ \"0,saa,mlaa\" | row }}",
                        "substitution": "{{ \"0,saa,mlaa,stdm\" | row }}",
                        "type": "inPlace",
                        "window": {
                            "after": "text:F/E-18",
                            "maxMatches": 1
                        }
                    }
                ]
            }
        ],
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/Weapons/DWeaponDB.uexp": [
            {
                "name": "MaxMultiLock",
                "patches": [
                    {
                        "description": "Ups the STDM max projectiles to 10",
                        "template": "{{ 2 | int }}{{ DWeaponDB.MaxProjectile }}",
                        "substitution": "{{ 10 | int }}{{ DWeaponDB.MaxProjectile }}",
                        "type": "inPlace",
                        "window": {
                            "after": "text:STDM",
                            "before": "text:stdm2"
                        }
                    }
                ]
            }
        ]
    }
}
```

You'll see the familiar combinations of `_meta`, `_inputs` and `filePatches`, but you'll see a new object in `_sicario.enableSteps`. That object is a dictionary that matches the `name` of a patch set to the condition controlling whether it should run.

## Evaluating Conditionals

The shortest possible explanation for this logic is that the patch set with the specified name will only be included in the final mod **if and only if** the condition specified on the right evaluates to a boolean `true`. In practice, that means the result of any values or templates in the condition should return the string "true" or "false". Returning true means the patch set will be included, returning false means the patch set will be ignored. 

> If a patch set doesn't appear in the `enableSteps` object at all, it will still be included by default.

As mentioned, you can use all the usual features in the patch sets conditional including templates, filters, inputs, and variables. You can see that in the example above where the value of an input is directly controlling the inclusion of a patch set. While that's probably the simplest case, there's also plenty of other ways to control whether a set is enabled using all the features we've covered previously.

## Usage

It is worth reiterating again that conditional patches should be a a **very niche** and/or last-resort feature. In general, you should use multiple mod files to create variants of a mod. For example, OP Weapons is built as 6 separate files so that users can most easily opt-in or opt-out of individual changes. This also makes Sicario's job easier since changes are applied mod-by-mod.

Conditional patches should _only_ be used in very specific scenarios where a small change that cannot be defined separately may have a large impact on the final mod.