---
title: "Building a Project Sicario mod"
linkTitle: "Building a Sicario mod"
weight: 20
---

Asset patches are the most reliable kind of patch and the one most authors should be using for most scenarios as it allows you to directly edit datatables using the same names, properties and structures the game uses, but uses a unique syntax to express this without having to learn UE4 (or development!). The first section of these docs will walk you through creating asset patches and they are what I recommend most authors to use, so you can likely skip over the section on hex patches.

Hex patches (also referred to as file patches) are the older method of building datatable mods for Project Wingman. They involve directly editing the binary parts of game files, replacing certain bytes with new bytes (generally represented in hex form). This approach still works, and it still _supported_ by {{< appName >}}, but is not recommended for most scenarios as it is less robust, more likely to break in case of game updates and is just generally kinda messy. With that in mind, {{< shortName >}} was originally built with hex patching in mind so file patching is still very much supported and documentation for working with hex patches is still included.