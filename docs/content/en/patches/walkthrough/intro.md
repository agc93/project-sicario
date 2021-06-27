---
title: "File Structure"
weight: 21
anchor: "howto-basics"
---

### The AoA unlocker mod file

```json
{
  "_meta": {
    "DisplayName": "AoA for All"
  },
  "FilePatches": {},
  "AssetPatches": {
      "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
          {
              "name": "AoA Unlock",
              "patches": [
                  {
                      "description": "Set CanUseAoA to true",
                      "template": "datatable:{'BaseStats*'}.{'CanUseAoA*'}",
                      "value": "BoolProperty:true",
                      "type": "propertyValue"
                  }
              ]
          }
      ]
  }
}
```

> Note that (for now), the `FilePatches: {}` object is still ***required***, even if it's not being used.

Now let's step through each part of this file.

### Patch Metadata (<code>_meta</code>)

The first thing you'll usually find in a {{< shortName >}} patch file is the `_meta` field, which is an object with a few optional properties:

- `DisplayName`: A user-friendly name to show for your mod
- `Author`: Take a guess what this one's for hotshot
- `Description`: also self-explanatory

All three of these values will be shown to users so please don't just put "do the thing" or fill them in with shitposting!

There is some more metadata you can optionally provide, but that's covered later on.

### The Asset Patches (`AssetPatches`)

The `AssetPatches` object is the main "substance" of a {{< shortName >}} patch. It is a dictionary grouping the files to be edited with sets of patches to be applied to that file.

> Note that to support auto-packing you need to include the game file as the full target path for the file you're editing.

In the example above, we have just one key in the object: `ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp`. This just means that `DB_Aircraft.uexp` will be the only file that this patch will be applied to and it will be packed in that specific path. Each file then has an array of "patch sets".

#### Patch Sets

Patch Sets are a purely logical organizational idea: they're just a way of organizing patches together. For example, if your mod is changing multiple stats for multiple planes, you might include each plane as a separate patch set. A patch set is just an object with `name` and an array of `patches`:

```json
{
  "name": "AoA Unlock",
  "patches": []
}
```

On it's own a patch set doesn't do anything, that's up to the actual _patches_ in the set.

#### Patches

Now we hit the real meat of a {{< shortName >}} patch: the actual hex edit to make. Here's the example for enabling AoA for all aircraft:

```json
{
  "description": "Set CanUseAoA",
  "template": "datatable:{'BaseStats*'}.{'CanUseAoA*'}",
  "value": "BoolProperty:true",
  "type": "propertyValue"
}
```

In plain English, this object just tells {{< shortName >}} "replace the value of any `CanUseAoA` properties with `true`".

##### Template and Value

These are the actual values {{< shortName >}} will be (respectively) looking for and inserting into the binary file. When it runs, {{< appName >}} will load the file into memory, parse the file (more on that below), look for _any_ values that match the **template** value and then apply the **value**. How exactly the value is applied varies based on the patch _type_.

##### Types

The two main types of patches currently in use are:

|Type|Notes|
|:--:|:----|
|`propertyValue`|This is by far the most common/useful and for good reason: it's the most useful and versatile type. It sets the value of matching properties to the given value.|
|`arrayPropertyValue`|Sibling type to `propertyValue` that creates/inserts an array value to the given properties using a simple array syntax|

Both of these types use a common convention for the `value` field: `DataType:value`. This ensures that the properties are set with the correct data types the game uses. Note that other patch types might not use the same convention.