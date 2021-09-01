---
title: "Packing Datatable Mods"
linkTitle: "Blueprint Mods"
weight: 20
---

Things are a little trickier (but not dramatically so) for blueprint/datatable mods.

The most important thing is that your blueprint changes need to already be in a {{< shortName >}} patch. If you've already done that, you're 90% of the way there. If not check out [the docs](../../introduction/_index.md) to build your changes as a {{< shortName >}} patch.

Once you have your patch(es), it's time to turn them into a preset. Don't worry, it's easy.

### Presets

A preset file is just a collection of {{< shortName >}} patches bundled into one standalone file including any inputs they need to run. Just like a patch file, a preset file is just a JSON file with a special extension: `.dtp`. At its most basic, here's what a preset file looks like:

```json
{
  "modParameters": {},
  "mods": []
}
```

The `modParameters` key is optional and can be used to specify any required parameters for the patches in your preset.

> The parameters work the same way as they do in the hosted {{< appName >}} app, just this way the merger doesn't have to prompt for the values to use.

The `mods` key is the important one and is an array of the patches included in the preset. Here is where you would put any patch(es) required for your mod to work. You can either add them as a single mod, or as several: that's up to you.

#### Engine Version

Presets also have an optional `engineVersion` key that can be used to indicate that your preset uses features only available in newer versions of {{< shortName >}}. If you set this value, then earlier versions of {{< toolName >}} will not attempt to load your preset as it might not be able to action them. This value is optional!

### Packaging

Once you have a preset, it's time to get it in the hands of users. You have a couple of options for that.

##### Loose Files

You can always just upload your preset file (i.e. the `dtm` file) to Nexus/ModDB/Discord/wherever you want and let users download it directly. As long as it ends up in their `~mods` folder, {{< toolName >}} will load and build the preset just fine.

##### Embedded in your mod

This one is a tiny bit trickier but way cooler. When you go to *pack* your mod, you can pack your preset file into the PAK file itself and {{< toolName >}} will then unpack it from the PAK during merge and build with those patches. Your preset file can be packed anywhere in the PAK file _as long as_ it's in a folder named `sicario`. {{< toolName >}} will load and merge any `.dtp` files it find in a `sicario` folder anywhere in the PAK file.

That way, users will only have to download and install one file! Obviously, if your mod _requires_ the changes from your patches then your mod will also then require {{< toolName >}} itself.

> You can even use an embedded preset alongside the [custom skin slot support](../skins) to include both a new skin slot and datatable changes