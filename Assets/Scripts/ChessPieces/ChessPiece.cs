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
        private uint _hp = 100;

        private GameObject _hpTextMesh;

        public virtual List<Vector2Int> GetSpecialAttack1Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            return null;
        }
        
        public virtual List<Vector2Int> GetSpecialAttack2Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            return null;
        }
        
        public virtual List<Vector2Int> GetSpecialAttack3Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            return null;
        }

        public void DamagePiece()
        {
            _hp--;
        }

        public uint GetHp()
        {
            return _hp;
        }

        public void SetHp(uint hp)
        {
            _hp = hp;
        }

        public bool IsDead()
        {
            return _hp == 0;
        }

        public bool IsHpLabelVisible()
        {
            return _hpTextMesh.activeSelf;
        }

        public void HpVisibility(bool value)
        {
            if (value) _hpTextMesh.GetComponentInChildren<TextMesh>().text = _hp.ToString();
            _hpTextMesh.SetActive(value);
        }
        
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
            _hpTextMesh = GetComponentInChildren<TextMesh>().gameObject;
            _hpTextMesh.SetActive(false);
            transform.rotation = Quaternion.Euler(Team == 0 ? new Vector3(0, 90, 0) : new Vector3(0, 270, 0));
        }

        private void Update()
        {
            if (Confrontation.GetCurrentAttacking() != null) return;
            Transform tr;
            (tr = transform).position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * 10f);
            transform.localScale = Vector3.Lerp(tr.localScale, _desiredScale, Time.deltaTime * 10f);
        }

        public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
        {
            return null;
        }

        public virtual List<Vector2Int> GetAvailableMovesInConfrontation(ref ChessPiece[,] board, int tileCountX,
            int tileCountY)
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

        public virtual void SetConfrontationAim(Vector3 position)
        {
            StartCoroutine(ReachAimInXSeconds(position, 0.2f));
        }

        private IEnumerator ReachAimInXSeconds(Vector3 position, float seconds)
        {
            var currentPos = transform.position;
            var t = 0f;
            while(t < 1)
            {
                t += Time.deltaTime / seconds;
                transform.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }
        }

        public IEnumerator SetPositionAfterSeconds(Vector3 position, int seconds)
        {
            yield return new WaitForSeconds(seconds);
            SetPosition(position, true);
        }
    }
}