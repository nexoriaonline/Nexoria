using Godot;

namespace AutoFarm
{
	[GlobalClass]
	public partial class GrowthSpeedBuildingEffect : BuildingEffect
	{
		[Export]
		public float Bonus;

		public override void Apply(BasePlant plant, BaseBuilding source)
		{
			plant?.AddAuraModifiers(Bonus, 0.0f);
		}
	}
}
