using AutoFarm;
using Godot;
using System;

public partial class Animator : AnimatedSprite2D
{
    [Export]
    private Character _character;
    [Export]
    public AnimatedSprite2D AttackAnimation;

    public override void _Ready()
    {
        base._Ready();
        _character.OnStateChanged += PlayerChangedState;
        AttackAnimation.AnimationLooped += _character.DoHarvestAction;
        AttackAnimation.Hide();
        AttackAnimation.Stop();
    }

    public override void _Process(double delta)
    {
        UpdateDirection();
    }

    private void UpdateDirection()
    {
        if (_character.Velocity.X >= 0)
        {
            FlipH = false;
        }
        else if (_character.Velocity.X < 0)
        {
            FlipH = true;
        }
    }


    private void PlayerChangedState(PlayerStates oldState, PlayerStates newState)
    {
        if(oldState == PlayerStates.GatheringPlant)
        {
            AttackAnimation.Stop();
            AttackAnimation.Hide();
        }

        switch (newState)
        {
            case PlayerStates.Idle:
                {
                    AttackAnimation.Stop();
                    AttackAnimation.Hide();
                    Play("Idle");
                    break;
                }
            case PlayerStates.Moving:
            case PlayerStates.Wandering:
                {
                    AttackAnimation.Stop();
                    AttackAnimation.Hide();
                    Play("WalkRight");
                    break;
                }
            case PlayerStates.GatheringPlant:
                {
                    Play("Idle");
                    AttackAnimation.Show();
                    AttackAnimation.Play("Gather");
                    break;
                }
        }
    }
}
