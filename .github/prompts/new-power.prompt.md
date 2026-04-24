---
description: "Scaffold a new power: .cs file + German localization entry + placeholder icons"
argument-hint: "Power name, power type (Buff/Debuff), stack type (Counter/Single), effect description"
agent: "agent"
---

Create a new power for the RayzorBlade OnePiece mod. Follow [copilot-instructions.md](../copilot-instructions.md) and [powers.instructions.md](../instructions/powers.instructions.md) strictly.

## Inputs (fill in from the user's message)

| Field | Description | Valid values |
|-------|-------------|--------------|
| `PowerName` | PascalCase class name — must end in `Power` | e.g. `FrostBitePower` |
| `PowerType` | Whether the power benefits or harms its owner | `Buff` / `Debuff` |
| `StackType` | Whether the power accumulates stacks | `Counter` / `Single` |
| `Effect` | Natural-language description of what the power does | |

## Tasks

1. **Read context** — read 1–2 existing powers in `Powers/` that have a similar effect (damage modifier, turn-end trigger, received-damage hook, etc.) as style references.

2. **Create `Powers/<PowerName>.cs`**
   - Inherit `CustomPower`, namespace `RayzorBladeOnePiece.Powers`
   - Set `PowerType` and `PowerStackType` from the inputs
   - Declare `CanonicalVars` for every numeric value; never hard-code numbers in hook bodies
   - Implement the appropriate override hooks (see [powers.instructions.md](../instructions/powers.instructions.md)):
     - Damage modifier → `ModifyDamageMultiplicative` or `ModifyDamageAdditive`
     - Intercept incoming damage → `ModifyHpLostAfterOstyLate` or `BeforeDamageReceived`
     - Trigger on turn end → `AfterTurnEnd` (include the `side` guard)
     - Draw phase → `BeforeHandDraw`
     - Health bar preview → `GetHealthBarForecastSegments`
   - Add `ExtraHoverTips` if the power references other powers
   - Call `Flash()` before any async visual effects
   - **Do not** create `.import` files

3. **Add localization** — add the new key triple to `RayzorBladeOnePiece/localization/deu/powers.json`:
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.title"` — German display name
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.description"` — short static description
   - `"RAYZORBLADEONEPIECE-<SCREAMING_SNAKE_ID>.smartDescription"` — dynamic in-combat description using `{Amount}`, `{OnPlayer:...|...}`, `[gold]keyword[/gold]`, `[blue]value[/blue]`
   - **Add the new keys in ascending order into the already sorted file**

4. **Copy placeholder icons** — derive `<id>` from the power class name (PascalCase → screaming_snake → lowercase, e.g. `FrostBitePower` → `frost_bite_power`). Run these two commands:
   ```powershell
   Copy-Item "RayzorBladeOnePiece/images/powers/placeholder.png" "RayzorBladeOnePiece/images/powers/<id>.png"
   Copy-Item "RayzorBladeOnePiece/images/powers/big/placeholder.png" "RayzorBladeOnePiece/images/powers/big/<id>.png"
   ```
   **Do not** create `.import` files — Godot generates them automatically.

5. **Update [powers.instructions.md](../instructions/powers.instructions.md)** — after writing the power, review what you implemented and add any patterns that are **not already documented** there. Only add genuinely new knowledge — do not duplicate what exists. Examples of things worth adding:
   - A new hook variant or usage pattern not yet listed
   - A new `DynamicVar` type or usage not yet shown
   - A non-obvious combination of commands
   If nothing new was learned, skip this step silently.

6. **Report** — after all files are written, show:
   - File paths created/modified
   - The generated power ID (e.g. `RAYZORBLADEONEPIECE-FROST_BITE_POWER`)
   - A reminder to replace the placeholder icons with real artwork
   - Any additions made to `powers.instructions.md` (or "instructions unchanged" if nothing was added)
