---
description: "Scaffold a new Devil Fruit card: .cs file + German localization entry"
argument-hint: "Card name, fruit, type (Attack/Skill/Power), rarity, cost, target, effect description"
agent: "agent"
---

Create a new card for the RayzorBlade OnePiece mod. Follow [copilot-instructions.md](../copilot-instructions.md) strictly.

## Inputs (fill in from the user's message)

| Field | Description | Valid values |
|-------|-------------|--------------|
| `CardName` | PascalCase class name | e.g. `RubberPunch` |
| `FruitCardPool` | Card pool class | e.g. `GomuGomuFruitCardPool` |
| `CardType` | `Attack` / `Skill` / `Power` / `Curse` | |
| `Rarity` | `Common` / `Uncommon` / `Rare` | |
| `TargetType` | `AnyEnemy` / `AllEnemies` / `Self` / `None` | |
| `BaseCost` | Integer energy cost, or `X` for energy-X cards | |
| `Effect` | Natural-language description of what the card does | |
| `PoolTitle` | *(only if pool doesn't exist)* Display title for the card pool | e.g. `"Gomu Gomu Fruit"` |
| `DeckEntryCardColor` | *(only if pool doesn't exist)* Hex color for deck entry | e.g. `"840240"` |
| `EnergyOutlineColor` | *(only if pool doesn't exist)* Hex color for energy outline | e.g. `"651565"` |

## Tasks

1. **Read context** — check whether `Cards/<FruitFolder>/<FruitCardPool>.cs` exists. If it does, read it for reference. Read 1–2 similar existing cards in the same fruit's folder as style references.

2. **Create card pool if missing** — if `Cards/<FruitFolder>/<FruitCardPool>.cs` does not exist:
   - Derive `<FruitFolder>` from the pool class name (e.g. `GomuGomuFruitCardPool` → `GomuGomuFruit`)
   - Create `Cards/<FruitFolder>/<FruitCardPool>.cs` inheriting `DevilFruitCardPool`:
     ```csharp
     public class GomuGomuFruitCardPool : DevilFruitCardPool
     {
         public override string Title => "<PoolTitle>";
         public override Color DeckEntryCardColor => new("<DeckEntryCardColor>");
         public override Color EnergyOutlineColor  => new("<EnergyOutlineColor>");
     }
     ```
   - If `PoolTitle`, `DeckEntryCardColor`, or `EnergyOutlineColor` were not provided, ask the user for them before proceeding.

3. **Create `Cards/<FruitFolder>/<CardName>.cs`** — the `<FruitFolder>` is the same directory created/used in step 2.
   - Inherit `CustomCard`, decorated with `[Pool(typeof(<FruitCardPool>))]`
   - Declare `CanonicalVars` for all dynamic values (damage, block, repeat, etc.)
   - Implement `OnPlay` — use `CommonActions`, `CreatureCmd`, `PowerCmd` patterns from existing cards
   - If the card has an upgrade, implement `OnUpgrade`
   - If the card targets all enemies, loop `CombatState.HittableEnemies`
   - If cost is X, set `protected override bool HasEnergyCostX => true` and use `ResolveEnergyXValue()`
   - If a condition should cause the card to glow gold, implement `ShouldGlowGoldInternal`
   - Add `ExtraHoverTips` for any powers the card references
   - **Do not** create `.import` files

4. **Add localization** — add the new key pair to `RayzorBladeOnePiece/localization/deu/cards.json`:
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.title"` — German card name
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.description"` — German description using `{VarName:diff()}` for upgradeable vars, `[gold]keyword[/gold]`, `[blue]value[/blue]`, `{IfUpgraded:show:text}`
   - **Add the new keys in ascending order into the already sorted file**

5. **Copy placeholder portraits** — derive `<id>` from the card class name (PascalCase → screaming_snake → lowercase, e.g. `WapoMetalBroadsword` → `wapo_metal_broadsword`). Run these two commands:
   ```powershell
   Copy-Item "RayzorBladeOnePiece/images/card_portraits/placeholder.png" "RayzorBladeOnePiece/images/card_portraits/<id>.png"
   Copy-Item "RayzorBladeOnePiece/images/card_portraits/big/placeholder.png" "RayzorBladeOnePiece/images/card_portraits/big/<id>.png"
   ```
   **Do not** create `.import` files — Godot generates them automatically.

6. **Update [cards.instructions.md](../instructions/cards.instructions.md)** — after writing the card, review what you implemented and add any patterns that are **not already documented** there. Only add genuinely new knowledge — do not duplicate what exists. Examples of things worth adding:
   - A new `OnPlay` pattern or API call not yet listed
   - A new `DynamicVar` type or usage variant
   - A new upgrade technique
   - A new combo of existing APIs that is non-obvious
   If nothing new was learned, skip this step silently.

7. **Report** — after all files are written, show:
   - File paths created/modified
   - The generated card ID (e.g. `RAYZORBLADEONEPIECE-RUBBER_PUNCH`)
   - A reminder to replace the placeholder portraits with real artwork
   - Whether the card pool was newly created or already existed
   - Any additions made to `cards.instructions.md` (or "instructions unchanged" if nothing was added)
