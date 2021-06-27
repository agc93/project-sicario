---
title: "Including Extended Metadata"
linkTitle: "Extended Metadata"
weight: 40
---

As we covered in the earlier sections, you can provide some very basic metadata on your mod in the special `_meta` object. There's actually a few extra bits of information you can also optionally provide, that will be covered in more detail below.

### `_meta`

As a refresher, the `_meta` object is a simple object just allows you to add a little metadata about your patch mod that will be shown to users. The `displayName` (as you'd guess) is the name that is shown to users. You can also include a `description` key in this object if you'd like (it's not required though). The `author` field is used to show the author of a patch mod, but note that if you upload your mod to a {{< shortName >}} server, the author field will be overwritten (to prevent impersonation).

### `_sicario`

The `_sicario` object is where we put app-specific metadata about your mod. The most important/common ones will be `group`, `private` and `preview`

|Key|Type|Meaning|
|:--|:---|:------|
|`group`|`string`|Used to group multiple mods together in the UI|
|`private`|`bool`|Only available to the uploader|
|`preview`|`bool`|Marks a mod as preview/unstable|

All of these properties are optional.

#### Group (`group`)

```json
"_sicario": {
    "group": "NPC Tweaks"
}
```

Including a group will tell {{< shortName >}} to visually group matching mods together in the UI. This is optional and should only be used where it makes sense, or you will risk your mod being completely hidden from users

#### Private (`private`)

```json
"_sicario": {
    "private": true
}
```

Marking a mod as private means it will only be visible to the uploader, not to other users. Note that uploaded mods will often be automatically set to `private` and that private mods are (obviously) only visible while signed in.

#### Preview/Unstable (`preview`)

```json
"_sicario": {
    "preview": true
}
```

Marking a mod as preview effectively flags it as potentially incomplete or unstable. These mods (whether public or private) will be hidden by default, but can be enabled from the build view.

#### Overwrites (`overwrites`)

This is a convenience flag that Sicario uses internally, and that it will show to the user to warn them if a patch might overwrite other mods. I'd urge patch authors to set this flag if their mod changes properties without checking current values.