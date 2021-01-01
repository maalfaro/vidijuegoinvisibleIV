using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    void Use(int number);
}

public abstract class Card : ICard
{

    public string Description;

    public abstract void Use(int number);

}
