using Godot;
using System.Collections.Generic;

namespace AutoFarm
{
	[GlobalClass]
	public partial class DuplicatePlantBuildingEffect : BuildingEffect
	{
		[Export]
		public float IntervalSeconds = 30.0f;

		[Export]
		public float MinSpawnDistance = 36.0f;

		[Export]
		public float MaxSpawnDistance = 180.0f;

		[Export]
		public int SpawnAttempts = 32;

		private readonly Dictionary<ulong, float> timersByBuildingId = new();
		private readonly RandomNumberGenerator random = new();

		public override void Tick(BaseBuilding source, double delta)
		{
			if (source?.FarmingController == null || source.IsDragging || IntervalSeconds <= 0) return;

			var sourceId = source.GetInstanceId();
			timersByBuildingId.TryGetValue(sourceId, out var timer);
			timer += (float)delta;

			if (timer < IntervalSeconds)
			{
				timersByBuildingId[sourceId] = timer;
				return;
			}

			timersByBuildingId[sourceId] = 0.0f;
			TryDuplicateRandomPlant(source);
		}

		private void TryDuplicateRandomPlant(BaseBuilding source)
		{
			var targetPlant = PickRandomPlantInsideAura(source);
			if (targetPlant == null) return;

			var scenePath = targetPlant.SceneFilePath;
			if (string.IsNullOrEmpty(scenePath))
			{
				GD.PushWarning($"Cannot duplicate {targetPlant.Name}: plant has no source scene path.");
				return;
			}

			var plantScene = GD.Load<PackedScene>(scenePath);
			if (plantScene == null)
			{
				GD.PushWarning($"Cannot duplicate {targetPlant.Name}: failed to load {scenePath}.");
				return;
			}

			if (!source.FarmingController.TryFindFreePlantPositionNear(
				targetPlant.GlobalPosition,
				MinSpawnDistance,
				MaxSpawnDistance,
				SpawnAttempts,
				out var spawnPosition,
				random))
			{
				return;
			}

			var duplicatedPlant = plantScene.Instantiate<Node2D>();
			duplicatedPlant.GlobalPosition = spawnPosition;
			source.GetTree().CurrentScene.AddChild(duplicatedPlant);
			if (duplicatedPlant is BasePlant plant)
			{
				plant.PlayDuplicateSpawnAnimation();
			}
		}

		private BasePlant PickRandomPlantInsideAura(BaseBuilding source)
		{
			var candidates = new List<BasePlant>();

			foreach (var plant in source.FarmingController.CurrentPlantsOnFarm)
			{
				if (plant == null || !GodotObject.IsInstanceValid(plant)) continue;

				if (source.IsPlantInsideAura(plant))
				{
					candidates.Add(plant);
				}
			}

			if (candidates.Count == 0) return null;

			random.Randomize();
			return candidates[random.RandiRange(0, candidates.Count - 1)];
		}
	}
}
