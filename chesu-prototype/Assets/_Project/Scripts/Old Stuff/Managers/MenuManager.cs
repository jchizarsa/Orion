using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Class <c>MenuManager</c> controls the tentative UI.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;

    private void Awake()
    {
        Instance = this;
    }

    /**
     * @desc Shows the information of a tile.
     * @param {Tile} tile - The tile to show the information of.
     */
    public void ShowTileInfo(Tile tile){
        // Base case if no tile is selected.
        if(tile == null){
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        // Show the tile info.
        _tileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.TileName;
        _tileObject.SetActive(true);

        // Show unit info if the tile is currently being occupied.
        if(tile.OccupiedUnit){
            _tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    /**
     * @desc Shows the information of a hero.
     * @param {BaseHero} hero - The hero to show the information of.
     */
    public void ShowSelectedHero(BaseHero hero){
        // Base case if no hero is clicked.
        if(hero == null){
            _selectedHeroObject.SetActive(false);
            return;
        }
        
        // Show the hero info.
        _selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }
}
