using AutoFarm;
using Godot;
using System;
using System.Collections.Generic;

public partial class ShopController : Node2D
{
	private enum ShopItemType
	{
		Plant,
		Building
	}

	private sealed class ShopItemData
	{
		public string Name;
		public float Price;
		public PackedScene Scene;
		public ShopItemType ItemType;
		public Texture2D ShopIconTexture;
		public Texture2D PlacementTexture;
		public Vector2 ShopIconScale = Vector2.One;
		public bool UsesCustomShopIconScale;
		public Vector2 PreviewScale = Vector2.One;
		public float AuraRadius;
	}

	[Export]
	public NodePath ShopIconPath = "/root/Node2D/ShopIcon";

	[Export]
	public NodePath EconomyControllerPath = "/root/Node2D/EconomyController";

	[Export]
	public NodePath FarmingControllerPath = "/root/Node2D/FarmingController";

	[Export]
	public NodePath ShopItemsRootPath = "TileMapLayer";

	[Export]
	public NodePath ButtonBuildingsPath = "ButtonBuildings";

	[Export]
	public NodePath ButtonPlantsPath = "ButtonPlants";

	[Export]
	public float BuildingShopIconMaxSize = 14.0f;

	[Export]
	public PackedScene PumpkinPlantScene = GD.Load<PackedScene>("res://PumpkinPlant.tscn");

	[Export]
	public PackedScene PotatoPlantScene = GD.Load<PackedScene>("res://PotatoPlant.tscn");

	[Export]
	public PackedScene CarrotPlantScene = GD.Load<PackedScene>("res://CarrotPlant.tscn");

	[Export]
	public PackedScene TomatoPlantScene = GD.Load<PackedScene>("res://TomatoPlant.tscn");

	[Export]
	public PackedScene ChiliPlantScene = GD.Load<PackedScene>("res://ChiliPlant.tscn");

	[Export]
	public PackedScene ScarecrowBuildingScene = GD.Load<PackedScene>("res://ScarecrowBuilding.tscn");

	[Export]
	public PackedScene WellBuildingScene = GD.Load<PackedScene>("res://WellBuilding.tscn");

	[Export]
	public PackedScene LampBuildingScene = GD.Load<PackedScene>("res://LampBuilding.tscn");

	[Export]
	public PackedScene DuplicatorBuildingScene = GD.Load<PackedScene>("res://DuplicatorBuilding.tscn");

	[Export]
	public PackedScene CombinerBuildingScene = GD.Load<PackedScene>("res://CombinerBuilding.tscn");

	[Export]
	public Texture2D ScarecrowBuildingShopIcon;

	[Export]
	public Texture2D WellBuildingShopIcon;

	[Export]
	public Texture2D LampBuildingShopIcon;

	[Export]
	public Texture2D DuplicatorBuildingShopIcon;

	[Export]
	public Texture2D CombinerBuildingShopIcon;

	public Sprite2D ShopSprite;
	private EconomyController _economyController;
	private FarmingController _farmingController;
	private readonly List<Sprite2D> _shopItems = new();
	private readonly List<Vector2> _shopItemIconBaseScales = new();
	private readonly List<ShopItemData> _plantShopItems = new();
	private readonly List<ShopItemData> _buildingShopItems = new();
	private Node2D _placementPreview;
	private Sprite2D _placementPreviewSprite;
	private Line2D _placementPreviewAura;
	private ShopItemData _itemToPlace;
	private bool _isPlacingItem;
	private ShopItemType _activeShopTab = ShopItemType.Plant;
	private Sprite2D _plantsTabButton;
	private Sprite2D _buildingsTabButton;
	private Tween _shopTween;
	private Vector2 _baseScale;
	private Color _baseModulate;
	private bool _isShopAnimating;

	public override void _Ready()
	{
		base._Ready();
		_baseScale = Scale;
		_baseModulate = Modulate;
		ShopSprite = GetNodeOrNull<Sprite2D>(ShopIconPath);
		_economyController = GetNodeOrNull<EconomyController>(EconomyControllerPath);
		_farmingController = GetNodeOrNull<FarmingController>(FarmingControllerPath);

		if (ShopSprite != null)
		{
			var area = ShopSprite.GetNodeOrNull<Area2D>("Area2D");
			if (area != null)
			{
				area.InputEvent += OnAreaInputEvent;
			}
		}

		CacheShopItems();
		CacheShopItemsData();
		CacheShopTabs();
		RefreshShopItems();
	}

	public void ToggleShop()
	{
		if (_isShopAnimating) return;

		if (Visible)
		{
			CloseShopAnimated();
		}
		else
		{
			OpenShopAnimated();
		}
	}

	private void OpenShopAnimated()
	{
		RestartShopTween();
		Visible = true;
		Scale = new Vector2(_baseScale.X * 0.85f, _baseScale.Y * 0.65f);
		Modulate = new Color(_baseModulate.R, _baseModulate.G, _baseModulate.B, 0.0f);

		_shopTween.TweenProperty(this, "scale", _baseScale * 1.05f, 0.12f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		_shopTween.Parallel().TweenProperty(this, "modulate", _baseModulate, 0.12f);
		_shopTween.TweenProperty(this, "scale", _baseScale, 0.08f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
		_shopTween.TweenCallback(Callable.From(() => _isShopAnimating = false));
	}

	private void CloseShopAnimated()
	{
		RestartShopTween();
		_shopTween.TweenProperty(this, "scale", new Vector2(_baseScale.X * 0.9f, _baseScale.Y * 0.55f), 0.12f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);
		_shopTween.Parallel().TweenProperty(this, "modulate", new Color(_baseModulate.R, _baseModulate.G, _baseModulate.B, 0.0f), 0.12f);
		_shopTween.TweenCallback(Callable.From(HideShopInstant));
	}

	private void HideShopInstant()
	{
		_shopTween?.Kill();
		_shopTween = null;
		_isShopAnimating = false;
		Visible = false;
		Scale = _baseScale;
		Modulate = _baseModulate;
	}

	private void RestartShopTween()
	{
		_shopTween?.Kill();
		_isShopAnimating = true;
		Scale = _baseScale;
		Modulate = _baseModulate;
		_shopTween = CreateTween();
	}

	private void OnAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.Pressed &&
			mouseEvent.ButtonIndex == MouseButton.Left)
		{
			ToggleShop();
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseEvent ||
			!mouseEvent.Pressed)
		{
			return;
		}

		if (_isPlacingItem)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				PlaceSelectedItem();
				GetViewport().SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right)
			{
				CancelItemPlacement();
				GetViewport().SetInputAsHandled();
			}

			return;
		}

		if (mouseEvent.ButtonIndex != MouseButton.Left)
		{
			return;
		}

		if (!Visible || _isShopAnimating)
		{
			return;
		}

		if (TrySwitchShopTabAt(GetGlobalMousePosition()))
		{
			GetViewport().SetInputAsHandled();
			return;
		}

		if (TryBuyItemAt(GetGlobalMousePosition()))
		{
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _Process(double delta)
	{
		if (_placementPreview != null)
		{
			UpdatePlacementPreview();
		}
	}

	private void CacheShopItems()
	{
		_shopItems.Clear();
		_shopItemIconBaseScales.Clear();

		var itemsRoot = GetNodeOrNull<Node>(ShopItemsRootPath);
		if (itemsRoot == null) return;

		foreach (var child in itemsRoot.GetChildren())
		{
			if (child is Sprite2D sprite && sprite.GetNodeOrNull<Label>("Label") != null)
			{
				_shopItems.Add(sprite);
				_shopItemIconBaseScales.Add(GetShopItemIcon(sprite)?.Scale ?? Vector2.One);
			}
		}
	}

	private void CacheShopItemsData()
	{
		_plantShopItems.Clear();
		_buildingShopItems.Clear();

		AddShopItem(_plantShopItems, "Pumpkin", 20, PumpkinPlantScene, ShopItemType.Plant, GetShopItemSlotIconTexture(0));
		AddShopItem(_plantShopItems, "Potato", 40, PotatoPlantScene, ShopItemType.Plant, GetShopItemSlotIconTexture(1));
		AddShopItem(_plantShopItems, "Carrot", 80, CarrotPlantScene, ShopItemType.Plant, GetShopItemSlotIconTexture(2));
		AddShopItem(_plantShopItems, "Tomato", 160, TomatoPlantScene, ShopItemType.Plant, GetShopItemSlotIconTexture(3));
		AddShopItem(_plantShopItems, "Chili", 320, ChiliPlantScene, ShopItemType.Plant, GetShopItemSlotIconTexture(4));

		AddShopItem(_buildingShopItems, "Scarecrow", 500, ScarecrowBuildingScene, ShopItemType.Building, ScarecrowBuildingShopIcon);
		AddShopItem(_buildingShopItems, "Well", 750, WellBuildingScene, ShopItemType.Building, WellBuildingShopIcon);
		AddShopItem(_buildingShopItems, "Lamp", 1000, LampBuildingScene, ShopItemType.Building, LampBuildingShopIcon);
		AddShopItem(_buildingShopItems, "Duplicator", 1500, DuplicatorBuildingScene, ShopItemType.Building, DuplicatorBuildingShopIcon);
		AddShopItem(_buildingShopItems, "Combiner", 1800, CombinerBuildingScene, ShopItemType.Building, CombinerBuildingShopIcon);
	}

	private Texture2D GetShopItemSlotIconTexture(int itemIndex)
	{
		if (itemIndex < 0 || itemIndex >= _shopItems.Count) return null;

		return GetShopItemIcon(_shopItems[itemIndex])?.Texture;
	}

	private bool TryBuyItemAt(Vector2 globalPosition)
	{
		var visibleItems = GetVisibleShopItems();

		for (var i = 0; i < _shopItems.Count; i++)
		{
			var item = _shopItems[i];
			if (!IsPointInsideSprite(item, globalPosition)) continue;

			if (i >= visibleItems.Count)
			{
				return true;
			}

			var shopItem = visibleItems[i];
			if (shopItem.Scene == null)
			{
				GD.PushWarning($"Shop item {item.Name} does not have a plant scene assigned.");
				return true;
			}

			var price = shopItem.Price;
			if (price <= 0) return true;

			if (_economyController == null)
			{
				GD.PushWarning("ShopController cannot find EconomyController.");
				return true;
			}

			if (_economyController.RemoveGold(price))
			{
				GD.Print($"Bought {shopItem.Name} for {price} gold.");
				StartItemPlacement(shopItem);
			}
			else
			{
				GD.Print($"Not enough gold to buy {shopItem.Name}. Price: {price}");
			}

			return true;
		}

		return false;
	}

	private bool TrySwitchShopTabAt(Vector2 globalPosition)
	{
		if (_plantsTabButton != null && IsPointInsideSprite(_plantsTabButton, globalPosition))
		{
			SetActiveShopTab(ShopItemType.Plant);
			return true;
		}

		if (_buildingsTabButton != null && IsPointInsideSprite(_buildingsTabButton, globalPosition))
		{
			SetActiveShopTab(ShopItemType.Building);
			return true;
		}

		return false;
	}

	private void SetActiveShopTab(ShopItemType shopTab)
	{
		if (_activeShopTab == shopTab) return;

		_activeShopTab = shopTab;
		RefreshShopItems();
		UpdateTabButtonVisuals();
	}

	private List<ShopItemData> GetVisibleShopItems()
	{
		return _activeShopTab == ShopItemType.Plant ? _plantShopItems : _buildingShopItems;
	}

	private void AddShopItem(List<ShopItemData> items, string itemName, float price, PackedScene scene, ShopItemType itemType, Texture2D shopIconTexture = null)
	{
		if (scene == null) return;

		var shopItem = new ShopItemData
		{
			Name = itemName,
			Price = price,
			Scene = scene,
			ItemType = itemType,
			ShopIconTexture = shopIconTexture
		};

		PopulatePreviewData(shopItem);
		items.Add(shopItem);
	}

	private void PopulatePreviewData(ShopItemData shopItem)
	{
		if (shopItem.ItemType == ShopItemType.Building)
		{
			var building = shopItem.Scene.Instantiate<BaseBuilding>();
			shopItem.PlacementTexture = building.GetPreviewTexture();
			shopItem.ShopIconTexture ??= shopItem.PlacementTexture;
			shopItem.ShopIconScale = GetFittedIconScale(shopItem.ShopIconTexture, BuildingShopIconMaxSize);
			shopItem.UsesCustomShopIconScale = true;
			shopItem.PreviewScale = building.Sprite?.GlobalScale ?? building.GlobalScale;
			shopItem.AuraRadius = building.AuraRadius;
			shopItem.Price = building.BuildingCost > 0 ? building.BuildingCost : shopItem.Price;
			building.QueueFree();
			return;
		}

		var plant = CreatePlantPreviewInstance(shopItem.Scene);
		var sourceSprite = plant?.AnimatedSprite2D;
		shopItem.PlacementTexture = GetPlantPreviewTexture(sourceSprite);
		shopItem.ShopIconTexture ??= shopItem.PlacementTexture;
		shopItem.PreviewScale = sourceSprite?.GlobalScale ?? plant?.GlobalScale ?? Vector2.One;
		plant?.QueueFree();
	}

	private void CacheShopTabs()
	{
		_plantsTabButton = GetNodeOrNull<Sprite2D>(ButtonPlantsPath);
		_buildingsTabButton = GetNodeOrNull<Sprite2D>(ButtonBuildingsPath);
		UpdateTabButtonVisuals();
	}

	private void UpdateTabButtonVisuals()
	{
		if (_plantsTabButton != null)
		{
			_plantsTabButton.Modulate = _activeShopTab == ShopItemType.Plant
				? Colors.White
				: new Color(1, 1, 1, 0.62f);
		}

		if (_buildingsTabButton != null)
		{
			_buildingsTabButton.Modulate = _activeShopTab == ShopItemType.Building
				? Colors.White
				: new Color(1, 1, 1, 0.62f);
		}
	}

	private void RefreshShopItems()
	{
		var visibleItems = GetVisibleShopItems();

		for (var i = 0; i < _shopItems.Count; i++)
		{
			var slot = _shopItems[i];
			var isUsed = i < visibleItems.Count;
			slot.Visible = isUsed;
			if (!isUsed) continue;

			var shopItem = visibleItems[i];
			var icon = GetShopItemIcon(slot);
			if (icon != null)
			{
				icon.Texture = shopItem.ShopIconTexture;
				icon.Scale = shopItem.UsesCustomShopIconScale
					? shopItem.ShopIconScale
					: GetShopItemIconBaseScale(i);
			}

			var label = slot.GetNodeOrNull<Label>("Label");
			if (label != null)
			{
				label.Text = shopItem.Price.ToString("0");
			}
		}
	}

	private void StartItemPlacement(ShopItemData shopItem)
	{
		HideShopInstant();
		_isPlacingItem = true;
		_itemToPlace = shopItem;

		_placementPreview?.QueueFree();
		_placementPreview = CreatePlacementPreview(shopItem);
		GetTree().CurrentScene.AddChild(_placementPreview);
		_placementPreview.GlobalPosition = GetGlobalMousePosition();
	}

	private Node2D CreatePlacementPreview(ShopItemData shopItem)
	{
		var previewRoot = new Node2D
		{
			ZIndex = 1000
		};

		_placementPreviewSprite = new Sprite2D
		{
			Texture = shopItem.PlacementTexture,
			Centered = true,
			Modulate = new Color(1, 1, 1, 0.65f)
		};
		_placementPreviewSprite.Scale = shopItem.PreviewScale;
		previewRoot.AddChild(_placementPreviewSprite);

		if (shopItem.ItemType == ShopItemType.Building)
		{
			_placementPreviewAura = CreateAuraPreview(shopItem.AuraRadius);
			previewRoot.AddChild(_placementPreviewAura);
		}
		else
		{
			_placementPreviewAura = null;
		}

		return previewRoot;
	}

	private static Line2D CreateAuraPreview(float radius)
	{
		var aura = new Line2D
		{
			Closed = true,
			DefaultColor = new Color(0.35f, 0.85f, 1.0f, 0.7f),
			Width = 2.0f,
			ZIndex = -1
		};

		for (var i = 0; i < 96; i++)
		{
			var angle = Mathf.Tau * i / 96.0f;
			aura.AddPoint(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
		}

		return aura;
	}

	private static Vector2 GetFittedIconScale(Texture2D texture, float maxSize)
	{
		if (texture == null || maxSize <= 0) return Vector2.One;

		var textureSize = texture.GetSize();
		var largestSide = Mathf.Max(textureSize.X, textureSize.Y);
		if (largestSide <= 0) return Vector2.One;

		var scale = Mathf.Min(1.0f, maxSize / largestSide);
		return new Vector2(scale, scale);
	}

	private Vector2 GetShopItemIconBaseScale(int itemIndex)
	{
		return itemIndex >= 0 && itemIndex < _shopItemIconBaseScales.Count
			? _shopItemIconBaseScales[itemIndex]
			: Vector2.One;
	}

	private void PlaceSelectedItem()
	{
		if (_itemToPlace?.Scene == null)
		{
			GD.PushWarning("ShopController cannot place item because no item scene is selected.");
			return;
		}

		var itemPosition = GetGlobalMousePosition();
		if (IsPlacementBlocked(itemPosition))
		{
			GD.Print("Cannot place item: this position is blocked.");
			return;
		}

		var item = _itemToPlace.Scene.Instantiate<Node2D>();
		GetTree().CurrentScene.AddChild(item);
		item.GlobalPosition = itemPosition;
		StopItemPlacement();
	}

	private void CancelItemPlacement()
	{
		if (_itemToPlace != null)
		{
			_economyController?.AddGold(_itemToPlace.Price);
			GD.Print($"Canceled {_itemToPlace.Name} placement. Refunded {_itemToPlace.Price} gold.");
		}

		StopItemPlacement();
	}

	private void StopItemPlacement()
	{
		_isPlacingItem = false;
		_itemToPlace = null;
		_placementPreview?.QueueFree();
		_placementPreview = null;
		_placementPreviewSprite = null;
		_placementPreviewAura = null;
	}

	private static AutoFarm.BasePlant CreatePlantPreviewInstance(PackedScene plantScene)
	{
		return plantScene?.Instantiate<AutoFarm.BasePlant>();
	}

	private void UpdatePlacementPreview()
	{
		var mousePosition = GetGlobalMousePosition();
		_placementPreview.GlobalPosition = mousePosition;
		_placementPreviewSprite.Modulate = IsPlacementBlocked(mousePosition)
			? new Color(1.0f, 0.28f, 0.28f, 0.75f)
			: new Color(1.0f, 1.0f, 1.0f, 0.65f);
	}

	private bool IsPlacementBlocked(Vector2 globalPosition)
	{
		if (_farmingController == null) return false;

		return _itemToPlace?.ItemType == ShopItemType.Building
			? !_farmingController.IsBuildingPositionAllowed(globalPosition)
			: !_farmingController.IsPlantPositionAllowed(globalPosition);
	}

	private static Texture2D GetPlantPreviewTexture(AnimatedSprite2D sprite)
	{
		return sprite?.SpriteFrames?.GetFrameTexture(sprite.Animation, 0);
	}

	private static Sprite2D GetShopItemIcon(Sprite2D shopItem)
	{
		foreach (var child in shopItem.GetChildren())
		{
			if (child is Sprite2D sprite)
			{
				return sprite;
			}
		}

		return null;
	}

	private static bool IsPointInsideSprite(Sprite2D sprite, Vector2 globalPosition)
	{
		var localPosition = sprite.ToLocal(globalPosition);
		return sprite.GetRect().HasPoint(localPosition);
	}

	private static float GetItemPrice(Sprite2D item)
	{
		var label = item.GetNodeOrNull<Label>("Label");
		if (label == null) return 0;

		if (!float.TryParse(label.Text, out var price))
		{
			GD.PushWarning($"Shop item {item.Name} has invalid price: {label.Text}");
		}

		return price;
	}
}
