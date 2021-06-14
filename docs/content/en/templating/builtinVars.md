---
title: "Commonly Used Variables"
linkTitle: "Common Variables"
weight: 34
---

This is a little more niche and mostly just used to save time while _building_ patches, but {{< shortName >}} also supports a handful of the most commonly used blueprint "delimiters" available as vars in your patches.

The variables are generally available as `BlueprintName.PropertyName` so, for example, `DB_Aircraft.MaxSpeed` or `DWeaponDB.ReloadTime`.

The complete list of currently available variables is included below.

## Usage Example

To use an example, here's how a patch might change the turn speed and roll speed statistics of a playable aircraft:

```json
{
    "_meta": {
        //...trimmed for brevity
    },
    "filePatches": {
        "ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp": [
        {
            "name": "Stat Changes",
            "patches": [
                {
                    "description": "Set RollSpeed",
                    "template": "{{180|float}}{{DB_Aircraft.RollSpeed}}",
                    "substitution": "{{350|float}}{{DB_Aircraft.RollSpeed}}",
                    "type": "inPlace",
                    "window": {
                        "after": "text:Sk.37",
                        "before": "text:F-15SMTD"
                    }
                }
                {
                    "description": "Set TurnSpeed",
                    "template": "{{95|float}}{{DB_Aircraft.TurnSpeed}}",
                    "substitution": "{{180|float}}{{DB_Aircraft.TurnSpeed}}",
                    "type": "inPlace",
                    "window": {
                        "after": "text:Sk.37",
                        "before": "text:F-15SMTD"
                    }
                }
            ]
        }
        ]
    }
}
```

## Available Variables

> The list below was updated as of 15/6/21.

> Ignore the *values* of the keys below as they may be out of date, simply refer to the variable names.

```ini
[DB_Aircraft]
Available = 
Unlocked = 
Purchased = 
Sellable = 
DemoOverride = 
DemoUnlocked = 
CQ_Available = 
CQ_Unlocked = 
CQ_Purchased = 
CQ_Level = 
CQ_Price = 
RequiresCampaignFinish = 
Price = 
HasGun = 
PilotCount = 
EngineCount = 
DisableAfterburner = 
RollInterpSpeed = 
PitchInterpSpeed = 
YawInterpSpeed = 
MaxSpeed = 
Acceleration = 
RollSpeed = 
TurnSpeed = 
YawSpeed = 
InterpSpeed = 
GearLiftVar = 
CannonType = 
CanUseAOA = 
VTOL = 
Region = 
FixedLoadout = 

[DWeaponDB]
WeaponAmmo = 
ReloadTime = 
MaxProjectile = 
MaxMultiLock = 
TargetType = 
LockonRange = 
FiringType = 
SalvoMaxMultiLock = 
IsAvailable = 
CQOnly = 

[DAirUnitNPC]
MinSpeed = 
DefaultSpeed = 
MaxSpeed = 
Acceleration = 
RollSpeed = 
TurnSpeed = 
YawSpeed = 
BaseHP = 

[DB_AirshipData]
CruiseSpeed = 

[DB_ProjectWingmanLevelList]
Available = 
IsCampaignLevel = 
MissionCompletionBonus = 

[DB_GroundUnit]
ScoreValue = 

[CQ_AlliedSquadBuyTable]
UnitLimit = 
InitialCost = 
SubsequentCost = 
CordiumCost = 
UpgradeCost = 
```
