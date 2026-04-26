---
description: "Scaffold a new Devil Fruit relic: .cs file + German localization entry + placeholder icons"
argument-hint: "Relic name, associated card pool, rarity, starter card (if any), effect description"
agent: "agent"
---

Create a new relic for the RayzorBlade OnePiece mod. Follow [copilot-instructions.md](../copilot-instructions.md) and [relics.instructions.md](../instructions/relics.instructions.md) strictly.

## Inputs (fill in from the user's message)

| Field | Description | Valid values |
|-------|-------------|--------------|
| `RelicName` | PascalCase class name | e.g. `GomuGomuFruit` |
| `CardPool` | Card pool class the relic unlocks | e.g. `GomuGomuFruitCardPool` |
| `Rarity` | Relic rarity | `Starter` / `Common` / `Uncommon` / `Rare` / `Shop` / `Boss` / `Special` |
| `StarterCard` | Card granted on pickup (optional) | e.g. `RubberPunch`, or _none_ |
| `Effect` | Natural-language description of any extra effect beyond the card pool unlock | |

## Tasks

1. **Read context** — read [SlowSlowFruit.cs](../../Relics/SlowSlowFruit.cs) and [DevilFruitRelic.cs](../../Relics/DevilFruitRelic.cs) as style references. If the associated `CardPool` already exists, read it too.

2. **Create `Relics/<RelicName>.cs`**
   - For a Devil Fruit relic, inherit `DevilFruitRelic<TCardPool>` (do **not** add `[Pool(typeof(SharedRelicPool))]` — it is inherited).
   - Set `Rarity` from the inputs.
   - If a starter card was specified, override `PickUpCardRewards`:
     ```csharp
     protected override IEnumerable<CardModel> PickUpCardRewards =>
         [Owner.RunState.CreateCard<StarterCard>(Owner)];
     ```
   - Add `ExtraHoverTips` for the starter card and any other referenced cards/powers.
   - **Do not** create `.import` files.

3. **Add localization** — add the new key triple to `RayzorBladeOnePiece/localization/deu/relics.json`:
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.description"` — German description; highlight card/keyword names with `[gold]name[/gold]`
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.flavor"` — German flavour text
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.title"` — German display name
   - **Add the new keys in ascending order into the already sorted file.**

4. **Copy placeholder icons** — derive `<id>` from the relic class name (PascalCase → screaming_snake → lowercase, e.g. `SlowSlowFruit` → `slow_slow_fruit`). Run these three commands:
   ```powershell
   Copy-Item "RayzorBladeOnePiece/images/relics/placeholder.png" "RayzorBladeOnePiece/images/relics/<id>.png"
   Copy-Item "RayzorBladeOnePiece/images/relics/placeholder_outline.png" "RayzorBladeOnePiece/images/relics/<id>_outline.png"
   Copy-Item "RayzorBladeOnePiece/images/relics/big/placeholder.png" "RayzorBladeOnePiece/images/relics/big/<id>.png"
   ```
   **Do not** create `.import` files — Godot generates them automatically.

5. **Update [relics.instructions.md](../instructions/relics.instructions.md)** — after writing the relic, review what you implemented and add any patterns that are **not already documented** there. Only add genuinely new knowledge — do not duplicate what exists. If nothing new was learned, skip this step silently.

6. **Report** — after all files are written, show:
   - File paths created/modified
   - The generated relic ID (e.g. `RAYZORBLADEONEPIECE-GOMU_GOMU_FRUIT`)
   - A reminder to replace the placeholder icons with real artwork
   - A reminder to create the card pool if it didn't exist yet
   - Any additions made to `relics.instructions.md` (or "instructions unchanged" if nothing was added)
