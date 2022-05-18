using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>ScriptableUnit</c> derives from ScriptableObject. It is a base class for all units for certain attributes.
/// - Faction Faction
/// - BaseUnit UnitPrefab
/// </summary>
[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
}

public enum Faction{
    Hero = 0,
    Enemy = 1
}