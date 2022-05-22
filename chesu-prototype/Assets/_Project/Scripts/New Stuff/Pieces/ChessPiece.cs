using UnityEngine;
using System.Collections.Generic;


public enum ChessPieceType{
    None = 0,
    White_Pawn = 1,
    White_Rook = 2,
    White_Knight = 3,
    White_Bishop = 4,
    White_Queen = 5,
    White_King = 6,
    Black_Pawn = 7,
    Black_Rook = 8,
    Black_Knight = 9,
    Black_Bishop = 10,
    Black_Queen = 11,
    Black_King = 12
}
public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Start(){
        // Set the desired rotation. Ensures pieces are facing in the correct direction. Note: This pretty much rotates the black team pieces 180 degrees during runtime.
        transform.rotation = Quaternion.Euler((team == 0) ? Vector3.zero : Vector3.up * 180);
    }
    private void Update(){
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10); // Smoothly move the piece to its desired position
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 5); // Smoothly scale the piece to its desired scale
    }
    /// <summary>
    /// Function to check for available moves on the board.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="tileCountX"></param>
    /// <param name="tileCountY"></param>
    /// <returns></returns>
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY){
        List<Vector2Int> r = new List<Vector2Int>();
        return r;
    }
    /// <summary>
    /// Helper function to set the desired position of the piece.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="force"></param>
    public virtual void SetPosition(Vector3 position, bool force = false){
        desiredPosition = position;
        if(force)
            transform.position = desiredPosition;
    }
    /// <summary>
    /// Helper function to set the desired scale of the piece.
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="force"></param>
    public virtual void SetScale(Vector3 scale, bool force = false){
        desiredScale = scale;
        if(force)
            transform.localScale = desiredScale;
    }
}
