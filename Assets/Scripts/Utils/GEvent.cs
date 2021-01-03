using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GEvent<T> where T : GEvent<T>
{

    public string Description;
    public bool UserEvent;
    private bool hasFired;
    public delegate void EventListener(T info);
    private static event EventListener listeners;

    public GEvent()
    {
        UserEvent = true;
    }


    public static void RegisterListener(EventListener listener)
    {
        listeners += listener;
    }

    public static void UnregisterListener(EventListener listener)
    {
        listeners -= listener;
    }

    public void FireEvent()
    {
        if (hasFired)
        {
            throw new Exception("This event has already fired, to prevent infinite loops you can't refire an event");
        }
        hasFired = true;
        if (listeners != null)
        {
            listeners(this as T);
        }
    }
}

public class DebugEvent : GEvent<DebugEvent>{}

public class OnDiceUsed : GEvent<OnDiceUsed> { public Card Card; public Dice Dice; }

public class OnDamageReceived : GEvent<OnDamageReceived> { public int damage; }

public class OnRecoveryHealth : GEvent<OnRecoveryHealth> { public int recoveryHealth; }

public class OnSplitDice : GEvent<OnSplitDice> { public List<int> numbers; }

public class OnAddDodgeDice : GEvent<OnAddDodgeDice> {  }

public class OnAddShieldDice : GEvent<OnAddShieldDice> { public int amount; }


public class OnRerollDice : GEvent<OnRerollDice> { public int rerollRemains; }
