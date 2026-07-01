using Godot;

namespace AutoFarm
{
	[GlobalClass]
	public partial class HarvestGoldBuildingEffect : BuildingEffect
	{
		[Export]
		public float Bonus;

		public override void Apply(BasePlant plant, BaseBuilding source)
		{
			plant?.AddAuraModifiers(0.0f, Bonus);
		}
	}
}
