---
description: "Use when creating, editing, or reviewing relic classes in the Relics/ directory. Covers the full relic API: DevilFruitRelic base, hooks, hover tips, localization patterns, and image conventions."
applyTo: "Relics/**"
---

# Relic Authoring

All relics live in `Relics/` and inherit either `DevilFruitRelic<TCardPool>` (most common) or `CustomRelic` directly. The namespace is `RayzorBladeOnePiece.Relics`.

Auto-discovery registers relics — no manual entry in `MainFile.cs` required.

## Devil Fruit Relic (standard pattern)

Use this for every relic that unlocks a Devil Fruit card pool and optionally grants starter cards.

```csharp
[Pool(typeof(SharedRelicPool))]
public class MyFruitRelic : DevilFruitRelic<MyFruitCardPool>
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // Optional — cards added to the player's deck when the relic is obtained:
    protected override IEnumerable<CardModel> PickUpCardRewards =>
        [Owner.RunState.CreateCard<MyStarterCard>(Owner)];

    // Optional — hover tips for referenced cards or powers:
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<MyStarterCard>()];
}
```

`DevilFruitRelic<TCardPool>` automatically:
- Adds `TCardPool` cards to **card rewards** during the run.
- Adds `TCardPool` cards to the **merchant** card pool.
- Calls `AfterObtained` to hand the player the `PickUpCardRewards`.

The `[Pool(typeof(SharedRelicPool))]` attribute is **inherited** from `DevilFruitRelic` — do not add it again on the subclass.

Valid `RelicRarity` values: `Starter`, `Common`, `Uncommon`, `Rare`, `Shop`, `Boss`, `Special`.

## Custom Relic (direct CustomRelic inheritance)

For relics that don't represent a Devil Fruit card pool, inherit `CustomRelic` directly and implement hooks as needed.

```csharp
public class MySpecialRelic : CustomRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterObtained() { /* one-time setup */ }
}
```

## Available Hooks

| Hook | Signature | Typical use |
|------|-----------|-------------|
| `AfterObtained` | `async Task()` | One-time effect when the relic is picked up (e.g. add cards to deck). |
| `PickUpCardRewards` | `IEnumerable<CardModel>` (property) | Return cards to add to the player's deck on pickup (used by `DevilFruitRelic`). |
| `ModifyCardRewardCreationOptions` | `(Player, CardCreationOptions) → CardCreationOptions` | Inject extra card pool into reward offers. |
| `ModifyMerchantCardPool` | `(Player, IEnumerable<CardModel>) → IEnumerable<CardModel>` | Inject extra cards into merchant inventory. |

## Hover Tips

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromCard<MyStarterCard>(), HoverTipFactory.FromPower<MyPower>()];
```

## Relic ID Derivation

Class name → localization/image ID:  
`SlowSlowFruit` → screaming snake → `SLOW_SLOW_FRUIT` → lowercase → `slow_slow_fruit`

## Localization Keys

Add entries to `RayzorBladeOnePiece/localization/deu/relics.json` in ascending key order:

| Key | Purpose |
|-----|---------|
| `RAYZORBLADEONEPIECE-<ID>.title` | Display name |
| `RAYZORBLADEONEPIECE-<ID>.description` | Relic description shown in UI |
| `RAYZORBLADEONEPIECE-<ID>.flavor` | Flavour text (optional but recommended) |

### Example

```json
"RAYZORBLADEONEPIECE-MY_FRUIT.description": "Schalte besondere Karten frei. Erhalte [gold]MyStarterCard[/gold].",
"RAYZORBLADEONEPIECE-MY_FRUIT.flavor": "Flavour text here.",
"RAYZORBLADEONEPIECE-MY_FRUIT.title": "Meine Frucht"
```

Tags: `[gold]keyword[/gold]` for highlighting card/keyword names in descriptions.

## Image Files

Three files are required per relic (all `.png`):

| File | Path |
|------|------|
| Small icon | `RayzorBladeOnePiece/images/relics/{id}.png` |
| Outline (for deck/tooltip border) | `RayzorBladeOnePiece/images/relics/{id}_outline.png` |
| Big icon | `RayzorBladeOnePiece/images/relics/big/{id}.png` |

`CustomRelic` falls back to `placeholder.png` / `placeholder_outline.png` automatically if the real image is missing.

## Do Not

- Add `[Pool(typeof(SharedRelicPool))]` on a subclass of `DevilFruitRelic` — the attribute is already on the base.
- Create `.import` files — Godot generates them automatically.
- Hard-code card creation outside of `PickUpCardRewards` / `AfterObtained` — always use `Owner.RunState.CreateCard<T>(Owner)`.
