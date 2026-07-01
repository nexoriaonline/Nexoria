using Godot;

namespace AutoFarm
{
	[GlobalClass]
	public abstract partial class BuildingEffect : Resource
	{
		public virtual void Apply(BasePlant plant, BaseBuilding source)
		{
		}

		public virtual void Tick(BaseBuilding source, double delta)
		{
		}
	}
}
