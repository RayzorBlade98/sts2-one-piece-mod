# RayzorBlade OnePiece Mod — Copilot Instructions

A Slay the Spire 2 mod built with C# + Godot using **alchyr.sts2.baselib**. The mod adds One Piece Devil Fruit characters as playable card sets. Currently implemented: Slow-Slow Fruit, Rust-Rust Fruit, Munch-Munch Fruit (+ WapoMetal sub-set). Many more fruits are planned.

## Build

Build and publish via Rider: **dotnet publish** (the standard RayzorBladeOnePiece project). The output lands in `bin/ExportRelease/net9.0/`. The mod manifest is `RayzorBladeOnePiece.json`.

## Architecture

```
Cards/
  CustomCard.cs               ← abstract base for all cards
  DevilFruitCardPool.cs       ← abstract base for per-fruit card pools
  <FruitName>/                ← one folder per Devil Fruit
    <FruitName>CardPool.cs    ← card pool for that fruit
    *.cs                      ← individual cards
Powers/
  CustomPower.cs              ← abstract base for all powers
  *.cs                        ← individual powers
Relics/
  CustomRelic.cs              ← abstract base for all relics
  DevilFruitRelic.cs          ← generic base: integrates a TCardPool into rewards
  *.cs                        ← individual relics
Extensions/
  ImagePathExtensions.cs      ← shared image-path resolution helpers
MainFile.cs                   ← mod entry point ([ModInitializer])
RayzorBladeOnePiece/
  images/
    card_portraits/           ← small: {id_lowercase}.png, big: big/{id_lowercase}.png
    powers/                   ← small: {id_lowercase}.png, big: big/{id_lowercase}.png
    relics/                   ← small + _outline + big/{id_lowercase}.png
  localization/
    deu/cards.json            ← German strings (only language currently)
    deu/powers.json
    deu/relics.json
```

**Auto-discovery** — BaseLib's MSBuild analyzers auto-register everything. No manual registration in `MainFile.cs` except `harmony.PatchAll()`.

## Adding a New Devil Fruit

1. Create `Cards/<FruitName>/` folder.
2. Add `<FruitName>CardPool.cs` inheriting `DevilFruitCardPool` — set `Title`, `DeckEntryCardColor`, `EnergyOutlineColor`.
3. Add cards decorated with `[Pool(typeof(<FruitName>CardPool))]`.
4. Add a relic in `Relics/` inheriting `DevilFruitRelic<<FruitName>CardPool>`.
5. Add localization keys to `localization/deu/cards.json`, `powers.json`, `relics.json`.
6. Add image files under `RayzorBladeOnePiece/images/`.

## Card Conventions

Cards inherit `CustomCard`, are decorated with `[Pool(typeof(<FruitCardPool>))]`, and live in `Cards/<FruitName>/`. See [cards.instructions.md](instructions/cards.instructions.md) for the full API: DynamicVar types, OnPlay patterns, upgrades, keywords, glow conditions, and more.

## Power Conventions

Powers inherit `CustomPower` and live in `Powers/`. See [powers.instructions.md](instructions/powers.instructions.md) for the full API: DynamicVar types, override hooks, helper commands, hover tips, localization patterns, and health bar forecasts.

## Relic Conventions

Devil Fruit relic (most common):
```csharp
[Pool(typeof(SharedRelicPool))]
public class MyFruitRelic : DevilFruitRelic<MyFruitCardPool>
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<CardModel> PickUpCardRewards => [Owner.RunState.CreateCard<MyStarterCard>(Owner)];
}
```

See [SlowSlowFruit.cs](../Relics/SlowSlowFruit.cs) and [DevilFruitRelic.cs](../Relics/DevilFruitRelic.cs).

## Card Pool Conventions

```csharp
public class MyFruitCardPool : DevilFruitCardPool
{
    public override string Title => "My Fruit";
    public override Color DeckEntryCardColor => new("840240");  // hex string
    public override Color EnergyOutlineColor  => new("651565");
}
```

## Naming Conventions

| Entity | Pattern | Example |
|--------|---------|---------|
| Card class | PascalCase, descriptive | `KyubiRush`, `WapoMetalLaser` |
| Power class | `*Power` suffix | `SlowBeamPower`, `RustFormPower` |
| Card pool | `*CardPool` suffix | `SlowSlowFruitCardPool` |
| Relic | Fruit or item name | `SlowSlowFruit` |
| Dynamic var keys | PascalCase strings | `"RepeatOnSlowed"`, `"BombDamage"` |

Card IDs are auto-generated: `KyubiRush` → `RAYZORBLADEONEPIECE-KYUBI_RUSH`.

## Localization

Strings live in `RayzorBladeOnePiece/localization/deu/`. Add entries for every new card/power/relic:
- Card: `RAYZORBLADEONEPIECE-<ID>.title` and `RAYZORBLADEONEPIECE-<ID>.description`
- Power: same key pattern in `powers.json`
- Relic: same key pattern in `relics.json`

Description variables use `{VarName:diff()}` for upgradeable values. Inline tags: `[gold]keyword[/gold]`, `[blue]value[/blue]`. Conditional text: `{IfUpgraded:show:text}`, `{InCombat:combat_text|fallback}`.

## VFX Paths

Use paths from existing cards as reference (e.g. `"vfx/vfx_attack_blunt"`, `"vfx/vfx_attack_slash"`). Do not invent new paths — pick the closest match from the codebase.

## Images

Place images as `.png` files. Paths are auto-resolved from the entity's lowercase ID:
- Card portrait: `RayzorBladeOnePiece/images/card_portraits/{id}.png`
- Power icon: `RayzorBladeOnePiece/images/powers/{id}.png`
- Relic icon: `RayzorBladeOnePiece/images/relics/{id}.png` + `{id}_outline.png`

`.import` files are auto-generated by Godot — never create them manually.
