using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollCard : Card
{
    private int rerollRemains;

    public RerollCard(int reamins)
    {
        rerollRemains = reamins;
        Description = $"Reroll a Dice\n ({reamins} uses)";
    }

    public override void Use(int number)
    {
        Debug.LogError($"Reroll de dado");
        rerollRemains--;
        new OnRerollDice() { rerollRemains = rerollRemains}.FireEvent();
    }
}

