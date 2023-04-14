using System.Collections;
using System.Collections.Generic;
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
        private Vector3 _desiredScale = new (0.08f, 0.08f, 0.08f);
        
        private static string ChessPieceTypeToString(ChessPieceType cpt)
        {
            return cpt switch
            {
                ChessPieceType.None => "",
                ChessPieceType.Pawn => "Pawn",
                ChessPieceType.Rook => "Rook",
                ChessPieceType.Knight => "Knight",
                ChessPieceType.Bishop => "Bishop",
                ChessPieceType.Queen => "Queen",
                ChessPieceType.King => "King",
                _ => ""
            };
        }

        public override string ToString()
        {
            return (Team == 0 ? "White" : "Black") + " " + ChessPieceTypeToString(Type) + " in " +
                   MovesUI.GetLabelFromPosition(new Vector2Int(CurrentX, CurrentY));
        }

        private void Start()
        {
            transform.rotation = Quaternion.Euler(Team == 0 ? new Vector3(0, 90, 0) : new Vector3(0, 270, 0));
        }

        private void Update()
        {
            Transform tr;
            (tr = transform).position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * 10f);
            transform.localScale = Vector3.Lerp(tr.localScale, _desiredScale, Time.deltaTime * 10f);
        }

        public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
        {
            return null;
        }

        public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList,
            ref List<Vector2Int> availableMoves)
        {
            return SpecialMove.None;
        }
        public virtual void SetScale(Vector3 scale, bool force = false)
        {
            _desiredScale = scale;
            if (force)
                transform.localScale = _desiredScale;
        }
        public virtual void SetPosition(Vector3 position, bool force = false)
        {
            _desiredPosition = position;
            if (force)
                transform.position = _desiredPosition;
        }

        public IEnumerator SetPositionAfterSeconds(Vector3 position, int seconds)
        {
            yield return new WaitForSeconds(seconds);
            SetPosition(position, true);
        }
    }
}