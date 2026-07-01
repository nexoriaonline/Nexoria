using Godot;
using System.Collections.Generic;

namespace AutoFarm
{
	[GlobalClass]
	public partial class AutoMergeBuildingEffect : BuildingEffect
	{
		[Export]
		public float IntervalSeconds = 20.0f;

		[Export]
		public int MergePairsPerInterval = 1;

		private readonly Dictionary<ulong, float> timersByBuildingId = new();

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

			var mergeCount = Mathf.Max(1, MergePairsPerInterval);
			for (var i = 0; i < mergeCount; i++)
			{
				if (!source.FarmingController.TryAutoMergePlantsInAura(source))
				{
					return;
				}
			}
		}
	}
}
