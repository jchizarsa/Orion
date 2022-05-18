using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Abstract Class <c>Tile</c> is the base that all tiles inherit from.
/// Has the basic functionality of a tile.
/// - Highlightable
/// - Unit setting for movement
/// - Walkability
/// </summary>
public abstract class Tile : MonoBehaviour
{
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    private Tile _otherTile;
    private Vector2 _mousePos;
    private bool _heroSelected = false;

    /**
     * @desc Initializes the color of a tile based on if the tile is an 'offset' tile.
     * @param {bool} isOffset - Whether the tile is an 'offset' tile.
     */
    public virtual void Init(int x, int y){
        
    }

    private void Awake(){
        
    }
    private void Update(){
        _mousePos = (Vector2)Input.mousePosition;
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

    /**
     * @desc Registers mouse input and has several actions based on the tile and the unit
     * @param N/A
     */
    void OnMouseDown(){
        // Base case determined by the GameManager to see if it is the player's turn.
        if(GameManager.Instance.GameState != GameState.HeroesTurn) return;

        // Variable to get the position of a tile.
        var selectedTile = GridManager.Instance.GetTileAtPosition((Vector2)this.transform.position);

        // Checks if the tile is occupied. OccupiedUnit = null if the tile is not occupied by a unit.
        if(OccupiedUnit != null){
            // Checks if the unit is the player's unit.
            if(OccupiedUnit.Faction == Faction.Hero){
                #region Hero selection and deselection functionalities present in the region below.
                if(!_heroSelected){
                    UnitsManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
                    _heroSelected = true;
                    
                }else{
                    UnitsManager.Instance.SetSelectedHero(null);
                    _heroSelected = false;
                }
                #endregion
            }   
            else{
                // If the unit is not a hero, it is an enemy unit.
                if(UnitsManager.Instance.SelectedHero != null) {
                    Debug.Log("Moving to tile: " + selectedTile + "...");
                    var enemy = (BaseEnemy)OccupiedUnit; // Get the unit currently occupying the tile. Note: it should be an enemy.
                    Debug.Log("Enemy captured: " + enemy + "!");

                    // *** Behavior for when the enemy is to be captured. ***
                    Destroy(enemy.gameObject);
                    // ******************************************************

                    MoveHero();
                    _heroSelected = false;
                }
            }
        }
        else{
            // Check if the tile is unoccupied.
            if(UnitsManager.Instance.SelectedHero != null){
                Debug.Log("Moving to tile: " + selectedTile + "...");

                // Check if the tile is walkable
                if(selectedTile.Walkable){
                    MoveHero();
                    Debug.Log("Movement successful!");
                }else{
                    Debug.Log("Can't move there! Please try again.");
                }
                _heroSelected = false;
            }
        }
        
        
    }

    /**
     * @desc Performs the movement of a unit.
     * @param {BaseHero} hero - The hero that is moving.
     */
    public void SetUnit(BaseUnit unit){
        if(unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    /**
     * @desc Specifically move the hero utilizing SetUnit() and reset the currently selected hero.
     * @param N/A
     */
    private void MoveHero(){
        SetUnit(UnitsManager.Instance.SelectedHero);
        UnitsManager.Instance.SetSelectedHero(null);
    }
}
