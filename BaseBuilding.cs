using Godot;

namespace AutoFarm
{
	public partial class BaseBuilding : Node2D
	{
		[Export]
		public float BuildingCost;

		[Export]
		public string BuildingName = "Building";

		[Export]
		public float AuraRadius = 80.0f;

		[Export]
		public float GrowthSpeedBonus;

		[Export]
		public float HarvestGoldBonus;

		[Export]
		public Godot.Collections.Array<BuildingEffect> Effects = new();

		[Export]
		public Sprite2D Sprite;

		public FarmingController FarmingController;
		private Tween visualTween;
		private Vector2 baseScale;
		private Color baseModulate;
		private int baseZIndex;
		private bool isDragging;
		private bool forceAuraVisible;
		private bool isAuraVisible;
		public bool IsDragging => isDragging;

		public override void _Ready()
		{
			base._Ready();
			baseScale = Scale;
			baseModulate = Modulate;
			baseZIndex = ZIndex;
			FarmingController = GetNodeOrNull<FarmingController>("/root/Node2D/FarmingController");
			FarmingController?.AddBuilding(this);
			SetAuraVisible(false);
		}

		public override void _Process(double delta)
		{
			TickEffects(delta);

			var shouldShowAura = forceAuraVisible || isDragging || ContainsGlobalPoint(GetGlobalMousePosition());
			if (isAuraVisible != shouldShowAura)
			{
				ApplyAuraVisible(shouldShowAura);
			}
		}

		private void TickEffects(double delta)
		{
			foreach (var effect in Effects)
			{
				effect?.Tick(this, delta);
			}
		}

		public override void _Draw()
		{
			if (!isAuraVisible) return;

			DrawCircle(Vector2.Zero, AuraRadius, new Color(0.35f, 0.85f, 1.0f, 0.12f));
			DrawArc(Vector2.Zero, AuraRadius, 0.0f, Mathf.Tau, 96, new Color(0.35f, 0.85f, 1.0f, 0.65f), 2.0f, true);
		}

		public bool ContainsGlobalPoint(Vector2 globalPosition)
		{
			if (Sprite == null || Sprite.Texture == null) return false;

			var localPosition = Sprite.ToLocal(globalPosition);
			var frameSize = Sprite.Texture.GetSize();
			var framePosition = Sprite.Centered ? -frameSize / 2.0f : Vector2.Zero;
			return new Rect2(framePosition + Sprite.Offset, frameSize).HasPoint(localPosition);
		}

		public bool IsPlantInsideAura(BasePlant plant)
		{
			return plant != null &&
				IsInstanceValid(plant) &&
				GlobalPosition.DistanceSquaredTo(plant.GlobalPosition) <= AuraRadius * AuraRadius;
		}

		public void ApplyEffectsTo(BasePlant plant)
		{
			if (!IsPlantInsideAura(plant)) return;

			if (Effects.Count > 0)
			{
				foreach (var effect in Effects)
				{
					effect?.Apply(plant, this);
				}

				return;
			}

			ApplyLegacyAuraEffect(plant);
		}

		private void ApplyLegacyAuraEffect(BasePlant plant)
		{
			plant?.AddAuraModifiers(GrowthSpeedBonus, HarvestGoldBonus);
		}

		public Texture2D GetPreviewTexture()
		{
			return Sprite?.Texture;
		}

		public void SetDraggingVisual(bool dragging)
		{
			isDragging = dragging;
			visualTween?.Kill();
			ZIndex = dragging ? 1000 : baseZIndex;
			Modulate = dragging ? new Color(baseModulate.R, baseModulate.G, baseModulate.B, 0.75f) : baseModulate;
			SetAuraVisible(dragging);
		}

		public void SetPlacementValidVisual(bool isValid)
		{
			Modulate = isValid
				? (isDragging ? new Color(baseModulate.R, baseModulate.G, baseModulate.B, 0.75f) : baseModulate)
				: new Color(1.0f, 0.28f, 0.28f, 0.75f);
		}

		public void SetAuraVisible(bool visible)
		{
			forceAuraVisible = visible;
			ApplyAuraVisible(forceAuraVisible || isDragging || ContainsGlobalPoint(GetGlobalMousePosition()));
		}

		private void ApplyAuraVisible(bool visible)
		{
			isAuraVisible = visible;
			QueueRedraw();
		}
	}
}
