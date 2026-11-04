# Unity Prototype — Evidence & Scope

**Status:** Early academic greybox / systems prototype. No verified public build. This repository preserves a development snapshot, not a finished playable game.

## What is inspectable in code

| Area | Source files | Observations |
| --- | --- | --- |
| Passenger AI | `Assets/Scripts/PassengerAI.cs` and `PassengerSpawner.cs` | NavMesh agent routing, waypoint paths, optional detours, front-agent collision avoidance and stuck recovery |
| Cat genetics | `Assets/Scripts/CatBreeding.cs`, `CatClassManager.cs`, `CatStats.cs` | Parent class inheritance, rarity/shiny outcome, mutation recipes, numerical stat generation, class tracking |
| Breeding sequence | `Assets/Scripts/Breeding/CatBreedingManager.cs` | Select parents, advance breeding timer, create a newborn object and record its inherited attributes |
| Inventory | `Assets/Scripts/Inventory/CatInventoryManager.cs` | Stores cat objects and their idle/working/breeding status |
| Food totals | `Assets/Scripts/Inventory/FoodManager.cs`, `CatStats.cs` | Global balances and interval-based accumulation tied to working cats |
| UI | `Assets/Scripts/UI/UIManager.cs` | Inventory, parent selection, stats, placement, food and daily-report panels |
| Metro scene | `Assets/Scenes/AiSystem.unity`, `Assets/Scenes/Metro.unity` | Greybox and experimental Unity scenes present in source |

### Caution when reading these files

The **presence of scripts** does not prove all scene references are correctly wired or all features run successfully after a fresh import. There was no local Unity compilation, automated validation or end-to-end gameplay test as part of the portfolio cleanup.

The full design includes kiosk trading, advanced NPC demographics, security raids, a fully balanced day loop, an odor meter and breeding/upkeep expansion. **Do not describe these as completed implementations without additional verification.**

## Why the visuals are greyboxes

The university assignment called for an original game proposal supported by models, Unity scenes and sample mechanics. Simple geometry, temporary UI and low-poly assets are appropriate to this purpose: they communicate scale, placement, player flows and interactions without finalizing visual polish.

### Evidence by source

- Station greybox — `PortfolioMedia/01-metro-greybox.jpg` (report p. 24).
- Cat placement and inventory — `PortfolioMedia/02-cat-placement-ui.jpg` (p. 28).
- Breeding panels — `PortfolioMedia/03-breeding-ui.jpg`, `04-newborn-cat-ui.jpg` (p. 27).
- End-of-day summary — `PortfolioMedia/05-end-of-day-ui.jpg` (p. 29).
- Class tree — `PortfolioMedia/06-cat-class-tree.jpg` (p. 23).
- Environment and character models — `PortfolioMedia/07-incirli-cat-tree.jpg` (p. 8), `08-cat-variations.jpg` (p. 19).
- AI-assisted concept illustration — `PortfolioMedia/09-hybridization-concept.jpg` (p. 29).

## Opening the source

The repository tracks Unity **6000.0.41f1** in `ProjectSettings/ProjectVersion.txt`. Open the repository root via Unity Hub in the matching editor, allowing package import. `Assets/Scenes/AiSystem.unity` is included in the build settings; `Assets/Scenes/Metro.unity` is another scene to inspect.

There is no guarantee that the repository can generate a usable build as-is. Treat it as an **educational systems prototype**.
