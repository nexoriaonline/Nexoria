using AutoFarm;
using Godot;
using System;
using System.Collections.Generic;

public partial class FarmingController : Node2D
{
    [Export]
    public Timer LocalTimer;

    [Export]
    public NodePath TrashChestPath = "/root/Node2D/ShopController/TrashChest";

    [Export]
    public NodePath HomeAreaPath = "/root/Node2D/Home/Area2D";

    [Export]
    public float PlantPlacementRadius = 32.0f;

    [Export]
    public float BuildingPlacementRadius = 32.0f;

    public List<BasePlant> CurrentPlantsOnFarm = new List<BasePlant>();
    public List<BasePlant> PlantsReadyToHarvest = new List<BasePlant>();
    public List<BaseBuilding> CurrentBuildingsOnFarm = new List<BaseBuilding>();
    private readonly List<TileMapLayer> plantableTileLayers = new List<TileMapLayer>();
    private readonly List<TileMapLayer> blockedTileLayers = new List<TileMapLayer>();
    private readonly HashSet<BasePlant> plantsInAutoMergeAnimation = new HashSet<BasePlant>();
    private BasePlant draggedPlant;
    private BaseBuilding draggedBuilding;
    private Vector2 draggedPlantStartPosition;
    private Vector2 draggedPlantOffset;
    private Vector2 draggedBuildingStartPosition;
    private Vector2 draggedBuildingOffset;
    private Character character;
    private TrashChest trashChest;
    private Area2D homeArea;

    public override void _Ready()
    {
        base._Ready();
        LocalTimer.WaitTime = 1;
        LocalTimer.OneShot = false;
        LocalTimer.Timeout += OnTimerTick;
        LocalTimer.Start();
        character = GetNodeOrNull<Character>("/root/Node2D/CharacterBody2D");
        trashChest = GetNodeOrNull<TrashChest>(TrashChestPath);
        homeArea = GetNodeOrNull<Area2D>(HomeAreaPath);
        CachePlantableTileLayers();
    }

    private void OnTimerTick()
    {
        UpdatePlantAuraModifiers();

        foreach(var plant in CurrentPlantsOnFarm)
        {
            if (plant == null || plant.IsFullyGrowed) continue;
            plant.TimeTick(1);
            if (plant.IsFullyGrowed)
            {
                PlantsReadyToHarvest.Add(plant);
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseEvent ||
            mouseEvent.ButtonIndex != MouseButton.Left)
        {
            return;
        }

        if (mouseEvent.Pressed)
        {
            StartPlantDrag(GetGlobalMousePosition());
            if (draggedPlant == null)
            {
                StartBuildingDrag(GetGlobalMousePosition());
            }
            return;
        }

        FinishPlantDrag(GetGlobalMousePosition());
    }

    public override void _Process(double delta)
    {
        if (draggedPlant != null)
        {
            draggedPlant.GlobalPosition = GetGlobalMousePosition() + draggedPlantOffset;
            trashChest?.SetOpen(IsDraggingPlantOverTrashChest());
        }
        else if (draggedBuilding != null)
        {
            draggedBuilding.GlobalPosition = GetGlobalMousePosition() + draggedBuildingOffset;
            var isOverTrashChest = IsDraggingOverTrashChest();
            trashChest?.SetOpen(isOverTrashChest);
            draggedBuilding.SetPlacementValidVisual(isOverTrashChest || IsBuildingPositionAllowed(draggedBuilding.GlobalPosition, draggedBuilding));
        }

        UpdatePlantAuraModifiers();
    }

    private void StartPlantDrag(Vector2 globalPosition)
    {
        draggedPlant = FindPlantAt(globalPosition);
        if (draggedPlant == null) return;

        draggedPlantStartPosition = draggedPlant.GlobalPosition;
        draggedPlantOffset = draggedPlant.GlobalPosition - globalPosition;
        draggedPlant.SetDraggingVisual(true);
        GetViewport().SetInputAsHandled();
    }

    private void StartBuildingDrag(Vector2 globalPosition)
    {
        draggedBuilding = FindBuildingAt(globalPosition);
        if (draggedBuilding == null) return;

        draggedBuildingStartPosition = draggedBuilding.GlobalPosition;
        draggedBuildingOffset = draggedBuilding.GlobalPosition - globalPosition;
        draggedBuilding.SetDraggingVisual(true);
        GetViewport().SetInputAsHandled();
    }

    private void FinishPlantDrag(Vector2 globalPosition)
    {
        if (draggedPlant == null)
        {
            FinishBuildingDrag();
            return;
        }

        var plantToMerge = draggedPlant;
        var targetPlant = FindPlantAt(globalPosition, plantToMerge);
        plantToMerge.SetDraggingVisual(false);

        if (IsDraggingPlantOverTrashChest())
        {
            DeletePlant(plantToMerge);
        }
        else if (targetPlant != null && targetPlant.CanMergeWith(plantToMerge))
        {
            MergePlants(plantToMerge, targetPlant);
        }
        else if (targetPlant == null && CanMovePlantTo(plantToMerge, plantToMerge.GlobalPosition))
        {
            MovePlant(plantToMerge);
        }
        else
        {
            plantToMerge.GlobalPosition = draggedPlantStartPosition;
            plantToMerge.PlayInvalidDropAnimation();
        }

        trashChest?.SetOpen(false);
        draggedPlant = null;
        GetViewport().SetInputAsHandled();
    }

    private void FinishBuildingDrag()
    {
        if (draggedBuilding == null) return;

        var building = draggedBuilding;
        building.SetDraggingVisual(false);

        if (IsDraggingOverTrashChest())
        {
            DeleteBuilding(building);
        }
        else if (IsBuildingPositionAllowed(building.GlobalPosition, building))
        {
            building.SetPlacementValidVisual(true);
            UpdatePlantAuraModifiers();
        }
        else
        {
            building.GlobalPosition = draggedBuildingStartPosition;
            building.SetPlacementValidVisual(true);
        }

        trashChest?.SetOpen(false);
        draggedBuilding = null;
        GetViewport().SetInputAsHandled();
    }

    private bool IsDraggingPlantOverTrashChest()
    {
        return IsDraggingOverTrashChest();
    }

    private bool IsDraggingOverTrashChest()
    {
        return trashChest != null &&
            IsInstanceValid(trashChest) &&
            trashChest.ContainsGlobalPoint(GetGlobalMousePosition());
    }

    private void DeletePlant(BasePlant plant)
    {
        RemovePlant(plant);
        RemoveReadyPlant(plant);
        ClearCharacterTarget(plant);
        plant.QueueFree();
        UpdatePlantAuraModifiers();
    }

    private void DeleteBuilding(BaseBuilding building)
    {
        RemoveBuilding(building);
        building.QueueFree();
        UpdatePlantAuraModifiers();
    }

    private void MergePlants(BasePlant plantToMerge, BasePlant targetPlant)
    {
        RemovePlant(plantToMerge);
        RemoveReadyPlant(plantToMerge);
        RemoveReadyPlant(targetPlant);
        ClearCharacterTarget(plantToMerge);
        ClearCharacterTarget(targetPlant);
        targetPlant.MergeWith(plantToMerge);
        plantToMerge.QueueFree();
        UpdatePlantAuraModifiers();
    }

    private void MergePlantsAnimated(BasePlant plantToMerge, BasePlant targetPlant)
    {
        RemovePlant(plantToMerge);
        RemoveReadyPlant(plantToMerge);
        RemoveReadyPlant(targetPlant);
        ClearCharacterTarget(plantToMerge);
        ClearCharacterTarget(targetPlant);
        plantsInAutoMergeAnimation.Add(plantToMerge);
        plantsInAutoMergeAnimation.Add(targetPlant);

        plantToMerge.PlayAutoMergeIntoAnimation(targetPlant.GlobalPosition, () =>
        {
            if (IsInstanceValid(targetPlant) && IsInstanceValid(plantToMerge))
            {
                targetPlant.MergeWith(plantToMerge);
                plantToMerge.QueueFree();
            }

            plantsInAutoMergeAnimation.Remove(plantToMerge);
            plantsInAutoMergeAnimation.Remove(targetPlant);
            UpdatePlantAuraModifiers();
        });
    }

    private void MovePlant(BasePlant plant)
    {
        RemoveReadyPlant(plant);
        ClearCharacterTarget(plant);
        plant.ResetGrowthProgress();
        UpdatePlantAuraModifiers();
    }

    private bool CanMovePlantTo(BasePlant plant, Vector2 globalPosition)
    {
        if (plant.GlobalPosition.DistanceSquaredTo(draggedPlantStartPosition) <= 1.0f) return false;

        return IsPlantPositionAllowed(globalPosition, plant);
    }

    private BaseBuilding FindBuildingAt(Vector2 globalPosition, BaseBuilding ignoredBuilding = null)
    {
        for (var i = CurrentBuildingsOnFarm.Count - 1; i >= 0; i--)
        {
            var building = CurrentBuildingsOnFarm[i];
            if (building == null || building == ignoredBuilding || !IsInstanceValid(building)) continue;
            if (building.ContainsGlobalPoint(globalPosition))
            {
                return building;
            }
        }

        return null;
    }

    private BasePlant FindPlantAt(Vector2 globalPosition, BasePlant ignoredPlant = null)
    {
        for (var i = CurrentPlantsOnFarm.Count - 1; i >= 0; i--)
        {
            var plant = CurrentPlantsOnFarm[i];
            if (plant == null || plant == ignoredPlant || !IsInstanceValid(plant)) continue;
            if (plant.ContainsGlobalPoint(globalPosition))
            {
                return plant;
            }
        }

        return null;
    }

    private void ClearCharacterTarget(BasePlant plant)
    {
        if (character?.CurrentTarget != plant) return;

        character.CurrentTarget = null;
        character.ChangePlayerState(PlayerStates.Idle);
    }

    public void AddPlant(BasePlant plant)
    {
        if (CurrentPlantsOnFarm.Contains(plant)) return;

        CurrentPlantsOnFarm.Add(plant);
        plant.FarmingController = this;
    }

    public void RemovePlant(BasePlant plant)
    {
        if (CurrentPlantsOnFarm.Contains(plant))
        {
            CurrentPlantsOnFarm.Remove(plant);
        }     
    }

    public void RemoveReadyPlant(BasePlant plant)
    {
        if (PlantsReadyToHarvest.Contains(plant))
        {
            PlantsReadyToHarvest.Remove(plant);
        }
    }

    public void AddBuilding(BaseBuilding building)
    {
        if (CurrentBuildingsOnFarm.Contains(building)) return;

        CurrentBuildingsOnFarm.Add(building);
        building.FarmingController = this;
        UpdatePlantAuraModifiers();
    }

    public void RemoveBuilding(BaseBuilding building)
    {
        if (CurrentBuildingsOnFarm.Contains(building))
        {
            CurrentBuildingsOnFarm.Remove(building);
        }
    }

    public bool HasPlantWithinPlacementRadius(Vector2 globalPosition, float radius, BasePlant ignoredPlant = null)
    {
        var radiusSquared = radius * radius;

        foreach (var plant in CurrentPlantsOnFarm)
        {
            if (plant == null || plant == ignoredPlant || !IsInstanceValid(plant)) continue;

            if (plant.GlobalPosition.DistanceSquaredTo(globalPosition) <= radiusSquared)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPlantPositionAllowed(Vector2 globalPosition, BasePlant ignoredPlant = null)
    {
        return IsPositionPlantableOnTiles(globalPosition) &&
            !IsPositionInsideArea(homeArea, globalPosition) &&
            !IsPositionInsideTrashChest(globalPosition) &&
            !HasBuildingWithinPlacementRadius(globalPosition, BuildingPlacementRadius) &&
            !HasPlantWithinPlacementRadius(globalPosition, PlantPlacementRadius, ignoredPlant);
    }

    public bool IsBuildingPositionAllowed(Vector2 globalPosition, BaseBuilding ignoredBuilding = null)
    {
        return IsPositionPlantableOnTiles(globalPosition) &&
            !IsPositionInsideArea(homeArea, globalPosition) &&
            !IsPositionInsideTrashChest(globalPosition) &&
            !HasPlantWithinPlacementRadius(globalPosition, PlantPlacementRadius) &&
            !HasBuildingWithinPlacementRadius(globalPosition, BuildingPlacementRadius, ignoredBuilding);
    }

    public bool TryFindFreePlantPositionNear(
        Vector2 origin,
        float minDistance,
        float maxDistance,
        int attempts,
        out Vector2 freePosition,
        RandomNumberGenerator random = null)
    {
        freePosition = origin;
        if (attempts <= 0) return false;

        random ??= new RandomNumberGenerator();
        random.Randomize();
        minDistance = Mathf.Max(0.0f, minDistance);
        maxDistance = Mathf.Max(minDistance, maxDistance);

        for (var i = 0; i < attempts; i++)
        {
            var angle = random.RandfRange(0.0f, Mathf.Tau);
            var distance = random.RandfRange(minDistance, maxDistance);
            var candidate = origin + Vector2.FromAngle(angle) * distance;

            if (!IsPlantPositionAllowed(candidate)) continue;

            freePosition = candidate;
            return true;
        }

        return false;
    }

    public bool TryAutoMergePlantsInAura(BaseBuilding source)
    {
        if (source == null || !IsInstanceValid(source)) return false;

        for (var i = 0; i < CurrentPlantsOnFarm.Count; i++)
        {
            var firstPlant = CurrentPlantsOnFarm[i];
            if (!CanPlantBeAutoMerged(firstPlant, source)) continue;

            for (var j = i + 1; j < CurrentPlantsOnFarm.Count; j++)
            {
                var secondPlant = CurrentPlantsOnFarm[j];
                if (!CanPlantBeAutoMerged(secondPlant, source)) continue;
                if (!firstPlant.CanMergeWith(secondPlant)) continue;

                MergePlantsAnimated(secondPlant, firstPlant);
                return true;
            }
        }

        return false;
    }

    private bool CanPlantBeAutoMerged(BasePlant plant, BaseBuilding source)
    {
        return plant != null &&
            IsInstanceValid(plant) &&
            !plantsInAutoMergeAnimation.Contains(plant) &&
            source.IsPlantInsideAura(plant);
    }

    private bool HasBuildingWithinPlacementRadius(Vector2 globalPosition, float radius, BaseBuilding ignoredBuilding = null)
    {
        var radiusSquared = radius * radius;

        foreach (var building in CurrentBuildingsOnFarm)
        {
            if (building == null || building == ignoredBuilding || !IsInstanceValid(building)) continue;

            if (building.GlobalPosition.DistanceSquaredTo(globalPosition) <= radiusSquared)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdatePlantAuraModifiers()
    {
        foreach (var plant in CurrentPlantsOnFarm)
        {
            if (plant == null || !IsInstanceValid(plant)) continue;

            plant.ResetAuraModifiers();

            foreach (var building in CurrentBuildingsOnFarm)
            {
                if (building == null || !IsInstanceValid(building)) continue;

                building.ApplyEffectsTo(plant);
            }
        }
    }

    private void CachePlantableTileLayers()
    {
        plantableTileLayers.Clear();
        blockedTileLayers.Clear();
        CachePlantableTileLayersFrom(GetTree().CurrentScene);
    }

    private void CachePlantableTileLayersFrom(Node node)
    {
        if (node == null) return;

        if (node is TileMapLayer tileMapLayer && node.HasMeta("plantable"))
        {
            if (node.GetMeta("plantable").AsBool())
            {
                plantableTileLayers.Add(tileMapLayer);
            }
            else
            {
                blockedTileLayers.Add(tileMapLayer);
            }
        }

        foreach (var child in node.GetChildren())
        {
            CachePlantableTileLayersFrom(child);
        }
    }

    private bool IsPositionPlantableOnTiles(Vector2 globalPosition)
    {
        foreach (var layer in blockedTileLayers)
        {
            if (IsTileLayerPlantableAt(layer, globalPosition) == false)
            {
                return false;
            }
        }

        foreach (var layer in plantableTileLayers)
        {
            if (IsTileLayerPlantableAt(layer, globalPosition) == true)
            {
                return true;
            }
        }

        return false;
    }

    private static bool? IsTileLayerPlantableAt(TileMapLayer layer, Vector2 globalPosition)
    {
        if (layer == null || !IsInstanceValid(layer)) return null;

        var localPosition = layer.ToLocal(globalPosition);
        var cell = layer.LocalToMap(localPosition);
        var tileData = layer.GetCellTileData(cell);
        if (tileData == null) return null;

        if (HasCustomDataLayer(layer.TileSet, "plantable"))
        {
            var customPlantable = tileData.GetCustomData("plantable");
            if (customPlantable.VariantType == Variant.Type.Bool)
            {
                return customPlantable.AsBool();
            }
        }

        return layer.HasMeta("plantable") ? layer.GetMeta("plantable").AsBool() : null;
    }

    private static bool HasCustomDataLayer(TileSet tileSet, string layerName)
    {
        if (tileSet == null) return false;

        for (var i = 0; i < tileSet.GetCustomDataLayersCount(); i++)
        {
            if (tileSet.GetCustomDataLayerName(i) == layerName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPositionInsideArea(Area2D area, Vector2 globalPosition)
    {
        if (area == null || !IsInstanceValid(area)) return false;

        foreach (var child in area.GetChildren())
        {
            if (child is CollisionShape2D collisionShape &&
                collisionShape.Shape != null &&
                IsPositionInsideShape(collisionShape, globalPosition))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPositionInsideTrashChest(Vector2 globalPosition)
    {
        return trashChest != null &&
            IsInstanceValid(trashChest) &&
            trashChest.ContainsGlobalPoint(globalPosition);
    }

    private static bool IsPositionInsideShape(CollisionShape2D collisionShape, Vector2 globalPosition)
    {
        var localPosition = collisionShape.ToLocal(globalPosition);

        if (collisionShape.Shape is RectangleShape2D rectangleShape)
        {
            return new Rect2(-rectangleShape.Size / 2.0f, rectangleShape.Size).HasPoint(localPosition);
        }

        if (collisionShape.Shape is CircleShape2D circleShape)
        {
            return localPosition.LengthSquared() <= circleShape.Radius * circleShape.Radius;
        }

        return false;
    }

    

}
