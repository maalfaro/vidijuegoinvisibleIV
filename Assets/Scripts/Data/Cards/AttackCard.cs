using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card
{

    public AttackCard()
    {
        Description = "Attack card";
    }

    public override void Use(int number)
    {
        Debug.LogError($"Hacer {number} de daño al enemigo");
    }
}
