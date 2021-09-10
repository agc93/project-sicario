---
linkTitle: "Patch Types"
title: "Understanding Patches and Patch Types"
weight: 23
---

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

In plain English, this object just tells {{< shortName >}} "replace the value of any `CanUseAoA` properties with `true`".

#### Template and Value

These are the actual values {{< shortName >}} will be (respectively) looking for and inserting into the binary file. When it runs, {{< appName >}} will load the file into memory, parse the file (more on that below), look for _any_ values that match the **template** value and then apply the **value**. How exactly the value is applied varies based on the patch _type_.

Templates are covered in more detail in the [Property Templates](../template) section.

#### Types

The main types of patches currently in use are:

|Type|Notes|
|:--:|:----|
|`propertyValue`|This is by far the most common/useful and for good reason: it's the most useful and versatile type. It sets the value of matching properties to the given value.|
|`arrayPropertyValue`|Sibling type to `propertyValue` that creates/inserts an array value to the given properties using a simple array syntax|
|`modifyPropertyValue`|Modifies a numeric value _based on it's existing value_.|
|`textProperty`|Specialized patch type for working UE4's TextProperty fields more effectively.|

Most of these types use a common convention for the `value` field: `DataType:value`. This ensures that the properties are set with the correct data types the game uses. Note that other patch types might not use the same convention.

##### Patch Type Details

###### `modifyPropertyValue`

The `modifyPropertyValue` patch type has its own syntax to define exactly what sort of change you want, consisting of an operation (`+`,`-`,`*`,`/`) followed by the value to apply. For example, to double incoming float values:

```json
{
  "template": "datatable:{'BaseStats*'}.{'MaxSpeed*'}.<FloatProperty>",
  "value": "FloatProperty:*2",
  "type": "modifyPropertyValue"
}
```

You can also include a min-max range in the `value` field to lock the final result inside a range. For example, `IntProperty:+6(0-10)` will add 6 to the incoming integer, but ensure the final result is between 0 and 10, regardless of input.

###### `arrayPropertyValue`

The `arrayPropertyValue` uses the type prefix to set the type of the _items_ in the array, not the array itself. For example: 

```json
{
  "template": "datatable:['ACG-01'].[0].{'HardpointSlots*'}",
  "value": "IntProperty:[2,2,4,1]",
  "type": "arrayPropertyValue"
}
```

You can also prefix the array itself in the `value` key with a `-` or `+` to modify an existing array value, so a `value` of `IntProperty:+[2]` will _add_ a single `2` element to the matched array.

##### Special Patch Types

The patch types above will handle the vast majority of {{< shortName >}} patches reasonably well, but a few of them work a little differently and can be easier to get wrong (especially the first time). 

There's also two patch types that deserve extra attention: `objectRef` and `duplicateEntry`. These two patch types are both much more complex to get working right and I'd recommend you start with the simpler changes handled by the regular patch types.

###### `textProperty`

The `textProperty` patch type is just a variation on the `propertyValue` type that can set the value of a `TextProperty` more cleanly. The format for the `value` is simple `'<key>':'<string-value>'`. Note however, that the key is a little dangerous: if you set an existing key's value using this patch it has the potential to change the text for unrelated objects. Unless you really need otherwise, I'd recommend letting {{< shortName >}} generate a key for you which can be done with the special `*` syntax: 

```json
{
    "description": "Change the FC-16's role",
    "template": "datatable:['F-16C'].[0].{'Role*'}.<TextProperty>",
    "value": "*:'Multirole'",
    "type": "textProperty"
}
```

When it runs, {{< shortName >}} will set the `TextProperty`'s key to a randomly generated unique ID to prevent any conflicts/issues

###### `duplicateEntry` and `duplicateProperty`

This one is pretty powerful as its the most reliable way to _add_ new properties to an existing object, by duplicating an existing property of an object. Adding a new property just requires matching the parent property, then specifying the source property's name and and the duplicate property's (unique) name.

Here's a patch that adds a new weapon based on the STDM:

```json
{
    "description": "Clone MSSL",
    "template": "datatable:[*]",
    "value": "'MSSL'>'MSTM'",
    "type": "duplicateEntry"
}
```

The template just matches the top-level datatable (i.e. all entries in the table) and the value follows the simple pattern of `'SourceName':'TargetName'` so you can specify the row to be duplicated (i.e. the `MSSL` row) and what the new row should be named (`MSTM`). This will take a copy of the `MSSL` property data and add it as a new property to the end of the datatable.

> `duplicateProperty` follows the same logic but for duplicating properties on an object rather than the whole object. It's a bit more niche.

Note that as covered in the "Patch Set Grouping" section above, patch sets are _matched_ together, so if you add a new property then want to match that property in later patches, make sure they're in a separate patch _set_.

###### `objectRef`

The `objectRef` patch type is used to change what object a property refers to. Internally, UE4 refers to objects using links that seem kind of convoluted on the surface: the property just has a number referring to an entry in the linked class list, which refers to another entry in the linked class list which then refers to an entry in the header list. That's a mess, so the `objectRef` patch type will handle updating each of those parts in order.

For example, here's a patch that changes what texture the first skin slot on the F-16C uses:

```json
{
    "description": "Change the default skin 1",
    "template": "datatable:['F-16C'].[0].{'SkinLibraryLegacy*'}.[[0]]",
    "value": "'F16Custom_01':'/Game/Assets/Objects/Aircraft/F16C/Textures/Skin/F16Custom_01'",
    "type": "objectRef"
}
```

You'll note that the template syntax is the same as always: just use the template that matches the linked property you want to change. The value syntax though is a little different, and can best be summed up as `'ObjectName':'ObjectPath'`. Simply specify what object that property should link to and the patch type will handle updating the linked classes and header list.