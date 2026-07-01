using AutoFarm;
using Godot;

public partial class DuplicatorBuilding : BaseBuilding
{
	public override void _Ready()
	{
		if (Effects.Count == 0)
		{
			Effects.Add(new DuplicatePlantBuildingEffect());
		}

		base._Ready();
	}
}
