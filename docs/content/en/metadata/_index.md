---
title: "Including Extended Metadata"
linkTitle: "Extended Metadata"
weight: 40
---

As we covered in the earlier sections, you can provide some very basic metadata on your mod in the special `_meta` object. There's actually a few extra bits of information you can also optionally provide.

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

Including a group will tell Sicario to visually group matching mods together in the UI. This is optional and should only be used where it makes sense, or you will risk your mod being completely hidden from users

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