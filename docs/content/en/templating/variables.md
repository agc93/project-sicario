---
title: "Template Variables"
linkTitle: "Variables"
weight: 33
---

This is a little more niche and mostly just used to save time while _building_ patches, but Sicario also supports storing and retrieving variables in templates.

### Defining Variables

To define your variables, you can add a top-level key called `_vars` to your file and add your variables with name and value to that:

```json
{
  "_meta": {
    // removed for brevity
  },
  "_vars": {
    "pilotCount": "00 00 00 1С 01"
  },
  "filePatches": {
    //removed for brevity
  }
}
```

Then, your patches can retrieve that value using `vars.variableNameHere` in a template:

```json
{
  "_vars": {
    "pilotCount": "00 00 00 1С 01"
  },
  "filePatches": {
    "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
      {
        "name": "Prez Unlock",
        "patches": [
          {
            "description": "Set Pilot count to 2",
            "template": "00 {{ vars.pilotCount }}",
            "substitution": "02 {{ vars.pilotCount }}",
            "type": "inPlace"
          }
        ]
      }
    ]
  }
}
```