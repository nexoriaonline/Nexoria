using AutoFarm;
using Godot;
using System;

public partial class TestPlant : BasePlant
{
	public override void AddGrowthState(int amount)
	{
		base.AddGrowthState(amount);
		UpdateGrowthFrame();
	}

	public override void OnGather()
	{
		base.OnGather();
		UpdateGrowthFrame();
	}

	public override void _Ready()
	{
		base._Ready();
		UpdateGrowthFrame();
	}

	protected override void UpdateGrowthFrame()
	{
		AnimatedSprite2D.Frame = CurrentGrowthLevel;
	}
}
