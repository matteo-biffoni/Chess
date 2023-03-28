using System;
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

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * 10f);
            transform.localScale = Vector3.Lerp(transform.localScale, _desiredScale, Time.deltaTime * 10f);
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
    }
}