using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material selectedTileMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float tileSize = 1.0f; // With Tiny Chess assets, tileSize = 2
    [SerializeField] private float yOffset = 0.2f; // With Tiny Chess assets, yOffset = 1.3
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float dragOffset = 1.5f;
    [SerializeField] private int boardType;
    [SerializeField] private int TILE_COUNT_X = 25;
    [SerializeField] private int TILE_COUNT_Y = 23;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    #region Dimensions
    // Level 1: X = 24, Y = 22
    #endregion
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Vector2Int> enemyMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake(){
        //Spawn level 1
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y, boardType);
        SpawnAllPieces(1);
        PositionalAllPieces();
    }

    private void Update(){
        // Set the camera at runtime if it's not set.
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight"))){
            
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
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = (ContainsValidMove(ref availableMoves, currentHover)) ? highlightMaterial : tileMaterial;
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = selectedTileMaterial;
            }

            // If the piece is a black piece, then highlight its available moves. If the player lands on one of those available moves, trigger logic for player defeat.
            for(int x = 0; x < TILE_COUNT_X; x++){
                for(int y = 0; y < TILE_COUNT_Y; y++){
                    if(chessPieces[x,y] != null){
                        if(chessPieces[x,y].team == 1){
                            enemyMoves = chessPieces[x,y].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            HighlightEnemyMoves();
                        }    
                    }
            }
        }
            
            /// Clicking and dragging functionality
            /// 
            // Select the piece if you click on it.
            if(Input.GetMouseButtonDown(0)){
                if(chessPieces[hitPosition.x, hitPosition.y] != null){
                    // Check turn. Is it your turn?
                    if(chessPieces[hitPosition.x, hitPosition.y].team == 0){
                        currentlyDragging = chessPieces[hitPosition.x,hitPosition.y];

                        // Get a list of where the piece can move
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles(); // Highlight the tiles that the piece can move to
                    }
                }
            }

            // Releasing the mouse button
            if(currentlyDragging != null && Input.GetMouseButtonUp(0)){
                // Save the previous position of the piece
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
                
                // Check if the move is valid
                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if(!validMove){
                    // Reset the piece to its previous position
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                }
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
            /// End of Clicking and dragging functionality
        }
        else{
            // Additional case to ensure hovering and highlighting. Prevents flickering.
            if(currentHover != -Vector2Int.one){
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = (ContainsValidMove(ref availableMoves, currentHover)) ? highlightMaterial : tileMaterial;
                currentHover = -Vector2Int.one;
            }

            // If you are dragging a piece and you release the mouse button, then move the piece to the new position.
            if(currentlyDragging && Input.GetMouseButtonUp(0)){
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
            }
        }

        // If we're dragging a piece, give some visual feedback
        if(currentlyDragging){
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if(horizontalPlane.Raycast(ray, out distance)){
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
            }
        }
    }


    #region Generate Board Functions
    /// <summary>
    /// Generates all the functional tiles for the board. Note: This function is not used for visuals.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="tileCountX"></param>
    /// <param name="tileCountY"></param>
    /// <param name="gridType"> Changes the board generation based on the level.</param>
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
                        if(x < 6 && y < 6){
                            tiles[x,y].SetActive(false);
                        }
                        if(x > 17 && y < 5){
                            tiles[x,y].SetActive(false);
                        }
                        if(x < 6 && y > 5 && y < 8){
                            tiles[x,y].SetActive(false);
                        }
                        if(x < 13 && y > 7 && y < 11){
                            tiles[x,y].SetActive(false);
                        }
                        if(y > 15 && x > 17){
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
    /// <summary>
    /// Generates individual tiles with a mesh filter, mesh renderer, and collider.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns> tileObject </returns>
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
    /// <summary>
    /// Spawns all the pieces for the board. Note: Pending update to spawn pieces based on level.
    /// Ex. chessPieces[x,y] = SpawnSinglePiece(ChessPieceType, team);
    /// Where x and y are the coordinates of the tile.
    /// </summary>
    private void SpawnAllPieces(int gridType){
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        
        int whiteTeam = 0, blackTeam = 1;

        switch(gridType){
            // Base Chess Board
            case 0:
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
            break;

            // Level 1
            case 1:
                chessPieces[9,3] = SpawnSinglePiece(ChessPieceType.White_Knight, whiteTeam);
                chessPieces[4,20] = SpawnSinglePiece(ChessPieceType.Black_King, blackTeam);
                chessPieces[9,15] = SpawnSinglePiece(ChessPieceType.Black_Rook, blackTeam);
                chessPieces[19, 10] = SpawnSinglePiece(ChessPieceType.Black_Rook, blackTeam);

                for(int x = 0; x < TILE_COUNT_X; x++){
                    for(int y = 0; y < TILE_COUNT_Y; y++){
                        if(y > 10 && x == 0){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }  
                        if(y == 11 && x > 0 && x < 14){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y == 23 && x > 0 && x < 17){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(x == 17 && y > 14){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y == 15 && x > 17){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        } 
                        if(x == 24 && y > 5 && y < 15){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y == 5 && x > 17){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(x == 17 && y < 6){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y == 0 && x < 17 && x > 5){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y > 0 && y < 8 && x == 6){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y == 7 && x > 6 && x < 14){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
                        }
                        if(y > 7 && y < 11 && x == 13){
                            chessPieces[x,y] = SpawnSinglePiece(ChessPieceType.White_Pawn, whiteTeam);
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

    /// <summary>
    /// Spawns a single piece. Uses the parameters to determine the type of piece to spawn and the team of the piece.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="team"></param>
    /// <returns> ChessPiece </returns>
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team){
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        
        
        return cp;
    }
    #endregion
    #region Positioning
    /// <summary>
    /// Driver code to the pieces onto the correct part of the board. Calls the helper function to do the actual positioning.
    /// </summary>
    private void PositionalAllPieces(){
        for(int x = 0; x < TILE_COUNT_X; x++){
            for(int y = 0; y < TILE_COUNT_Y; y++){
                if(chessPieces[x,y] != null){
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }
    /// <summary>
    /// Function to determine where a single piece should be placed.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="force"></param>
    private void PositionSinglePiece(int x, int y, bool force = false){
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPosition(GetTileCenter(x, y), force); 
    }
    /// <summary>
    /// Helper function to get the center of a tile for positioning.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns> a vector that determines the center of a tile. </returns>
    private Vector3 GetTileCenter(int x, int y){
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    #endregion
    #region Operations
    /// <summary>
    /// Function to determine the index of a tile. Helps for the raycast.
    /// </summary>
    /// <param name="hitInfo"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Function to move the piece to a new tile.
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns> True unless you try to move another piece on to another piece you own. </returns>
    private bool MoveTo(ChessPiece cp, int x, int y){

        if(!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
            return false;
        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        // Is there another piece on the target position?
        if(chessPieces[x,y] != null){
            ChessPiece otherPiece = chessPieces[x,y];
            if(cp.team == otherPiece.team){
                return false;
            }

            // Remove the other piece if it is an enemy, capture it.
            if(otherPiece.team == 0){
                deadWhites.Add(otherPiece);
                otherPiece.SetScale(Vector3.one*deathSize);
                
                // Original code sets position to the side of the board.
                // otherPiece.SetPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize) 
                //                         - bounds 
                //                         + new Vector3(tileSize / 2, 0, tileSize / 2)
                //                         + (Vector3.forward * deathSpacing) * deadWhites.Count);
            }
            else{
                RemoveEnemyMoves();
                deadBlacks.Add(otherPiece);
                otherPiece.SetScale(Vector3.one*deathSize);
                //otherPiece.SetPosition(new Vector3(otherPiece.transform.position.x, -1f, otherPiece.transform.position.z)); 

                // Original code sets position to the side of the board.
                // otherPiece.SetPosition(new Vector3(-1 * tileSize, yOffset, 8 * tileSize) 
                //                         - bounds 
                //                         + new Vector3(tileSize / 2, 0, tileSize / 2)
                //                         + (Vector3.back * deathSpacing) * deadBlacks.Count);
            }
        }

        


        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);
        return true;
    }
    /// <summary>
    /// Determines if a tile is valid to move to.
    /// </summary>
    /// <param name="moves"></param>
    /// <param name="pos"></param>
    /// <returns> True unless it is invalid. </returns>
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos){
        for(int i = 0; i < moves.Count; i++){
            if(moves[i].x == pos.x && moves[i].y == pos.y){
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Highlight Tiles
    /// <summary>
    /// Handles the highlighting of tiles. Loops through the available moves and highlights the tiles.
    /// </summary>
    private void HighlightTiles(){
        for(int i = 0; i < availableMoves.Count; i++){
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
            tiles[availableMoves[i].x, availableMoves[i].y].GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }
    /// <summary>
    /// Handles the unhighlighting of tiles. Loops through the available moves and unhighlights the tiles.
    /// </summary>
    private void RemoveHighlightTiles(){
        for(int i = 0; i < availableMoves.Count; i++){
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
            tiles[availableMoves[i].x, availableMoves[i].y].GetComponent<MeshRenderer>().material = tileMaterial;
        }

        availableMoves.Clear();
    }
    private void HighlightEnemyMoves(){
        for(int i = 0; i < enemyMoves.Count; i++){
            tiles[enemyMoves[i].x, enemyMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
            tiles[enemyMoves[i].x, enemyMoves[i].y].GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }
    private void RemoveEnemyMoves(){
        for(int i = 0; i < enemyMoves.Count; i++){
            tiles[enemyMoves[i].x, enemyMoves[i].y].layer = LayerMask.NameToLayer("Tile");
            tiles[enemyMoves[i].x, enemyMoves[i].y].GetComponent<MeshRenderer>().material = tileMaterial;
        }

        enemyMoves.Clear();
    }
    #endregion
}
