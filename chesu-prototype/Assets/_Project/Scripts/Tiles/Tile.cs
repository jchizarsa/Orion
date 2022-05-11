using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    /**
     * @desc Initializes the color of a tile based on if the tile is an 'offset' tile.
     * @param {bool} isOffset - Whether the tile is an 'offset' tile.
     */
    public virtual void Init(int x, int y){
        
    }

    #region Highlight
    /**
     * @desc Highlights the tile when the mouse is over it.
     */
    void OnMouseEnter(){
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }
    void OnMouseExit(){
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }
    #endregion

    void OnMouseDown(){
        if(GameManager.Instance.GameState != GameState.HeroesTurn) return;

        if(OccupiedUnit != null){
            if(OccupiedUnit.Faction == Faction.Hero) UnitsManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            else{
                if(UnitsManager.Instance.SelectedHero != null) {
                    var enemy = (BaseEnemy)OccupiedUnit;
                    Destroy(enemy.gameObject);
                    UnitsManager.Instance.SetSelectedHero(null);
                }
            }
        }
        else{
            if(UnitsManager.Instance.SelectedHero != null){
                SetUnit(UnitsManager.Instance.SelectedHero);
                UnitsManager.Instance.SetSelectedHero(null);
            }
        }
    }

    public void SetUnit(BaseUnit unit){
        if(unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}
