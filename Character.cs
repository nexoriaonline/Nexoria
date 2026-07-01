using AutoFarm;
using Godot;
using System;

public partial class Character : CharacterBody2D
{
	[Export]
	public float Speed = 200;
	[Export]
	public float GatherDistance = 25;
	[Export]
	public bool CanWander = true;
	[Export]
	public float WanderRadius = 160;
	[Export]
	public float MinIdleTimeBeforeWander = 2;
	[Export]
	public float MaxIdleTimeBeforeWander = 5;
	[Export]
	public float WanderTargetDistance = 12;
	public PlayerStates CurrentPlayerState = PlayerStates.Idle;

	private NavigationAgent2D agent;
	private readonly RandomNumberGenerator random = new RandomNumberGenerator();
	private Vector2 homePosition;
	private Vector2 wanderTargetPosition;
	private float idleActivityTimer;

	public Node2D CurrentTarget;

	public event Action<PlayerStates, PlayerStates> OnStateChanged;

	public FarmingController FarmingController;
	public EconomyController EconomyController;

	public override void _Ready()
	{
		agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		FarmingController = GetNode<FarmingController>("/root/Node2D/FarmingController");
		EconomyController = GetNode<EconomyController>("/root/Node2D/EconomyController");
		homePosition = GlobalPosition;
		random.Randomize();
		ScheduleNextIdleActivity();
	}
	
	public void DoHarvestAction()
	{
		var plant = CurrentTarget as BasePlant;
		if (plant != null && plant.IsFullyGrowed)
		{
			plant.DoHarvest();
			if (!plant.IsFullyGrowed)
			{
				FarmingController.RemoveReadyPlant(plant);
				CurrentTarget = null;
				ChangePlayerState(PlayerStates.Idle);
			}
		}
		else
		{
			CurrentTarget = null;
			ChangePlayerState(PlayerStates.Idle);
		}
	}


	public void ChangePlayerState(PlayerStates newState)
	{
		if (CurrentPlayerState == newState)
			return;

		PlayerStates oldState = CurrentPlayerState;

		CurrentPlayerState = newState;

		if (newState == PlayerStates.Idle)
		{
			ScheduleNextIdleActivity();
		}

		OnStateChanged?.Invoke(oldState, newState);
	}

	public void SearchForPlants()
	{
		if(FarmingController.PlantsReadyToHarvest.Count > 0)
		{
			CurrentTarget = FarmingController.PlantsReadyToHarvest[0];
			agent.TargetPosition = CurrentTarget.GlobalPosition;
		}
	}

	private void ScheduleNextIdleActivity()
	{
		idleActivityTimer = random.RandfRange(MinIdleTimeBeforeWander, MaxIdleTimeBeforeWander);
	}

	private void UpdateIdleActivity(double delta)
	{
		if (!CanWander) return;

		idleActivityTimer -= (float)delta;
		if (idleActivityTimer > 0) return;

		StartWandering();
	}

	private void StartWandering()
	{
		wanderTargetPosition = GetRandomWanderPoint();
		agent.TargetPosition = wanderTargetPosition;
		ChangePlayerState(PlayerStates.Wandering);
	}

	private Vector2 GetRandomWanderPoint()
	{
		Vector2 randomDirection = Vector2.FromAngle(random.RandfRange(0, Mathf.Tau));
		float randomDistance = random.RandfRange(WanderRadius * 0.35f, WanderRadius);
		return homePosition + randomDirection * randomDistance;
	}

	private bool IsCloseToCurrentTarget()
	{
		return CurrentTarget != null && GlobalPosition.DistanceTo(CurrentTarget.GlobalPosition) <= GatherDistance;
	}

	public override void _PhysicsProcess(double delta)
	{

		switch (CurrentPlayerState)
		{
			case PlayerStates.Idle:
				{
					Velocity = Vector2.Zero;
					MoveAndSlide();
					SearchForPlants();
					if(CurrentTarget != null && !IsCloseToCurrentTarget())
					{                      
						ChangePlayerState(PlayerStates.Moving);
					}
					if(IsCloseToCurrentTarget())
					{
						ChangePlayerState(PlayerStates.GatheringPlant);
					}
					if(CurrentTarget == null)
					{
						UpdateIdleActivity(delta);
					}
					break;
				}
			case PlayerStates.GatheringPlant:
				{
					break;
				}
			case PlayerStates.GatheringOre:
				{
					break;
				}
			case PlayerStates.Moving:
				{
					if (IsCloseToCurrentTarget())
					{
						Velocity = Vector2.Zero;
						MoveAndSlide();
						ChangePlayerState(PlayerStates.GatheringPlant);
						return;
					}
					if(CurrentTarget == null || agent.IsNavigationFinished())
					{
						ChangePlayerState(PlayerStates.Idle);
						return;
					}
					Vector2 nextPoint = agent.GetNextPathPosition();
					Vector2 direction =
						(nextPoint - GlobalPosition).Normalized();
					Velocity = direction * Speed;
					MoveAndSlide();
					break;
				}
			case PlayerStates.Wandering:
				{
					SearchForPlants();
					if (CurrentTarget != null)
					{
						ChangePlayerState(PlayerStates.Moving);
						return;
					}
					if (GlobalPosition.DistanceTo(wanderTargetPosition) <= WanderTargetDistance || agent.IsNavigationFinished())
					{
						Velocity = Vector2.Zero;
						MoveAndSlide();
						ChangePlayerState(PlayerStates.Idle);
						return;
					}

					Vector2 nextPoint = agent.GetNextPathPosition();
					Vector2 direction =
						(nextPoint - GlobalPosition).Normalized();
					Velocity = direction * Speed;
					MoveAndSlide();
					break;
				}
		}
			

		
	}
}
