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
  "FilePatches": {
    "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [{
      "name": "AoA Unlock",
      "patches": [{
        "description": "Set CanUseAoA",
        "template": "00 48 02",
        "substitution": "01"
      }]
    }]
  }
}
```

Now let's step through each part of this file.

### Patch Metadata (<code>_meta</code>)

The first thing you'll usually find in a {{< shortName >}} patch file is the `_meta` field, which is an object with a few optional properties:

- `DisplayName`: A user-friendly name to show for your mod
- `Author`: Take a guess what this one's for hotshot
- `Description`: also self-explanatory

All three of these values will be shown to users so please don't just put "do the thing" or fill them in with shitposting!

There is some more metadata you can optionally provide, but that's covered later on.

### The File Patches (`FilePatches`)

The `FilePatches` object is the main "substance" of a {{< shortName >}} patch. It is a dictionary grouping the files to be edited with sets of patches to be applied to that file.

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
  "template": "00 48 02",
  "substitution": "01",
  "type": "before"
}
```

In plain English, this object just tells {{< shortName >}} "replace the byte immediately **before** `00 48 02` with a `01`".

##### Template and Substitution

These are the actual values {{< shortName >}} will be (respectively) looking for and inserting into the binary file. When it runs, {{< appName >}} will load the file into memory, look for _any_ appearance of the **template** value and then apply the **substitution**. How exactly the substitution is applied varies based on the patch _type_.

##### Types

The two main types of patches currently in use are:

- `before`: replaces the byte(s) _before_ the template with the value of substitution.
- `inPlace`: replaces the _entire_ template with the value of the substitution.

> There is also a `valueBefore` type, but it's only useful in niche scenarios

In fact, we could have also shown the AoA patch above with an `inPlace` patch:

```json
{
  "description": "Set CanUseAoA",
  "template": "00 00 48 02",
  "substitution": "01 00 48 02",
  "type": "inPlace"
}
```

To translate, this patch just tells {{< shortName >}} "replace every occurrence of the byte pattern `00 00 48 02` with the byte pattern `01 00 48 02`". These two examples serve essentially the same purpose, so choose the patch type that makes the most sense.

That being said, you should place a preference towards using `inPlace`: users will be shown a warning when including a `before` type patch since they ignore load order and could undo changes from another patch. 
