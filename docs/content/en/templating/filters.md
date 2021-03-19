---
title: "Filters and conversions"
linkTitle: "Introduction to Filters"
weight: 31
---

Filters are one of the most useful parts of the Liquid template engine we're using since it allows you to write some or all of your mod files in more "natural" formats then have {{< shortName >}} convert it to the underlying hex for you.

## Basics

The Liquid filter syntax looks like this:

```text
{{ someValue | filterName }}
```

In short, you want to pipe (i.e. `|`) a value through one or more filters to get a result.

#### Example

Let's look at a simple example: text. In an earlier example we saw using the special `text:` syntax to use a raw string in a match rather than having to convert it to hex first. You can do this with filters too! To reuse the example: 

```json
{
    "_meta": {
        // removed for brevity
    },
    "FilePatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
            {
                "name": "Prez Unlock",
                "patches": [
                    {
                        "description": "Set Pilot count to 2",
                        "template": "00 00 00 00 1C 01",
                        "substitution": "02 00 00 00 1C 01",
                        "type": "inPlace",
                        "window": {
                            "after": "{{ 'PW-Mk.I' | string }}",
                            "before": "{{ 'MG-21' | string }}"
                        }
                    }
                ]
            }
        ]
    }
}
```

You'll see that in our `window.after` and `window.before` we could use the original string, run it through the `string` filter and {{< shortName >}} will automatically change those values to their hex equivalents. Do take note that string values (like MG-21) do need to be quoted when they're used in a template (i.e. `'MG-21` in the example above).

## Number Conversions

One of the other most common uses for {{< shortName >}}'s filters is for converting different numbers to hex automatically. For example, if you wanted to change every plane with a `RollInterpSpeed` statistic of 2 to 2.5, you could use this patch:

```json
{
    "description": "Set RollInterpSpeed",
    "template": "{{ 2 | float }}D5 01 00 00 00 00",
    "substitution": "{{ 2.5 | float }}D5 01 00 00 00 00",
    "type": "inPlace"
}
```

When the patch is run, {{< shortName >}} will automatically convert 2, to its appropriate hex equivalent _as a floating point value_, and likewise for 2.5. So that patch will effectively "become": 

```json
{
    "description": "Set RollInterpSpeed",
    "template": "00 00 00 40 D5 01 00 00 00 00",
    "substitution": "00 00 20 40 D5 01 00 00 00 00",
    "type": "inPlace"
}
```