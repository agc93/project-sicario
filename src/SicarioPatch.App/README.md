# SicarioPatch.App

This is the main Blazor app for configuring and building merged mods from Sicario patches, as well as a series of shared components and a basic upload/management UI.

## Configuration

Mostly for my own reference, here's a short summary of some of the major config options and how they're used:

|Key|Usage|
|:--|:----|
|`Files`|Deserializes to a `SourceFileOptions` used to source game files|
|`Mods`|Deserializes to a `ModLoadOptions` used by the `WingmanModLoader` to load patch files|
|`TemplateModelsFile`|Absolute path to an INI file used to populate the model in the template context|
|`TemplateModels`|Dictionary to directly add specific variables to the template context|
|`Access`|Deserializes to an `AccessOptions` object for allowed users/uploaders|
|`Discord`|Object with the `ClientId` and `ClientSecret` for Discord configuration|
|`SignatureFiles`|List of absolute file paths to be included with output files in a `_meta/sicario` package path|