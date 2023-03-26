using UnityEngine;

namespace ChessPieces
{
    public enum ChessPieceType
    {
        None = 0,
        Pawn = 1,
        Rook = 2,
        Knight = 3,
        Bishop = 4,
        Queen = 5,
        King = 6
    }

    public class ChessPiece : MonoBehaviour
    {
        public int Team;
        public int CurrentX;
        public int CurrentY;
        public ChessPieceType Type;

        private Vector3 _desiredPosition;
        private Vector3 _desiredScale;
    }
}