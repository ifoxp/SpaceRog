**English** · [Українська](README.uk.md)

# SpaceRog

A 2D mobile space shooter for Android, built in Unity. Survive waves, earn
currency, buy ships and buffs, complete daily quests.

This is the project where I learned the parts of game development that are not
gameplay: monetisation, persistence, store flows, localisation, onboarding.
Those turned out to be most of the work.

## What is in it

Nine scenes wired into the build:

| Scene | Purpose |
|---|---|
| Menu | entry point |
| Survival | the actual game |
| Hangar | your ships |
| ShipShop / BuffShop | spending currency |
| Quests | daily objectives |
| Magazine | in-app purchases |
| Ranking | scores |
| Setting | options and language |

Alongside the gameplay: Unity Ads with interstitials and rewarded video, in-app
purchases, a daily quest system that resets on a schedule, a tutorial and hint
system for new players, and Unity Localization for multiple languages.

## Technical notes

Unity 2021.3.4f1, targeting Android.

Progress is persisted through `PlayerPrefs` — ships owned, buffs, currency,
quest state and scores. This is the simplest thing that works at this size.

Rewarded video is wired into the reward loop rather than bolted on: watching an
ad is one of the ways to earn currency, so the ad has a place in the game's
economy instead of only interrupting it.

## Honest assessment

The code carries the marks of being my first shipped game. `GameManager` is a
static singleton assigned in `Start()`, which is fragile if script execution
order ever changes. `PlayerFly.cs` grew into a 458-line monolith. Naming is
inconsistent in places, and some `PlayerPrefs` keys are built by string
concatenation.

None of that is hidden here, because the project is still worth showing: it is
a complete game with the full commercial surface working, and building that end
to end taught me more than a cleaner but unfinished prototype would have.

## Building

Open in Unity 2021.3.4f1 and build for Android. The keystore is not included.

---

**Chekaliuk Dmytro** · [@ifoxp](https://github.com/ifoxp) ·
[ifoxp.top](https://ifoxp.top) · Telegram [@ifoxp](https://t.me/ifoxp) ·
Discord `ifoxp`
