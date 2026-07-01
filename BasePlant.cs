using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarm
{
	public partial class BasePlant: Node2D
	{
		[Export]
		public float BasePlantCost;
		[Export]
		public int BasePlantLevel;
		//How much need to make gather actions for this resource
		[Export]
		public int GatherCount;
		[Export]
		//States to achieve max growth
		public int GrowthStateCount = 1;
		//Controller ticks for single state change
		[Export]
		public float TimeToChangeSingleState = 3;

		[Export]
		public AnimatedSprite2D AnimatedSprite2D;

		public bool IsFullyGrowed = false;
		public float GrowthSpeedMultiplier = 1.0f;
		public float HarvestGoldMultiplier = 1.0f;
		private float currentTimer;
		private int currentHarvestCount = 0;
		private Tween visualTween;
		private Vector2 baseScale;
		private Color baseModulate;
		private float baseRotation;
		private Label levelBadge;
		private CheckBox levelShowCheckBox;
		private int baseZIndex;
		public int CurrentGrowthLevel = 0;
		public FarmingController FarmingController;
		public EconomyController EconomyController;
		public override void _Ready()
		{
			base._Ready();
			baseScale = Scale;
			baseModulate = Modulate;
			baseRotation = Rotation;
			baseZIndex = ZIndex;
			FarmingController = GetNode<FarmingController>("/root/Node2D/FarmingController");
			FarmingController.AddPlant(this);
			EconomyController = GetNode<EconomyController>("/root/Node2D/EconomyController");
			levelShowCheckBox = FindLevelShowCheckBox();
			CreateLevelBadge();
			UpdateGrowthFrame();
			UpdateLevelBadge();
			UpdateLevelBadgeVisibility();
		}

		public override void _Process(double delta)
		{
			UpdateLevelBadgeVisibility();
		}

		protected virtual void UpdateGrowthFrame()
		{
			AnimatedSprite2D.Frame = CurrentGrowthLevel;
		}

		public virtual void OnGather()
		{
			EconomyController.AddGold(BasePlantLevel * BasePlantCost * HarvestGoldMultiplier);
			UpdateGrowthFrame();
			PlayHarvestAnimation();
		}

		public bool CanMergeWith(BasePlant other)
		{
			return other != null &&
				other != this &&
				GetType() == other.GetType() &&
				BasePlantLevel == other.BasePlantLevel;
		}

		public void MergeWith(BasePlant other)
		{
			if (!CanMergeWith(other)) return;

			BasePlantLevel *= 2;
			currentTimer = 0;
			currentHarvestCount = 0;
			CurrentGrowthLevel = 0;
			IsFullyGrowed = false;
			UpdateGrowthFrame();
			UpdateLevelBadge();
			PlayMergeAnimation();
		}

		public void ResetGrowthProgress()
		{
			currentTimer = 0;
			currentHarvestCount = 0;
			CurrentGrowthLevel = 0;
			IsFullyGrowed = false;
			UpdateGrowthFrame();
		}

		public void ResetAuraModifiers()
		{
			GrowthSpeedMultiplier = 1.0f;
			HarvestGoldMultiplier = 1.0f;
		}

		public void AddAuraModifiers(float growthBonus, float goldBonus)
		{
			GrowthSpeedMultiplier += growthBonus;
			HarvestGoldMultiplier += goldBonus;
		}

		public void SetDraggingVisual(bool isDragging)
		{
			visualTween?.Kill();
			ZIndex = isDragging ? 1000 : baseZIndex;
			Modulate = isDragging ? new Color(baseModulate.R, baseModulate.G, baseModulate.B, 0.75f) : baseModulate;
		}

		public bool ContainsGlobalPoint(Vector2 globalPosition)
		{
			if (AnimatedSprite2D == null) return false;

			var localPosition = AnimatedSprite2D.ToLocal(globalPosition);
			var frameTexture = AnimatedSprite2D.SpriteFrames?.GetFrameTexture(AnimatedSprite2D.Animation, AnimatedSprite2D.Frame);
			if (frameTexture == null) return false;

			var frameSize = frameTexture.GetSize();
			var framePosition = AnimatedSprite2D.Centered ? -frameSize / 2.0f : Vector2.Zero;
			return new Rect2(framePosition + AnimatedSprite2D.Offset, frameSize).HasPoint(localPosition);
		}

		private void CreateLevelBadge()
		{
			levelBadge = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Size = new Vector2(14, 9),
				Position = new Vector2(-7, -36),
				ZIndex = 100,
				Visible = false
			};

			levelBadge.AddThemeFontSizeOverride("font_size", 7);
			levelBadge.AddThemeColorOverride("font_color", new Color(0.23f, 0.16f, 0.06f));
			levelBadge.AddThemeColorOverride("font_outline_color", new Color(1.0f, 0.9f, 0.48f, 0.65f));
			levelBadge.AddThemeConstantOverride("outline_size", 1);

			var badgeStyle = new StyleBoxFlat
			{
				BgColor = new Color(1.0f, 0.82f, 0.26f, 0.62f),
				BorderColor = new Color(0.28f, 0.18f, 0.05f, 0.75f),
				BorderWidthBottom = 1,
				BorderWidthLeft = 1,
				BorderWidthRight = 1,
				BorderWidthTop = 1
			};
			badgeStyle.SetCornerRadiusAll(5);
			levelBadge.AddThemeStyleboxOverride("normal", badgeStyle);
			AddChild(levelBadge);
		}

		private CheckBox FindLevelShowCheckBox()
		{
			return GetTree().CurrentScene?.FindChild("LevelShowCheckBox", true, false) as CheckBox;
		}

		private void UpdateLevelBadge()
		{
			if (levelBadge != null)
			{
				levelBadge.Text = BasePlantLevel.ToString();
			}
		}

		private void UpdateLevelBadgeVisibility()
		{
			if (levelBadge == null) return;

			if (levelShowCheckBox == null || !IsInstanceValid(levelShowCheckBox))
			{
				levelShowCheckBox = FindLevelShowCheckBox();
			}

			levelBadge.Visible = (levelShowCheckBox != null && levelShowCheckBox.ButtonPressed) ||
				ContainsGlobalPoint(GetGlobalMousePosition());
		}

		private void RestartVisualTween()
		{
			visualTween?.Kill();
			Scale = baseScale;
			Modulate = baseModulate;
			Rotation = baseRotation;
			visualTween = CreateTween();
		}

		private void PlayMatureAnimation()
		{
			RestartVisualTween();
			visualTween.TweenProperty(this, "scale", baseScale * 1.2f, 0.12f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(1.25f, 1.2f, 0.75f), 0.12f);
			visualTween.TweenProperty(this, "scale", baseScale, 0.18f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", baseModulate, 0.18f);
		}

		private void PlayGrowthStepAnimation()
		{
			RestartVisualTween();
			visualTween.TweenProperty(this, "scale", new Vector2(baseScale.X * 1.08f, baseScale.Y * 0.94f), 0.08f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(0.8f, 1.18f, 0.72f), 0.08f);
			visualTween.TweenProperty(this, "scale", baseScale, 0.14f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", baseModulate, 0.14f);
		}

		private void PlayHarvestAnimation()
		{
			RestartVisualTween();
			visualTween.TweenProperty(this, "scale", baseScale * 1.35f, 0.08f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(1.0f, 0.95f, 0.55f), 0.08f);
			visualTween.Parallel().TweenProperty(this, "rotation", baseRotation + 0.12f, 0.08f);
			visualTween.TweenProperty(this, "scale", new Vector2(baseScale.X * 0.8f, baseScale.Y * 1.25f), 0.1f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.InOut);
			visualTween.Parallel().TweenProperty(this, "rotation", baseRotation - 0.08f, 0.1f);
			visualTween.TweenProperty(this, "scale", baseScale, 0.18f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", baseModulate, 0.18f);
			visualTween.Parallel().TweenProperty(this, "rotation", baseRotation, 0.18f);
		}

		public void PlayMergeAnimation()
		{
			RestartVisualTween();
			visualTween.TweenProperty(this, "scale", baseScale * 1.3f, 0.12f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(0.8f, 1.25f, 0.85f), 0.12f);
			visualTween.TweenProperty(this, "scale", baseScale, 0.18f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", baseModulate, 0.18f);
		}

		public void PlayDuplicateSpawnAnimation()
		{
			RestartVisualTween();
			Scale = baseScale * 0.25f;
			Modulate = new Color(0.65f, 1.25f, 0.78f, 0.2f);
			visualTween.TweenProperty(this, "scale", baseScale * 1.2f, 0.18f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(0.8f, 1.25f, 0.85f, 1.0f), 0.18f);
			visualTween.TweenProperty(this, "scale", baseScale, 0.12f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out);
			visualTween.Parallel().TweenProperty(this, "modulate", baseModulate, 0.12f);
		}

		public void PlayAutoMergeIntoAnimation(Vector2 targetPosition, Action onFinished)
		{
			RestartVisualTween();
			ZIndex = 1000;
			visualTween.TweenProperty(this, "global_position", targetPosition, 0.28f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.InOut);
			visualTween.Parallel().TweenProperty(this, "scale", baseScale * 0.35f, 0.28f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
			visualTween.Parallel().TweenProperty(this, "modulate", new Color(0.8f, 1.25f, 0.85f, 0.35f), 0.28f);
			visualTween.TweenCallback(Callable.From(() =>
			{
				ZIndex = baseZIndex;
				onFinished?.Invoke();
			}));
		}

		public void PlayInvalidDropAnimation()
		{
			RestartVisualTween();
			visualTween.TweenProperty(this, "rotation", baseRotation + 0.12f, 0.06f);
			visualTween.TweenProperty(this, "rotation", baseRotation - 0.12f, 0.08f);
			visualTween.TweenProperty(this, "rotation", baseRotation, 0.06f);
		}

		public virtual void OnPlant()
		{

		}

		public virtual void OnDestroy()
		{

		}

		public virtual void SpecialAction()
		{

		}

		public virtual void TimeTick(float tickValue)
		{
			currentTimer += tickValue * GrowthSpeedMultiplier;
			if(currentTimer >= TimeToChangeSingleState)
			{
				currentTimer = 0;
				AddGrowthState(1);
			}
		}

		public virtual void DoHarvest()
		{
			currentHarvestCount++;
			if(currentHarvestCount >= GatherCount)
			{
				currentHarvestCount = 0;
				IsFullyGrowed = false;
				CurrentGrowthLevel = 0;
				OnGather();
			}
		}

		public virtual void AddGrowthState(int amount)
		{
			int maxGrowthLevel = GrowthStateCount - 1;

			if (CurrentGrowthLevel + amount >= maxGrowthLevel)
			{
				CurrentGrowthLevel = maxGrowthLevel;
				IsFullyGrowed = true;
				UpdateGrowthFrame();
				PlayMatureAnimation();
				return;
			}
			CurrentGrowthLevel += amount;
			UpdateGrowthFrame();
			PlayGrowthStepAnimation();
		}
	}
}
