# Project Sicario

> An **experimental** new way of building and merging datatable mods for Project Wingman.

The Sicario Patcher can attempt to "merge" multiple data table edits together into a single mod to improve compatibility and user choices. Underneath the fancy (terrible) UI, Project Sicario just uses [HexPatch](https://github.com/agc93/HexPatch) to apply a sequence of byte-level edits to any number of game files.

Project Sicario specifically adds some extra features over the top of HexPatch to make authoring and applying patches easier:

- Templating for patches to semi-dynamically create replacements
- Auto-length patching for `uasset` files
- Parameter support to allow user input (when combined with patch templating)
- A big old Blazor UI on top of everything
- Optional auto-packing of the final mod with u4pak (using HexPatch.Build)

### Authentication

There's also a **very rudimentary** authentication layer with Discord sign-in support to only allow for authenticated uploads. Due to concerns with distributing patches, the default configuration is to disallow uploads to all users, but allow builds with loaded patches to any authenticated user. This can be tweaked with the app configuration.

### Patches

Like HexPatch, Sicario patches are just JSON files with a `.dtm` file extension and a specific format. All patch files are deserialized to a `WingmanMod` type, so you can use that to guide you until docs are done (maybe). Both the `template` and `substitution` keys support templating using the Liquid language (powered by the [Fluid](https://github.com/sebastienros/fluid/) engine).