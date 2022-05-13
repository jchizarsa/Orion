using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Class <c>GridManager</c> controls the grid generation and determining tile locations.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _grassTile, _mountainTile;
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;
    
    void Awake(){
        Instance = this;
    }

    /**
     * @desc Generates a grid of tiles.
     */
    public void GenerateGrid(){
        _tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < _width; x++){
            for(int y = 0; y < _height; y++){
                #region Randomly generate a tile/Biome logic
                var randomTile = Random.Range(0,6) == 3 ? _mountainTile : _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                
                spawnedTile.Init(x,y);

                _tiles[new Vector2(x,y)] = spawnedTile;
                #endregion
            }
        }

        // Center the camera to the middle of the grid.
        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height/2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    /**
     * @desc Gets the player spawning tile at a "random" location (Update required).
     * @param N/A
     * @return The tile at the specified location.
     */
    public Tile GetHeroSpawnTile(){
        return _tiles.Where(t=>t.Key.x < _width/2 && t.Value.Walkable).OrderBy(t=>Random.value).First().Value;
    }

    /**
     * @desc Gets the enemy spawning tile at a "random" location (Update required).
     * @param N/A
     * @return The tile at the specified location.
     */
    public Tile GetEnemySpawnTile(){
        return _tiles.Where(t=>t.Key.x > _width/2 && t.Value.Walkable).OrderBy(t=>Random.value).First().Value;
    }

    /**
     * @desc Returns the tile at the given position.
     * @param {Vector2} pos - The position of the tile.
     * @return {Tile} - The tile at the given position. : 
     *         {Null} - If no tile is found at the given position.
     */
    public Tile GetTileAtPosition(Vector2 pos){
        if(_tiles.TryGetValue(pos, out var tile)){
            return tile;
        }else
            return null;
    }
}
