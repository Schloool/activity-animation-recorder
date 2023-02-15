using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationItemList<T> : ScriptableObject where T : ChoiceObject
{
    public List<T> items;
}