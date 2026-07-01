# Idle Farm Game

Idle Farm Game is a small Godot 4 C# farming project about planting crops, waiting for them to grow, collecting gold, and merging matching plants into stronger versions.

## Current Features

- Shop-based crop purchasing and placement.
- Plant growth stages with visual feedback.
- Harvest animation and compact gold display.
- Merge mechanic: drag two plants of the same type and level together to create a higher-level plant.
- Optional plant level indicators with hover support.
- Character harvesting behavior with simple wandering while idle.

## Tech Stack

- Godot 4.6
- C#
- .NET project: `AutoFarm.csproj`

## Running

Open the project folder in Godot 4.6 and run `MainScene.tscn`.

You can also verify the C# project with:

```powershell
dotnet build
```

## Notes

The project uses free pixel-art farm assets stored in the repository. IDE state such as `.vs/` is intentionally ignored and should not be committed.
