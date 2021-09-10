---
title: "Changelog"
weight: 99
---

Note that not all releases may appear here, and some releases will only include changes for {{< toolName >}}, the Builder or the {{< shortName >}} engine.

### v0.2.0

#### PSM: 
- Running PSM manually will now prompt before closing so you can see the output
- Adds support for checking the engine version of presets before running
- Only replaces DB_Aircraft if actually required
- Prevents missing unpacked files from failing build

#### {{< shortName >}} Engine:
- Adds support for specifying value ranges in `modifyPropertyValue`
- Adds support for type loader parameters
- Adds new struct match fragment
- Adds `BoolProperty` support to `arrayPropertyValue`
- Correctly load and write `umap` files
- Adds support for partial modifications with `arrayPropertyValue`

#### {{< shortName >}} Builder:
- Includes engine version with built presets for PSM compatibility
- Cleaner (and smaller) preset files by trimming out unused properties

### v0.1.3

- Fix GOG game detection
- Internal refactoring for merge components and reports

### v0.1.2

- Improve fallback behaviour when game not located correctly

### v0.1.1

- Fix custom skin slot detection
