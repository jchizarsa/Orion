using UnityEngine;

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
    private Vector3 desiredScale;
}
