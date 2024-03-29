---
title: "Packing Aircraft Skins"
linkTitle: "Skins"
weight: 10
---

In general, there's only one thing that {{< toolName >}} adds that skin makers should be aware of but it's a big one: automatically adding skin slots to aircraft!

If you offer a {{< toolName >}}-compatible version of your skin, {{< toolName >}} users will not have to replace a vanilla skin slot or worry about compatibility as {{< toolName >}} will dynamically add a new skin slot to the relevant aircraft to use your skin when it builds a merged mod.

Now, to do this does require a small change on your end, but it's pretty easy so offering a {{< toolName >}}-compatible skin mod doesn't require a lot of work.

### Packing

The important thing is that {{< toolName >}} bases its skin slots on the **path** you use when packing your skin. Rather than the usual quagmire of inconsistent names and paths {{< toolName >}} uses a simple format to detect skins it needs to add a slot for:

```text
ProjectWingman/Content/Assets/Skins/<AIRCRAFT-NAME-HERE>/<YOUR-SKIN-NAME-HERE>.uasset
```

That's it. You don't need to change anything about your skin itself, just give it a unique name (so that it doesn't conflict with other skins), cook it in the `Content/Assets/Skins/<AIRCRAFT-NAME-HERE>` path and pack it in the path as above.

Okay, there is one gotcha: the `<AIRCRAFT-NAME-HERE>`. Since {{< toolName >}} can't know the _player-facing_ name of every aircraft in the game, it actually uses the aircraft names from the `DB_Aircraft` datatable. As of v1.0.4, check the table below for the correct name to use.

> An advantage of this approach is that this same pattern also works for custom aircraft. Want to add a skin for the Overpowered Sk.37 mod? Just use `OP-37`. Check with the author for the right name to use.

### Example

So for example, you could pack a mod that includes a `Content/Assets/Skins/SU-37/Splinter_Red.uasset` and {{< toolName >}} will add a new skin slot to the Sk.37 for your skin. More importantly, if the user installs another skin mod that includes `Content/Assets/Skins/SU-37/FederationFerris.uasset` then PSM will add _two_ slots to the Sk.37, one for the Splinter camo, one for the Ferris scheme.

Same for if your PAK file includes multiple versions of a skin with different markings you could pack it like this:

```text
Content/Assets/Skins/MiG-29/Serdyukov-Federation.uasset
Content/Assets/Skins/MiG-29/Serdyukov-Monarch.uasset
Content/Assets/Skins/MiG-29/Serdyukov-Clean.uasset
```

When PSM runs on the user's install, it will add _three_ slots to the MG-29 with each variant of those skins. 

> _That being said_, please don't go mad with variants. The game will almost certainly have a maximum number of slots, we just haven't found it yet.

### Aircraft Names

|In-Game|Internal Name| |In-Game|Internal Name|
|:-----:|:-----------:|-|:-:|:-:|
|T-21|`T-21`||T/F-4|`TF-4E`|
|MG-21|`MiG-21`||SV-37|`AJS-37`|
|F/E-4|`F-4E`||F/C-16|`F-16C`|
|CR.105|`CF-105`||Sk.25U|`Su-25`|
|MG-31|`MIG-31`||F/D-14|`F-14D`|
|MG-29|`MiG-29`||Accipiter|`AV-8`|
|F/E-18|`F-18E`||F/C-15|`F-15C`|
|Sk.27|`SU-27`||Sk.37|`SU-37`|
|F/S-15|`F-15SMTD`||VX-23|`F-22`|
|CHIMERA|`ACG-01`||SP-34R|`SPEAR`|
|PW-Mk.1|`PW-001`||||

> You can even use custom skin slots in the same PAK as [an embedded preset](../mods) to include both a new skin slot and datatable changes