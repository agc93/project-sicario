### Patch Metadata (`_meta`)

The first thing you'll usually find in a Sicario patch file is the `_meta` object. This simple object just allows you to add a little metadata about your patch mod that will be shown to users. The `displayName` (as you'd guess) is the name that is shown to users. You can also include a `description` key in this object if you'd like (it's not required though).

### The File Patches (`FilePatches`)

The `FilePatches` object is the main "substance" of a Sicario patch. It is a dictionary grouping the files to be edited with sets of patches to be applied to that file.

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

Now we hit the real meat of a Sicario patch: the actual hex edit to make. Here's the example for enabling AoA for all aircraft:

```json
{
  "description": "Set CanUseAoA",
  "template": "00 48 02",
  "substitution": "01",
  "type": "before"
}
```

In plain English, this object just tells Sicario "replace the byte immediately **before** `00 48 02` with a `01`".

##### Template and Substitution

These are the actual values Sicario will be (respectively) looking for and inserting into the binary file. When it runs, Project Sicario will load the file into memory, look for _any_ appearance of the **template** value and then apply the **substitution**. How exactly the substitution is applied varies based on the patch _type_.

##### Types

The two main types of patches currently in use are:

- `before`: replaces the byte(s) _before_ the template with the value of substitution.
- `inPlace`: replaces the _entire_ template with the value of the substitution.

In fact, we could have also shown the AoA patch above with an `inPlace` patch:

```json
{
  "description": "Set CanUseAoA",
  "template": "00 00 48 02",
  "substitution": "01 00 48 02",
  "type": "inPlace"
}
```

To translate, this patch just tells Sicario "replace every occurrence of the byte pattern `00 00 48 02` with the byte pattern `01 00 48 02`". These two examples serve essentially the same purpose, so choose the patch type that makes the most sense.