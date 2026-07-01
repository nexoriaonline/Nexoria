using AutoFarm;
using Godot;

public partial class CombinerBuilding : BaseBuilding
{
	public override void _Ready()
	{
		if (Effects.Count == 0)
		{
			Effects.Add(new AutoMergeBuildingEffect());
		}

		base._Ready();
	}
}
