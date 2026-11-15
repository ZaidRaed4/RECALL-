# Translocator Platformer — Roadmap

A 2D puzzle platformer (Godot 4.7, C#) built around one special mechanic:
**drop a translocator anywhere, then press a key to warp back to it.**
You use it to make one-way trips into round trips, get through gaps you can't fit,
hold switches down and beat timed doors.

---

## Progress

| Part | Name | Status |
|---|---|---|
| 0 | Project setup | ✅ Done |
| 1 | Player movement | ✅ Done |
| 2 | Platforming feel + camera | ⏭️ Skipped |
| 3 | ⭐ Translocator v1: drop & recall | ✅ Done |
| 4 | Translocator rules & feedback | ✅ Done |
| 5 | Tile levels, hazards & respawn | 🔨 In progress |
| 6 | Puzzle pieces | ⬜ Not started |
| 7 | Level design | ⬜ Not started |
| 8 | UI & game flow | ⬜ Not started |
| 9 | Art, animation, audio, juice | ⬜ Not started |
| 10 | Ship it | ⬜ Not started |

Status key: ⬜ Not started · 🔨 In progress · ✅ Done · ⏭️ Skipped

> **▶ Resume here (updated 2026-11-15):** Part 5, tile levels, hazards & respawn. Order: tilemapping lessons (Craftpix dungeon tiles) → camera → checkpoints. Discuss hazards/death when we get there.
> Level rules (16 px tiles in game): the jump reaches ~46 px, so ledges are at most **2 tiles** (32 px) high. The wizard's hitbox is 12×26, so gaps and headroom need at least **2 tiles** (32 px).
> Wizard sprite ✅ done early. Tilemapping lesson 1 ✅ (36 rock tiles in `levels/dungeon_tileset.tres`, `Ground` at 0.25 scale, collision reshaped on tile1 + tile2). The other flat-top/ledge tiles (3, 25–34) are still full squares: reshape each one the first time it is used. Background ✅ (`Background` Sprite2D, Pale cave image repeated to 960×270) and camera ✅ (limits 0,0 → 960,270, set in test_room). `Lava` TileMapLayer ✅ (14 lava tiles in the TileSet, **no collision yet**; rock is painted under every pool so you can't fall out; lava gets hazard collision in the hazards lesson). Lava glow ✅ (the pack's lava tiles are single frames, so instead of tile animation an `AnimationPlayer` child of `Lava` loops its `modulate` white → `c08080` → white over 1.2 s, autoplay). tile14 collision reshaped too.
> Spell ✅ (added at the user's request, not in the original plan): `cast` (F, L) plays the wizard's `cast` animation (Attack2, 8 frames, 15 FPS, loop off), and on frame 5 (`_sprite.Frame == 4`, via `FrameChanged`) `spell/spell.tscn` spawns: an Area2D with the purple crystal item at 0.25 scale, mask = world, which flies at 200 px/s and frees itself on hitting a wall or after 1.5 s. `_isCasting` locks `UpdateAnimation` until `AnimationFinished`. The `hit` animation (Hit.png, 4 frames, 10 FPS, loop off) is already in SpriteFrames but not used yet.
> **Next (user chose this): 3 hearts + the `hit` animation.** Proposed: touching lava costs a heart and plays `hit`, then a short blink of invulnerability so you don't lose all 3 at once; at 0 hearts, respawn. Confirm the damage source with the user first (lava is the only candidate so far), and clear `_isCasting` when hit. Then finish tilemapping lesson 2 (walls, ledges, a 2-tile-high gap) and checkpoints.
> Part 4 is done (safe recall, cooldown, blink, HUD label). KeepMomentum and walk-over pickup were cut to keep it simple for trainees.
> Note: `PlatformLow` is 45 px above the floor and the current jump reaches about 46 px. If it feels too tight, raise `_jumpSpeed` a little.
> *(Optional, any time)* Add the backup keys: ←/→/↓ arrows, W and ↑ for jump, J for drop, K for recall.
>
> Repo: https://github.com/ZaidRaed4/RECALL- (branch `main`)

**How every part works:**
1. Go over the concepts the part teaches.
2. Build it: you set up the nodes in the editor, and we write the script together.
3. Playtest it against the **Done when** list.
4. Commit it as `Part N: <name>`.
5. Tick the boxes here and update the table.

---

## Controls

| Action | Keys | Input action name |
|---|---|---|
| Move | A / D, ← / → | `move_left`, `move_right` |
| Drop through platform | S, ↓ | `move_down` |
| Jump | Space, W, ↑ | `jump` |
| Drop translocator | Q, J | `translocator_drop` |
| Recall to translocator | E, K | `translocator_recall` |
| Cast spell | F, L | `cast` |
| Restart room | R | `restart` |
| Pause | Esc | `pause` |

---

## Translocator design rules (v1 defaults; we'll change them as we playtest)

| Rule | Default | Why |
|---|---|---|
| How many | One at a time | Easy to read and plan around |
| Dropping again | Moves it to where you are now | No separate "pick up" key needed |
| Physics | Falls with gravity, lands and stays put | Can fall through grates and press plates |
| Recall | Teleports you to it, and it **stays** | A respawn point you control |
| Velocity after recall | Reset to zero | Predictable |
| On death | Respawn at checkpoint; translocator removed | Hazards still matter |
| Recall blocked when | It's in a no-translocate field, or there's no room to stand | Gives puzzles constraints |

**Open questions** (we'll decide these by playtesting):
- Should recalling use up the translocator?
- Limited charges per level?
- Stretch goal: *throw* it instead of only dropping it.

**Puzzle patterns the mechanic enables:**
- **Leave it behind:** drop it at the top, jump into a pit you can't climb out of, grab the gem, recall.
- **Through the grate:** grates block the player but not the translocator. Drop it through, then recall into the sealed room.
- **Hold the plate:** the translocator is heavy enough to hold a pressure plate down.
- **Race the door:** leave it by a timed door, run and pull the lever, then recall before the door shuts.

---

## Project structure

```
res://
├── assets/        imported art, tiles, audio, fonts (no scripts in here)
├── globals/       autoload singletons: GameState.cs
├── player/        player.tscn, Player.cs
├── translocator/  translocator.tscn, Translocator.cs   <- the special mechanic
├── spell/        spell.tscn, Spell.cs (the wizard's bolt)
├── objects/       level pieces: door, pressure plate, lever, grate, no-translocate
│                  field, checkpoint, key, gem, exit
├── hazards/       spikes, kill zone
├── levels/        test_room.tscn, level_01.tscn ..., tileset .tres
├── ui/            hud, main menu, pause menu, level complete
└── ROADMAP.md     this file
```

**Conventions** (the same ones as the Zombies project):
- **Keep it simple.** Trainees will be walked through this project, so build the plain version that works and skip extra or clever features.
- Scenes: `snake_case.tscn`. Scripts: `PascalCase.cs`, and the class name matches the file name.
- Each feature keeps its scene and script in the same folder.
- Private fields: `_camelCase`. Anything we want to tune goes in an `[Export]` field, not a hard-coded number.
- Use `[Signal]` and C# events so objects don't need to know about each other.
  A plate doesn't know what a door is; it just says "pressed".
- Node names in the scene tree are `PascalCase`.

**Physics layers:**

| # | Name | What's on it |
|---|---|---|
| 1 | world | ground, walls, platforms, doors |
| 2 | player | the player |
| 3 | translocator | the dropped translocator |
| 4 | hazards | spikes, kill zones |
| 5 | interactables | plates, levers, pickups, checkpoints, exit |
| 6 | grates | blocks the player only (the translocator's mask leaves this out) |

---

## Phase 1: Foundation

### Part 0: Project setup
**Goal:** a project that's configured correctly, so we never have to fight the settings later.
**Concepts:** Project Settings, Input Map (actions vs. keys), collision *layers* vs. *masks*, pixel-art rendering, folder organization.
- [x] Folder structure
- [x] ROADMAP.md
- [x] Display: 480×270 base resolution, 1440×810 window, integer scaling, `canvas_items` stretch
- [x] Nearest-neighbour texture filter (crisp pixels)
- [x] Input actions: main keys done (backup keys still to add)
- [x] Physics layer names (2D Physics, layers 1–6)
- [x] C# solution (`.csproj` + `.sln`)
- [x] TestRoom with a floor, set as main scene, runs with F5
- [x] git init + first commit, pushed to GitHub

**Done when:** the project opens with no errors and every setting shows up in Project Settings.

### Part 1: Player movement
**Goal:** a placeholder player that runs and jumps around a greybox room.
**Concepts:** `CharacterBody2D`, `_PhysicsProcess` and `delta`, `Velocity` + `MoveAndSlide()`, gravity as acceleration,
`IsOnFloor()`, `Input.GetAxis` / `IsActionJustPressed`, `MoveToward` for acceleration and friction, `[Export]` tuning.
- [x] `player/player.tscn`: CharacterBody2D → ColorRect (placeholder) + CollisionShape2D, origin at the feet
- [x] `player/Player.cs`: gravity, horizontal acceleration/friction, jump
- [x] `levels/test_room.tscn`: StaticBody2D floors, walls, and platforms at different heights
- [x] Set test_room as the main scene

**Done when:** you can run left and right with a smooth start and stop, jump onto a platform, and you can't walk through walls.

### Part 2: Platforming feel + camera ⏭️ Skipped
**Decision (2026-11-05):** the basic movement from Part 1 is enough for this project. We'll come back to these only if playtesting shows a real problem.
The camera moved to Part 5, because the test room is only one screen and the camera has nothing to follow until then.
- ~~Jump tuned in tiles (`JumpHeight`, `TimeToApex`) instead of magic numbers~~
- ~~Release jump early → shorter hop~~
- ~~Fall gravity multiplier + max fall speed~~
- ~~Coyote time (~0.1s)~~
- ~~Jump buffer (~0.1s)~~
- ~~Camera2D follows the player with smoothing~~ → moved to Part 5

---

## Phase 2: The Translocator

### Part 3: ⭐ Translocator v1: drop & recall
**Goal:** the core mechanic working in the test room.
**Concepts:** `PackedScene` + `Instantiate()`, adding it to the **level** rather than the player (so it stays put when you move),
keeping a reference to it and checking `IsInstanceValid`, `QueueFree()`, `GlobalPosition` vs. `Position`, teleporting a physics body,
giving the translocator its own gravity.
- [x] `translocator/translocator.tscn`: CharacterBody2D + sprite + collision (collides with the world, not the player)
- [x] `translocator/Translocator.cs`: falls, lands, stays
- [x] `translocator_drop` → spawns it at the player's feet, or moves the existing one there
- [x] `translocator_recall` → teleports the player to it and resets velocity

**Done when:** you can drop it on a high ledge, jump down, press recall and be back on the ledge.
Dropping it mid-air makes it fall to the ground. There's never more than one.

### Part 4: Translocator rules & feedback
**Goal:** the mechanic is readable and fair, and it can't break the game.
**Concepts:** a physics query to check for room to stand, a cooldown timer, a `Tween` for a simple blink, a `CanvasLayer` HUD with a `Label`.
- [x] Safe-recall check: never teleport into a wall or ceiling (`HasRoomAt` → `IntersectShape` with the player's box, 1 px inset)
- [x] Short recall cooldown (`_recallCooldown`, 0.4 s)
- [x] Feedback: a quick blink on drop and recall
- [x] HUD label: "No translocator" / "Recall ready" / "Cooling down"
- ~~`[Export] bool KeepMomentum` option~~ ⏭️ cut: too much for trainees
- ~~Pick it back up by walking over it~~ ⏭️ cut: too much for trainees

**Done when:** recall never puts you inside a wall, and you always know where your translocator is and whether recall is ready.

---

## Phase 3: World & Puzzles

### Part 5: Tile levels, hazards & respawn
**Goal:** real levels built from tiles, with danger and checkpoints.
**Concepts:** `TileSet` + `TileMapLayer`, tile physics layers, one-way platforms (and dropping through them),
`Area2D` + `BodyEntered`, groups, an autoload singleton (`GameState`, like in Zombies), respawning in place vs. reloading the scene,
camera limits per level.
- [x] Tileset with collision: Craftpix *Free Dungeon Platformer* pack (free-game-assets.itch.io/free-dungeon-platformer-game-tileset-pixel-art). 64×64 tile images, `Ground` scaled to 0.25 so a tile is 16 px in game. Shared `levels/dungeon_tileset.tres`. Licence: free for games, but the raw art files must not be redistributed.
- [ ] One-way platforms + drop-through with `move_down`
- [ ] Spikes + fall kill zone
- [ ] Checkpoints
- [ ] Death → respawn at checkpoint and remove the translocator
- [x] Camera2D follows the player (moved here from Part 2), with limits set to the level's size (limits set per level via Editable Children)
- [ ] `globals/GameState.cs` autoload: current level, gems collected
- [x] Wizard spell: `cast` animation + crystal bolt (`spell/`)
- [ ] 3 hearts + `hit` animation (heart HUD; the pack has heart item art)

**Done when:** the level is made of tiles, and touching spikes or falling out of the world puts you back at the last checkpoint.

### Part 6: Puzzle pieces
**Goal:** a toolbox of reusable objects that make the translocator *matter*.
**Concepts:** reusable component scenes, linking objects with exported node references and signals connected in the inspector,
a shared `IActivatable` interface (`Activate()` / `Deactivate()`), using collision masks so an object reacts only to the player or only to the translocator,
the `Timer` node, animating doors with `Tween`.
- [ ] Pressure plate: held down by the player **or** the translocator
- [ ] Door: opens and closes when told to
- [ ] Lever / button, with an optional timer for timed doors
- [ ] Grate: blocks the player, the translocator falls through
- [ ] No-translocate field: blocks recall or destroys the translocator inside it
- [ ] Key + locked door
- [ ] Gem collectible
- [ ] Level exit

**Done when:** each piece works on its own in the test room and can be linked to a door without writing new code.

### Part 7: Level design
**Goal:** 5–6 short levels that teach the mechanic one idea at a time (teach → test → twist).
**Concepts:** level-sequence data (a list of scene paths), `ChangeSceneToFile`, the level-complete flow, and how levels can teach without any text.
- [ ] Level 1, *Basics*: run and jump, no translocator yet
- [ ] Level 2, *Leave it behind*: a gem in a pit you can't climb out of
- [ ] Level 3, *Through the grate*: get into a sealed room
- [ ] Level 4, *Hold the plate*: the translocator keeps a door open
- [ ] Level 5, *Race the door*: timed door + lever
- [ ] Level 6, *Finale*: combine everything

**Done when:** a friend can play every level in order without you explaining the mechanic.

---

## Phase 4: Ship It

### Part 8: UI & game flow
**Goal:** it feels like a complete game.
**Concepts:** `Control` nodes, anchors and containers, `CanvasLayer`, themes, pausing (`GetTree().Paused` + `ProcessMode`),
button focus and keyboard navigation.
- [ ] Main menu
- [ ] Pause menu (Esc)
- [ ] HUD: gems + translocator status
- [ ] Level complete screen
- [ ] Restart room (R)
- [ ] End / credits screen

### Part 9: Art, animation, audio, juice
**Goal:** polish.
**Concepts:** `AnimatedSprite2D` / `SpriteFrames`, `AnimationPlayer`, particles, screen shake, squash and stretch,
audio buses, physics interpolation vs. pixel snapping.
- [x] Player animations: idle, run, jump, fall (wizard sprite, done early during Part 5; the blink covers teleport)
- [ ] Translocator animation + glow
- [ ] Dust on jump/land, teleport effect
- [ ] SFX: jump, land, drop, recall, door, plate, death
- [ ] Music + volume settings

### Part 10: Ship it
**Goal:** a build anyone can run.
**Concepts:** export presets, .NET export, testing the exported build.
- [ ] Windows export to `../builds/`
- [ ] Full playtest + bug-fix pass
- [ ] README with controls and a short description

---

## Changelog
<!-- One line per finished part, e.g. "Part 1: Player movement (2026-11-05)" -->
- Part 0: Project setup (2026-11-05)
- Part 1: Player movement (2026-11-05)
- Part 2: skipped; basic movement is enough, and the camera moved to Part 5 (2026-11-05)
- Part 3: Translocator v1: drop & recall (2026-11-12)
- Part 4: Translocator rules & feedback (2026-11-15)
