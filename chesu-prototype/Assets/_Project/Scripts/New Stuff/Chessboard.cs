using System;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material selectedTileMaterial;
    [SerializeField] private float tileSize = 1.0f; // With Tiny Chess assets, tileSize = 2
    [SerializeField] private float yOffset = 0.2f; // With Tiny Chess assets, yOffset = 1.3
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    #region Dimensions
    // Level 1: X = 24, Y = 22
    #endregion
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake(){
        //Spawn level 1
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y, 0);
        SpawnAllPieces();
        PositionalAllPieces();
    }

    private void Update(){
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover"))){
            // Get indices of the tiles that are hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If you start hovering a tile that is not the current hover, then change the current hover
            if(currentHover == -Vector2Int.one){
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = selectedTileMaterial;
            }
            // Otherwise, change the previous hover tile
            if(currentHover != hitPosition){
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = selectedTileMaterial;
            }
        }
        else{
            if(currentHover != -Vector2Int.one){
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                currentHover = -Vector2Int.one;
            }
            else{

            }
        }
    }


    #region Generate Board Functions
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY, int gridType){
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX/2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];

        switch(gridType){
            // Base Chess Board
            case 0:
                for(int x = 0; x < 8; x++){
                    for(int y = 0; y < 8; y++){
                        tiles[x,y] = GenerateSingleTile(tileSize, x, y);
                    }
                }
                break;
            // Level 1
            case 1:
                for(int x = 0; x < tileCountX; x++){
                    for(int y = 0; y < tileCountY; y++){
                        tiles[x,y] = GenerateSingleTile(tileSize, x, y);   
                        if(x < 7 && y < 5){
                            tiles[x,y].SetActive(false);
                        }
                        if(x > 16 && y < 5){
                            tiles[x,y].SetActive(false);
                        }
                        if(x < 7 && y == 5){
                            tiles[x,y].SetActive(false);
                        }
                        if(x < 14 && y > 5 && y < 12){
                            tiles[x,y].SetActive(false);
                        }
                        if(y > 11 && y < 15){
                            // do nothing here, just let it spawn
                        }
                        if(y > 14 && x > 16){
                            tiles[x,y].SetActive(false);
                        }
                    }
        }
            break;
            // Level 2
            case 2:
            break;
            // Level 3
            case 3:
            break;
            // Level 4
            case 4:
            break;
            // Level 5
            case 5:
            break;
            // Level 6
            case 6:
            break;
        }

    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        // Generate the mesh
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;


        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }
    #endregion
    #region Spawning pieces
    private void SpawnAllPieces(){
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        //White Team
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.White_Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.White_Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.White_Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.White_Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.White_King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.White_Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.White_Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.White_Rook, whiteTeam);
        for(int i = 0; i < TILE_COUNT_X; i++){
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
        }

        //Black Team
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Black_Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Black_Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Black_Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Black_King, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.Black_Queen, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Black_Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Black_Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Black_Rook, blackTeam);
        for(int i = 0; i < TILE_COUNT_X; i++){
            chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Black_Pawn, blackTeam);
        }

    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team){
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        
        
        return cp;
    }
    #endregion
    #region Positioning
    private void PositionalAllPieces(){
        for(int x = 0; x < TILE_COUNT_X; x++){
            for(int y = 0; y < TILE_COUNT_Y; y++){
                if(chessPieces[x,y] != null){
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }
    private void PositionSinglePiece(int x, int y, bool force = false){
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].transform.position = GetTileCenter(x, y);
    }
    private Vector3 GetTileCenter(int x, int y){
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    #endregion
    #region Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo){
        for(int x = 0; x < TILE_COUNT_X; x++){
            for(int y = 0; y < TILE_COUNT_Y; y++){
                if(tiles[x,y] == hitInfo){
                    return new Vector2Int(x, y);
                }
            }
        }

        return -Vector2Int.one; // Invalid
    }
    #endregion
}
