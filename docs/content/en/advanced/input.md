---
title: "Working with user input"
linkTitle: "User Input"
weight: 51
---

For some more complex cases, you might need to get user input to control how your patches are applied or change the values you're using in your patches. For this reason, {{< shortName >}} supports  some basic input types that will be shown to users when they enable your mod in a build process.

## Defining Inputs

Inputs need to be separately defined in a special `_inputs` key in your mod file:

```json
{
    "_meta": {
        //trimmed for brevity
    },
    "_inputs": [
        {
            "id": "uniqueIdHere",
            "type": "number",
            "message": "This will be shown to users",
            "default": "5" // REQUIRED
        }
    ],
    "FilePatches": {
        //trimmed for brevity
    }
}
```

When the user enables a mod that defines inputs, {{< shortName >}} will show a collection of input fields (grouped by mod) for the user to enter their own values. The `message` field is what will be shown to users, and you **must** define a `default` value or the input may not be shown or parsed correctly.

## Using Inputs

Now that's easy but where do those values go? They are available in any templated fields!

The `id` of your input is the important part as that will be how you access your input's final value in the patch definition. For example, if we define this input:

```json
"_inputs": [
    {
        "id": "customRollRate",
        "type": "number",
        "message": "Enter a turn rate",
        "default": "200" //note that this is always a string!
    }
]
```

We can then use the value from this input in a patch with a template, the special `inputs` object and the `id` of our input:

```json
"ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
        {
            "name": "Stat Changes",
            "patches": [
                {
                    "description": "Set RollInterpSpeed",
                    "template": "00 00 48 43 3C 02",
                    "substitution": "{{ inputs.customRollRate | float}} 3C 02",
                    "type": "inPlace"
                }
            ]
        }
    ]
}
```

> You will almost always want to chain inputs with [filters](/templating/filters) (like the above example) so that users don't have to enter raw hex.