using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>GrassTile</c> is a tile that can be occupied by a unit. Changes color to be a checker board pattern.
/// </summary>
public class GrassTile : Tile
{
    [SerializeField] private Color _baseColor, _offsetColor;

    public override void Init(int x, int y){
        
        // Different color for offset tiles. (Checkerboard pattern)
        var isOffset = (x + y) % 2 == 1;
        _renderer.color = isOffset ? _offsetColor : _baseColor; 
    }
}
