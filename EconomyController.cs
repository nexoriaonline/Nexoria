using Godot;
using System;

public partial class EconomyController : Node2D
{
    [Export]
    public float CurrentPlayerGold = 55550;
    public Action<float> OnGoldChanged;

    public void AddGold(float amount)
    {
        CurrentPlayerGold += amount;
        OnGoldChanged?.Invoke(CurrentPlayerGold);
    }

    public bool RemoveGold(float amount)
    {
        if (CurrentPlayerGold - amount < 0) return false;
        CurrentPlayerGold -= amount;    
        OnGoldChanged?.Invoke(CurrentPlayerGold); 
        return true;
    }
}
