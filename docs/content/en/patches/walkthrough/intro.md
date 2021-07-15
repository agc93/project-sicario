---
title: "File Structure"
weight: 21
anchor: "howto-basics"
---

Let's dive into the deep end and use a reasonably simple example of a patch file to illustrate how an asset patch file works. There's a complete example of a reasonably simple mod included below: the AoA Unlocker. This will probably seem like a lot but over the next few pages we'll walk slowly through all the individual parts so you can get an idea of how the patches work and how you can build your own.

## The AoA unlocker mod file

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

Now let's step through each part of this file, so you can see what each part does.

## Patch Metadata (<code>_meta</code>)

The first thing you'll usually find in a {{< shortName >}} patch file is the `_meta` field, which is an object with a few optional properties:

- `DisplayName`: A user-friendly name to show for your mod
- `Author`: Take a guess what this one's for hotshot
- `Description`: also self-explanatory

All three of these values will be shown to users so please don't just put "do the thing" or fill them in with shitposting!

There is some more metadata you can optionally provide, but that's covered later on.

## The Asset Patches (`AssetPatches`)

The `AssetPatches` object is the main "substance" of a {{< shortName >}} patch. It is a dictionary grouping the files to be edited with sets of patches to be applied to that file.

> Note that to support auto-packing you need to include the game file as the full target path for the file you're editing.

In the example above, we have just one key in the object: `ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp`. This just means that `DB_Aircraft.uexp` will be the only file that this patch will be applied to and it will be packed in that specific path. Each file then has an array of "patch sets".

### Patch Sets

Patch Sets are mostly a logical organizational idea: they're a way of organizing patches together. For example, if your mod is changing multiple stats for multiple planes, you might include each plane as a separate patch set. A patch set is just an object with `name` and an array of `patches`:

```json
{
  "name": "AoA Unlock",
  "patches": []
}
```

On it's own a patch set doesn't do much, that's up to the actual _patches_ in the set. 

#### Patch Set Grouping

There is one important thing to note with patch sets: all the patches in a set are _matched_ together. That means if one patch requires a previous change, or matches based on a value changed by a previous patch, they need to be in separate patch sets. For example, if you change the name of something and then want a later patch to find the property based on that new name, it needs to be in its own set. 

This will make a bit more sense later on, but just keep it in mind if you're making changes that rely on each other.

> The reason for this is both complex and simple: it was possible for a change from an earlier patch to then match a later patches template which was extremely confusing in practice.

### Patches

Now we hit the real meat of a {{< shortName >}} patch: the actual edit to make. Here's the example for enabling AoA for all aircraft:

```json
{
  "description": "Set CanUseAoA",
  "template": "datatable:{'BaseStats*'}.{'CanUseAoA*'}",
  "value": "BoolProperty:true",
  "type": "propertyValue"
}
```

In plain English, this object just tells {{< shortName >}} "replace the value of any `CanUseAoA` properties with `true`", but that's probably not immediately obvious. Don't worry, it's not as complicated as it looks!

#### Template and Value

These are the actual values {{< shortName >}} will be (respectively) looking for and inserting into the file. When it runs, {{< appName >}} will load the file into memory, parse the file (more on that later), look for _any_ values that match the **template** value and then apply the **value**. How exactly the value is applied varies based on the patch _type_.

In short, the Template is how {{< shortName >}} knows which properties to change (and not to change) and is used to isolate out exactly what you want to change. Templates are covered in more detail in the [next](../template) section.

The Value of the patch, on the other hand, is how {{< shortName >}} knows _how_ you want to change it and is used to apply any number of potential changes based on the patch _type_. The patch value (and patch types) are also covered in more detail in a [later](../patches) section.