using AutoFarm;
using Godot;
using System;

public partial class ChiliPlant : BasePlant
{
	[Export]
	public AnimatedSprite2D SecondAnimatedSprite2D;
	protected override void UpdateGrowthFrame()
	{
		base.UpdateGrowthFrame();
		SecondAnimatedSprite2D.Frame = CurrentGrowthLevel;
	}
}
