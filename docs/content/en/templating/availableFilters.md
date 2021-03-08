---
title: "Available Filters in Templates"
linkTitle: "Available Filters"
weight: 32
---

For convenience, {{< appName >}} has a **lot** of non-standard filters available for patch files to use that will make a lot of actions much simpler. In no particular order, they are summarized below:

## Number Types

Filters are available for all the commonly used number types to automatically convert them to their respective hex representation:

```liquid
{{ 5.5 | float }}
{{ 2 | int }} # 4-byte int32
{{ 250 | byte }} # 1-byte uint8
{{ 2 | mult: 2 | float }} # will multiply by the given value
```

## Boolean Filters

There's also a few for more easily dealing with boolean states:

```liquid
{{ "true" | bool }} # will return 00 or 01 (01 in this case)
{{ "false" | not }} # inverts, returns "true"
{{ "true" | not | bool }} # inverts and converts, returns 00
```

## Blueprint Filters

There's also one **very** powerful filter specific to working with Blueprint edits: the `row` filter.

This one is used where the binary includes a string that's prefixed by the string's length. Most notably for Project Wingman, this is how loadouts are stored. The `row` filter will include the length of the final string and convert the whole thing to its hex:

```liquid
{{ "0,stdm,saa,mlaa" | row }}
```

```json
{
    "description": "Add weapons to second slot",
    "template": "{{ \"0,saa,mlaa,mlag\" | row }}",
    "substitution":"{{ \"0,mlaa,saa\" | row }}",
    "type": "inPlace"
}
```

## Random Filter

This one is a little trickier to use than the other due to some technical issues we faced. In short, this filter will generate a random number in a specific range.

```json
{
    "description": "Replaces 35 HP (so many planes) with random 10-100 value",
    "template": "{{ 35 | float }}7F 00 00 00 00 00 00 00 73",
    "substitution": "{{ 10 | random: 100 | float }}7F 00 00 00 00 00 00 00 73",
    "type": "inPlace"
}
```

This will generate a random value between 10 (the input value) and 100 (the first argument) that you can then pass through the `float` filter to get the correct type. 