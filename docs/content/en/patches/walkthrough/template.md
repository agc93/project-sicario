---
linkTitle: "Property Templates"
title: "Understanding Property Templates"
weight: 22
---

If you oversimplify a UE4 cooked asset file (that's Sicario's job), they are essentially a collection of properties, some of them with their own properties and so forth. The `template` value is essentially a filter to control which properties the `value` is applied to, using a set of simplistic filters. The syntax can seem kinda of daunting but it's not so bad once you understand how it works.

The basic format is this: a data type at the start, then a set of filters separated by `.`s.

When it runs a patch, Sicario will load the file using the specified type, it will run every property in the file through the chain of filters and then run the actual patch type and value on all the matching properties.

Each filter in the chain is run in order so only properties that match each step will be included in the final result.

## Data Types

The first thing in a template is the data type. **As of writing** there is only one supported data type: data tables, specified with the `datatable:` prefix. This loads the file as a UE4 DataTable, returning every row of the table as a named property.

## Filters

This is the most daunting part to understand, but really isn't as bad as it looks. Let's explore this using our example mod: the AoA unlocker. Here's that mod's template:

```
datatable:{'BaseStats*'}.{'CanUseAoA*'}
```

First off, the easy part: the type. The `datatable:` tells Sicario to load the file as a UE4 DataTable and returns every row. Next up is a set of filters separated by `.`s. In our case, both filters are doing the same thing: returning only the struct properties with a name that starts with a certain string, first off `BaseStats`. Next, the result of that filter (i.e. a bunch of properties named `BaseStats`) are passed into the second filter. That filter just returns any struct properties with a name that starts with `CanUseAoA`. All the properties that match that last filter are passed directly to the patch type to have the value modified.

#### Example in Action

If you've ever seen the `DB_Aircraft` file that this example patch is modifying, you'll have noticed it looks something like this abridged sample:

```jsonc
"T-21": {
    "export_type": "RowStruct",
    "FamilyID_114_171BFD3F41B21A7C1797CE91CF694311": "mig21",
    "ID_101_9607548349D1C4AAC5D972B872BB91E4": {
        // trimmed
    },
    "CQ_ID_149_53E0A3174FB70B3F9698058FB61388D9": {
        //trimmed
    },
    "Price_77_50AB72FA43B3127002F5F893321A7AAA": 0,
    // ...trimmed
    "BoneDetails_46_F00ED91D4FDF91F973A7DAAF8A291DBF": {
        "PilotCount_2_65D06B7D492533AA2F8E9DA8254C5747": 1,
        "CondensationPoint_15_187804564BEB3AD239710AB3F72AABDF": 0,
        // ...trimmed
    },
    "BaseStats_90_4FCD5FE44A06097FD9A455ACA4754B1E": {
        "RollInterpSpeed_28_C66FD5244EC8FBAE6B5EC9837FB70B1E": 1.9,
        "PitchInterpSpeed_29_90B4863F4DFF9111D073229AE0C909E9": 2.1,
        "YawInterpSpeed_30_272A70AB493C5265822667AFA304857C": 1.5,
        "MaxSpeed_31_58011513426ABC47552EEFA646C7E89A": 2500.0,
        "Acceleration_34_294F4BA04EA43CF04A560E82B90B7DFA": 55.0,
        "RollSpeed_39_E8C7F3914C6D3146D2166FAF21971CED": 170.0,
        "TurnSpeed_40_5254820C48D4E4E50EBD089AB8C2B12E": 85.0,
        "YawSpeed_37_33AA996848D24FBF807F609931FDC135": 10.0,
        "InterpSpeed_43_B02AFCF7418D3359B814BB9FA4E8DF9F": 2.0,
        "GearLiftVar_47_037DE3944066F0D58637388A1488C773": 25.0,
        "CannonType_50_D23A2D56429DB7C2FD1F3F9BD32989C9": "S_CannonType::NewEnumerator0",
        "CanUseAoA_52_927DA6AE426EEE21F9ECD7AF7E857628": false,
        "VTOL_54_FABB19354B444768DC73E1BF80437EA1": false
    },
    "UIImage_83_026AA95649B0C45015114AAB80EDFCD4": ["mg21_icon", "/Game/Assets/Objects/Aircraft/Mig21/Textures/mg21_icon"],
    "HardpointSlots_122_9A810A7F449CEB12E0CD0C808A4D3162": [2, 2],
    "HardpointCompatibilityList_130_7BA5148945B4572A862CC0ACFFB7AB21": ["stdm", "0,droptank"],
    // ...trimmed
},
"TF-4E": {
    // ...trimmed
}
```

It should be a little clearer now that our `{'BaseStats*'}.{'CanUseAoA*'}` template will match and return the `BaseStats_90_4FCD5FE44A06097FD9A455ACA4754B1E` property for each aircraft, then the second filter will match and return the `CanUseAoA_52_927DA6AE426EEE21F9ECD7AF7E857628` property for each aircraft. Now, just those properties will be run through the patch type to be updated.

## Filter Types

Now, all the examples above were looking for the same thing: struct property types. The template syntax though, is a lot more flexible than that. To see what an **extreme** example looks like: here is another template.

```text
datatable:['F-15C'].[0].{'HardpointSlots*'}.[[1]].<IntProperty='2'>
```

Now if we step through this one again, you'll see a few new filters, but the same basic idea:

- `datatable:` : Loads the file as a data table
- `['F-15C']`: Matches any properties with a name of `F-15C`
- `[0]`: only returns the **first** match (arrays are numbered from zero).
- `{'HardpointSlots*'}`: returns properties from any matched structs with a name that starts with `HardpointSlots`
- `[[1]]`: Matches the second entry _in a matching array property_ (rather than just the first match).
- `<IntProperty='2'>`: Only returns matching properties that are of an `IntProperty` type **and** have a value of `2`.

> The `[]` vs `[[]]` distinction can be tricky. The easiest summary is that `[[]]` matches the _contents_ of an `ArrayProperty`, but `[]` just matches the first property it sees and returns the whole property

Now that's a lot of filters, the result of which is that out of the entire lengthy and complex `DB_Aircraft` datatable, that result returns exactly **one** thing: the _unmodded_ second weapon hardpoint for the FC-15.

While they can be tricky to make, a well-written template will ensure that you only change the values you want and can even ensure that you don't overwrite modded values, allowing users to merge your patch with others better.

## Available Filters

> Please note that this list may not be exhaustive: Sicario is designed to support additional filters easily

Here's a very brief summary of the available filters for use in templates:

|Syntax|Example|Notes|
|-----:|:-----:|:----|
|`[int]`|`[0]`,`[2]`|Matches the `n`th incoming result. Helpful for filtering duplicates, or Mk.1 planes|
|`['string']`|`['F-15C']`, `['MSSL']`|Matches an incoming property with the given name. Doesn't care what type it is or what properties it might have.|
|`[[int]]`|`[[1]]`|Matches the `n`th entry of incoming `ArrayProperty`s, returning the actual entry, not the array itself.|
|`{'string'}`|`{'BoneDetails*'}, `{'UnitType_2_2706'}`|Matches a _child_ property of a StructProperty by name, optionally with partial matching|
|`<string>`|`<IntProperty>`, `<StructProperty>`|Matches only the incoming properties of the specific given type|
|`<string=int>`|`<IntProperty=2>`, `<FloatProperty=1.5>`|Matches only incoming properties of the specific type *and* specific value|
|`<string='string'>`|`<StrProperty='saa,mlaa'>`|Same as above, but for non-numeric types|
|`<string::string>`|`<S_CannonType::NewEnumerator2>`|Matches only enum properties, with the right enum type and the right enum value|

There's also one special filter: `[*]`. That filter will just match everything and return all the properties it receives. This can be useful if you're trying to match a whole datatable for example.