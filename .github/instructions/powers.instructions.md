---
description: "Use when creating, editing, or reviewing power classes in the Powers/ directory. Covers the full power API: Type/StackType, DynamicVar usage, override hooks, helper methods, hover tips, localization patterns, and health bar forecasts."
applyTo: "Powers/**"
---

# Power Authoring

All powers inherit `CustomPower` and live in `Powers/`. The namespace is `RayzorBladeOnePiece.Powers`.

```csharp
public class MyPower : CustomPower
{
    public override PowerType Type => PowerType.Buff;       // Buff | Debuff
    public override PowerStackType StackType => PowerStackType.Counter; // Counter | Single
}
```

Auto-discovery registers powers — no manual entry required.

### Instanced Powers

Add `public override bool IsInstanced => true;` when each application of the power must carry its own independent state (e.g. a separate damage value per stack). Without this flag all stacks share a single instance.

```csharp
public class MyPower : CustomPower
{
    public override bool IsInstanced => true;
    // ...
}
```

## DynamicVar Types

Declare all numeric values in `CanonicalVars`. The `Amount` property is a shorthand for the current stack count of `Counter` powers.

| Type | Usage |
|------|-------|
| `new("MyKey", 2M)` | Custom named var — access via `DynamicVars["MyKey"].BaseValue` |
| `new DamageVar(10M, ValueProp.Unpowered)` | Damage value scaled to combat stats |
| `new BoolVar("MyKey", false)` | Boolean var — access via `((BoolVar)DynamicVars["MyKey"]).BoolVal` |
| `Amount` | Shorthand for current stack count (only valid on `Counter` powers) |

Access: `DynamicVars["MyKey"].BaseValue` (decimal), `DynamicVars["MyKey"].IntValue` (integer).

`BaseValue` is settable — use a public method to inject a value at apply-time when the power is `IsInstanced`:

```csharp
public void SetDamage(decimal damage) => DynamicVars.Damage.BaseValue = damage;
```

## Override Hooks

| Hook | Signature | Typical use |
|------|-----------|-------------|
| `ModifyDamageMultiplicative` | `(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource) → decimal` | Return a multiplier (e.g. `0.75M` to reduce 25 %). Return `1M` to pass through. |
| `ModifyDamageAdditive` | Same signature → `decimal` | Return an additive modifier (negative to reduce). Return `0M` to pass through. |
| `ModifyHpLostAfterOstyLate` | Same signature → `decimal` | Modify final HP lost after all defenses. Return `0M` to cancel damage and redirect it. |
| `BeforeDamageReceived` | `async Task(PlayerChoiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)` | React before damage hits (e.g. apply counter-power to dealer). |
| `BeforeDamageDealt` | `async Task(PlayerChoiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)` | React before damage is dealt from the owner. |
| `BeforeTurnEnd` | `async Task(PlayerChoiceContext, CombatSide side)` | Trigger before a turn ends. Compare `side` to `Owner.Side` to target the right turn. |
| `AfterTurnEnd` | `async Task(PlayerChoiceContext, CombatSide side)` | Trigger at end of a turn. Compare `side` to `Owner.Side` to target the right turn. |
| `AfterPlayerTurnStart` | `async Task(PlayerChoiceContext, Player player)` | Trigger at the start of the player's turn. Guard with `if (player != Owner.Player) return;`. |
| `BeforeHandDraw` | `async Task(Player player, PlayerChoiceContext, CombatState)` | Trigger before the hand is drawn each turn. |
| `GetHealthBarForecastSegments` | `IEnumerable<HealthBarForecastSegment>(HealthBarForecastContext)` | Add colored forecast segments to the creature's health bar. |

### AfterTurnEnd — Side Check Patterns

```csharp
// Trigger on ENEMY turn end (typical for debuffs placed on enemies):
if (side == Owner.Side) return;

// Trigger on OWNER turn end (typical for buffs):
if (side != Owner.Side) return;

// Also guard against dead/missing combat state:
if (Owner.Player is null || Owner.CombatState is null) return;
```

### ModifyDamageMultiplicative — Guard Pattern

```csharp
public override decimal ModifyDamageMultiplicative(
    Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    // Only modify when the owner is the dealer and it is a powered attack:
    return dealer != Owner || !props.IsPoweredAttack_() ? 1M : DynamicVars["Factor"].BaseValue;
}
```

### ModifyDamageAdditive — Guard Pattern

```csharp
public override decimal ModifyDamageAdditive(
    Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    return Owner != dealer || !props.IsPoweredAttack_() ? 0M : Amount * -1;
}
```

### BeforeDamageReceived — Apply Counter-Power

```csharp
public override async Task BeforeDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (target != Owner || dealer == null || !props.IsPoweredAttack_()) return;

    Flash();
    await PowerCmd.Apply<CounterPower>(dealer, Amount, Owner, null);
}
```

### ModifyHpLostAfterOstyLate — Redirect Damage

```csharp
public override decimal ModifyHpLostAfterOstyLate(
    Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (target != Owner || cardSource is null || amount <= 0M) return amount;

    // Redirect to another power instead of dealing HP damage:
    PowerCmd.Apply<StoragePower>(target, amount * DynamicVars["Multiplier"].BaseValue, Applier, cardSource);
    return 0M;
}
```

### BeforeTurnEnd — Countdown and Detonate

Use `BeforeTurnEnd` when an effect should fire **before** the turn ends (e.g. a bomb that counts down). The same `side` guard rules apply as for `AfterTurnEnd`.

```csharp
public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
{
    if (side != Owner.Side) return;

    if (Amount > 1)
    {
        await PowerCmd.Decrement(this); // self-decrement (awaitable)
        return;
    }

    Flash();
    await Cmd.CustomScaledWait(0.2f, 0.4f);
    // ... detonate
    await PowerCmd.Remove(this);
}
```

### AfterTurnEnd — Deal Damage and Remove

```csharp
public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
{
    if (side == Owner.Side || Owner.Player is null || Owner.CombatState is null) return;

    Flash();
    await Cmd.CustomScaledWait(0.2f, 0.4f);

    for (var i = 0; i < Amount; i++)
    {
        var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
        if (target is null) continue;
        await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage, Owner);
    }

    await PowerCmd.Remove(this);
}
```

### GetHealthBarForecastSegments

```csharp
public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
{
    return [new HealthBarForecastSegment(Amount, new Color("c466be"), HealthBarForecastDirection.FromRight)];
}
```

### AfterPlayerTurnStart — Select and Transform a Card

```csharp
public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
{
    if (player != Owner.Player) return;

    var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
    var card = (await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, null, this))
        .FirstOrDefault();
    if (card is null) return;

    var replacement = Owner.Player.RunState.CreateCard<MyCard>(Owner.Player);
    await CardCmd.Transform(card, replacement);
}
```

## Helper Methods & Commands

| Expression | Description |
|-----------|-------------|
| `Flash()` | Flash the power icon (call before async effects) |
| `Amount` | Current stack count (decimal) |
| `Applier` | The entity that applied this power (use as source for sub-effects) |
| `Owner` | The creature this power is attached to |
| `Owner.GetPower<T>()` | Get another power from the owner (returns null if absent) |
| `Owner.HasPower<T>()` | Check whether the owner has a power |
| `await PowerCmd.Apply<T>(target, amount, applier, cardSource)` | Apply a power asynchronously |
| `PowerCmd.Apply<T>(target, amount, applier, cardSource)` | Apply a power fire-and-forget (inside synchronous hooks) |
| `await PowerCmd.Remove(this)` | Remove this power |
| `PowerCmd.Decrement(power)` | Decrement another power's stack count (fire-and-forget) |
| `await PowerCmd.Decrement(this)` | Self-decrement (awaitable; use inside async hooks) |
| `await CreatureCmd.Damage(choiceContext, target, DynamicVars.Damage, Owner)` | Deal damage from a power |
| `await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, DynamicVars.Damage, Owner)` | Deal damage to all hittable enemies at once (pass collection instead of single target) |
| `await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Unblockable \| ValueProp.Unpowered, Applier, null)` | Deal unblockable unpowered damage |
| `await Cmd.CustomScaledWait(0.2f, 0.4f)` | Wait a short time (for VFX pacing) |
| `await CardSelectCmd.FromHand(choiceContext, player, prefs, null, this)` | Prompt the player to select cards from their hand |
| `await CardCmd.Transform(card, replacement)` | Replace a card in-place with another card |

## Hover Tips

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RelatedPower>()];
```

## Wrapping a BaseLib Power

When a power must extend an existing BaseLib power (e.g. `TemporaryStrengthPower`) instead of `CustomPower`, implement `ICustomPower` and `ICustomModel` manually:

```csharp
public class MyWrappedPower : TemporaryStrengthPower, ICustomPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<MySourceCard>();
    protected override bool IsPositive => false;

    public string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToPowerImagePath();
    public string CustomBigIconPath   => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ToBigPowerImagePath();
}
```

## Localization Keys

Add entries to `RayzorBladeOnePiece/localization/deu/powers.json` in ascending key order:

| Key | Purpose |
|-----|---------|
| `RAYZORBLADEONEPIECE-<ID>.title` | Display name |
| `RAYZORBLADEONEPIECE-<ID>.description` | Static description (no variables) |
| `RAYZORBLADEONEPIECE-<ID>.smartDescription` | Dynamic description shown in combat |

### Description Variable Tags

| Tag | Output |
|-----|--------|
| `{Amount}` | Current stack count |
| `{Amount:diff()}` | Stack count with upgrade diff highlight |
| `{Amount:abs()}` | Absolute value of stack count |
| `{VarName:diff()}` | Named var value with upgrade diff |
| `[gold]keyword[/gold]` | Gold keyword highlight |
| `[blue]value[/blue]` | Blue value highlight |
| `{OnPlayer:player_text\|enemy_text}` | Different text depending on whether the power is on the player |

### Example

```json
"RAYZORBLADEONEPIECE-MY_POWER.description": "Short static description.",
"RAYZORBLADEONEPIECE-MY_POWER.smartDescription": "{OnPlayer:Erhältst|Erhält} [blue]{Amount}[/blue] [gold]Keyword[/gold].",
"RAYZORBLADEONEPIECE-MY_POWER.title": "Mein Effekt",
```

## Power ID Derivation

Power class name → localization/image ID:  
`MyCustomPower` → screaming snake → `MY_CUSTOM_POWER` → lowercase → `my_custom_power`

Image files: `RayzorBladeOnePiece/images/powers/{id}.png` (small) and `powers/big/{id}.png` (big).

## Do Not

- Hard-code numeric values — always declare in `CanonicalVars` and read via `DynamicVars`
- Create `.import` files — Godot generates them automatically
- Forget the `side` guard in `AfterTurnEnd` — without it the hook fires on both turns
