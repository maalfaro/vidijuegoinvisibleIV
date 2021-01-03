using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Card : ScriptableObject
{

    public string Title;
    public string Description;
    public Color Color;
    public Condition Condition;
    public float movement = 500f;

    public abstract void Initialize();

    public abstract void Use(int number);

    /// <summary>
    /// Devuelve true si se cumple la condicion, sino devuelve false
    /// </summary>
    /// <returns></returns>
    public bool CheckCondition(int number) {
        if (Condition == null) return true;
        if(Condition.type.Equals(ConditionTypes.None)) return true;

        switch (Condition.type) {
            case ConditionTypes.Even: return number % 2 == 0;
            case ConditionTypes.Odd: return number % 2 != 0;
            case ConditionTypes.Max: return number <= Condition.number;
            case ConditionTypes.Min: return number >= Condition.number;
        }

        return true;
    }

}

[System.Serializable]
public class Condition {

    public int number;
    public ConditionTypes type;
}

public enum ConditionTypes { None, Even, Odd, Min, Max }
