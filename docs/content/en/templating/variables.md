---
title: "Template Variables"
linkTitle: "Using Variables"
weight: 33
---

This is a little more niche and mostly just used to save time while _building_ patches, but {{< shortName >}} also supports storing and retrieving variables in templates.

### Defining Variables

To define your variables, you can add a top-level key called `_vars` to your mod and add your variables with name and value to that:

```json
{
  "_meta": {
    // removed for brevity
  },
  "_vars": {
    "weaponList": "'0,saa,mlaa'"
  },
  "assetPatches": {
    //removed for brevity
  }
}
```

Then, your patches can retrieve that value using `vars.variableNameHere` in a template:

```json
{
  "_vars": {
    "weaponList": "'0,saa,mlaa'"
  },
  "filePatches": {
    "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
      {
          "name": "WeaponsChanges",
          "patches": [{
              "description": "Give FC16 the chosen weapons",
              "template": "datatable:['F-16C'].{'HardpointCompatibilityList*'}.[[1]]",
              "value": "StrProperty:{{ vars.weaponList }}",
              "type": "propertyValue"
          }]
      },
    ]
  }
}
```

> Note that variable names only have to be unique to the mod they're defined in. Variables are not shared across mods so you don't have to worry about affecting other mods if you include it in a preset or merged mod.