using Godot;
using System;

public partial class GoldIndicator : Node2D
{
	public EconomyController EconomyController;

	[Export]
	public Label GoldLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EconomyController = GetNode<EconomyController>("/root/Node2D/EconomyController");
		GoldLabel.Text = FormatGold(EconomyController.CurrentPlayerGold);
		EconomyController.OnGoldChanged += UpdateGoldIndicator;
	}

	private void UpdateGoldIndicator(float amount)
	{
		GoldLabel.Text = FormatGold(amount);
	}

	private static string FormatGold(float amount)
	{
		string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };
		double displayAmount = amount;
		int suffixIndex = 0;

		while (displayAmount >= 1000 && suffixIndex < suffixes.Length - 1)
		{
			displayAmount /= 1000;
			suffixIndex++;
		}

		if (suffixIndex == 0)
		{
			return Mathf.FloorToInt(amount).ToString();
		}

		return displayAmount >= 100
			? $"{displayAmount:0}{suffixes[suffixIndex]}"
			: $"{displayAmount:0.#}{suffixes[suffixIndex]}";
	}
}
