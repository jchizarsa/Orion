using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>BaseUnit</c> is a base class for all units.
/// Has the following attributes:
/// - string UnitName
/// - Tile OccupiedTile
/// - Faction Faction
/// </summary>
public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
}
