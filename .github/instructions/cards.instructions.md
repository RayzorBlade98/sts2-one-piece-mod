---
description: "Use when creating, editing, or reviewing card classes in the Cards/ directory. Covers the full card API: DynamicVar types, OnPlay patterns, upgrade patterns, keywords, hover tips, glow conditions, energy-X cards, card pile commands, calculated damage, and block-from-damage."
applyTo: "Cards/**"
---

# Card Authoring

All cards inherit `CustomCard` and live in `Cards/<FruitName>/`. Register them to a pool with `[Pool(typeof(<FruitCardPool>))]`.

```csharp
[Pool(typeof(MyFruitCardPool))]
public class MyCard() : CustomCard(
    baseCost: 1,
    type: CardType.Attack,
    rarity: CardRarity.Common,
    target: TargetType.AnyEnemy)
{ ... }
```

Valid `target` values: `AnyEnemy`, `AllEnemies`, `RandomEnemy`, `Self`, `None`.  
Valid `rarity` values: `Common`, `Uncommon`, `Rare`, `Token`, `Curse`.

## DynamicVar Types

Declare all numeric values in `CanonicalVars` — never hard-code values in `OnPlay`.

| Type | Usage                                                |
|------|------------------------------------------------------|
| `new DamageVar(5M, ValueProp.Move)` | Standard scaled attack damage                        |
| `new RepeatVar(3)` | Hit count (`DynamicVars.Repeat.IntValue`)            |
| `new BlockVar(8M, ValueProp.Move)` | Block amount                                         |
| `new EnergyVar(3M)` | Energy gain (`DynamicVars.Energy.BaseValue`)               |
| `new PowerVar<MyPower>(5M)` | Amount for a specific power — key is `nameof(MyPower)`, access via `DynamicVars[nameof(MyPower)]` |
| `new("MyKey", 2M)` | Custom named var — access via `DynamicVars["MyKey"]` |
| `new CalculationBaseVar(0M)` + `new ExtraDamageVar(1M)` + `new CalculatedDamageVar(ValueProp.Move).WithMultiplier(...)` | Equation-based damage (e.g. scales with deck size)   |

Access helpers: `DynamicVars.Damage`, `DynamicVars.Repeat`, `DynamicVars.Block`.  
Use `.IntValue` for counts, `.BaseValue` for decimals.

## OnPlay Patterns

Always guard single-target plays with `ArgumentNullException.ThrowIfNull(cardPlay.Target)`.

**Basic attack:**
```csharp
await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt").Execute(choiceContext);
```

**Multi-hit attack:**
```csharp
await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue, "vfx/vfx_attack_blunt").Execute(choiceContext);
```

**Conditional hit count:**
```csharp
var hitCount = cardPlay.Target.HasPower<MyPower>()
    ? DynamicVars["BonusRepeat"].IntValue
    : DynamicVars.Repeat.IntValue;
await CommonActions.CardAttack(this, cardPlay, hitCount, "vfx/vfx_attack_blunt").Execute(choiceContext);
```

**All enemies (attack):**
```csharp
foreach (var enemy in CombatState.HittableEnemies)
    await CommonActions.CardAttack(this, cardPlay with { Target = enemy }, vfx: "vfx/vfx_attack_slash").Execute(choiceContext);
```

**All enemies (apply power):**
```csharp
foreach (var enemy in CombatState.HittableEnemies)
    await CommonActions.Apply<MyPower>(choiceContext, enemy, this, DynamicVars["Amount"].BaseValue);
```

**Apply power to enemy / self:**
```csharp
await CommonActions.Apply<MyPower>(choiceContext, cardPlay.Target, this, 1m);
await CommonActions.ApplySelf<MyPower>(choiceContext, this, DynamicVars["Amount"].BaseValue);
```

**Apply instanced power then call a typed method on it:**
```csharp
var power = await CommonActions.ApplySelf<MyInstancedPower>(choiceContext, this, DynamicVars["Amount"].BaseValue);
power?.MyTypedMethod(DynamicVars["SomeKey"].BaseValue);
```

**Remove enemy block:**
```csharp
await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);
```

**Block equal to damage dealt:**
```csharp
public override bool GainsBlock => true;  // declare this property
// then in OnPlay:
var result = await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt").Execute(choiceContext);
var blockAmount = result.Results.Sum(r => r.TotalDamage + r.OverkillDamage);
await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Move, cardPlay);
```

**Gain fixed block:**
```csharp
await CommonActions.CardBlock(this, cardPlay);
```

**Gain energy:**
```csharp
await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
```

**Remove a power from enemy:**
```csharp
await PowerCmd.Remove<MyPower>(cardPlay.Target);
```

**Exhaust a random card from hand (and deal damage if one was exhausted):**
```csharp
var pile = PileType.Hand.GetPile(Owner);
var card = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
if (card is not null)
{
    await CardCmd.Exhaust(choiceContext, card);
    await CommonActions.CardAttack(this, cardPlay, vfx: "vfx/vfx_attack_blunt").Execute(choiceContext);
}
```

**Add generated cards to combat (e.g. create token cards into hand):**
```csharp
var cards = MyActions.CreateMyTokenCards(Owner, isUpgraded: IsUpgraded);
await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
```

**Select a card from a pile:**
```csharp
var card = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, choiceContext, PileType.Draw);
if (card is not null)
    await CardPileCmd.Add(card, PileType.Hand);
```

**Select a card from hand (e.g. to transform it):**
```csharp
var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
var card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
if (card is null) return;
```

**Transform a card into another:**
```csharp
var newCard = /* create the replacement card */;
await CardCmd.Transform(cardToTransform, newCard);
```

**Equation-based damage (scales with deck size):**
```csharp
protected override IEnumerable<DynamicVar> CanonicalVars =>
[
    new CalculationBaseVar(0M),
    new ExtraDamageVar(1M),
    new CalculatedDamageVar(ValueProp.Move)
        .WithMultiplier((card, _) => PileType.Deck.GetPile(card.Owner).Cards.Count)
];
```

**Equation-based damage (scales with cards played this combat, with tag-weighted upgrade):**
```csharp
new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CalculateDamageMultiplier)

private static decimal CalculateDamageMultiplier(CardModel card, Creature? _)
{
    return CombatManager.Instance.History.CardPlaysFinished.Aggregate(0M, (sum, entry) =>
    {
        if (entry.CardPlay.Card.Owner != card.Owner) return sum;
        if (!card.IsUpgraded) return sum + 1;
        var increase = entry.CardPlay.Card.Tags.Contains(WapoMetalTags.WapoMetal)
            ? card.DynamicVars["MyMultiplierKey"].BaseValue
            : 1;
        return sum + increase;
    });
}
```

**Energy-X card:**
```csharp
protected override bool HasEnergyCostX => true;
// in OnPlay, use ResolveEnergyXValue() as hit count or multiplier
await CommonActions.CardAttack(this, cardPlay, ResolveEnergyXValue(), "vfx/vfx_attack_slash").Execute(choiceContext);
```

## Keywords & Hover Tips

Valid keywords include: `CardKeyword.Exhaust`, `CardKeyword.Retain`.

```csharp
public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MyPower>()];
// Static hover tip for a built-in game concept (e.g. Block, Strength):
protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];
// Conditional hover tips:
protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded
    ? [HoverTipFactory.FromPower<PowerA>(), HoverTipFactory.FromPower<PowerB>()]
    : [HoverTipFactory.FromPower<PowerA>()];
```

`HoverTipFactory.Static` requires `using MegaCrit.Sts2.Core.HoverTips;`.

## Glow Gold

Glows when the condition is meaningful to play:
```csharp
protected override bool ShouldGlowGoldInternal =>
    CombatState != null && CombatState.HittableEnemies.Any(e => e.HasPower<MyPower>());
```

## Upgrade Patterns

| Goal | Code |
|------|------|
| Increase damage | `DynamicVars.Damage.UpgradeValueBy(2M)` |
| Increase repeat | `DynamicVars.Repeat.UpgradeValueBy(1M)` |
| Increase a custom var | `DynamicVars["MyKey"].UpgradeValueBy(1M)` |
| Reduce energy cost | `EnergyCost.UpgradeBy(-1)` |
| Remove keyword | `RemoveKeyword(CardKeyword.Exhaust)` |
| If-upgraded branch in OnPlay | `if (IsUpgraded) { ... }` |

## Return to Hand Next Turn

Override `BeforeHandDraw` and check combat history:
```csharp
public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
{
    if (player != Owner || Pile?.Type == PileType.Hand) return;
    if (CombatManager.Instance.History.CardPlaysFinished.Any(e =>
            e.CardPlay.Card == this && e.RoundNumber == combatState.RoundNumber - 1))
        await CardPileCmd.Add(this, PileType.Hand);
}
```

## Card Pool Sub-sets (e.g. WapoMetal)

Sub-set cards use a child pool (`WapoMetalCardPool`) in a subfolder and are typically `Token` rarity. They are generated into the player's hand during combat by a parent card or utility method (see `MunchMunchActions`).

WapoMetal cards inherit the `WapoMetalCard` abstract base instead of `CustomCard` directly — it provides `[Pool(typeof(WapoMetalCardPool))]`, `CardRarity.Token`, and the `WapoMetal` tag automatically:
```csharp
public class WapoMetalAxe() : WapoMetalCard(1, CardType.Attack, TargetType.AnyEnemy) { ... }
```

## Card Tags

Custom tags are declared with `[CustomEnum]` and referenced in `CanonicalTags`:
```csharp
// Definition (in a static utility class):
[CustomEnum("WapoMetal")]
public static CardTag WapoMetal;

// Usage on a card:
protected override HashSet<CardTag> CanonicalTags => [WapoMetalTags.WapoMetal];

// Checking in multipliers / conditions:
entry.CardPlay.Card.Tags.Contains(WapoMetalTags.WapoMetal)
```

## Namespace Convention

`RayzorBladeOnePiece.Cards.<FruitName>` (e.g. `RayzorBladeOnePiece.Cards.SlowSlowFruit`).  
Sub-sets: `RayzorBladeOnePiece.Cards.<FruitName>.<SubSet>`.

## Do Not

- Hard-code numeric values in `OnPlay` — always use `DynamicVars`
- Create `.import` files — Godot generates these automatically
- Invent VFX path strings — known valid paths: `"vfx/vfx_attack_blunt"`, `"vfx/vfx_attack_slash"`, `"vfx/vfx_dramatic_stab"`
