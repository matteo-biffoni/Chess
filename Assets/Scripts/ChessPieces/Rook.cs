using System.Collections.Generic;
using UnityEngine;

namespace ChessPieces
{
    public class Rook : ChessPiece
    {
        public override List<Vector2Int> GetPossibleMovesFromPosition(ref ChessPiece[,] board, int tileCountX, int tileCountY, Vector2Int fromPosition)
        {
            var r = new List<Vector2Int>();
            for (var i = fromPosition.y - 1; i >= 0; i--)
            {
                if (board[fromPosition.x, i] == null)
                    r.Add(new Vector2Int(fromPosition.x, i));
                if (board[fromPosition.x, i] != null)
                {
                    if (board[fromPosition.x, i].Team != Team)
                        r.Add(new Vector2Int(fromPosition.x, i));
                    break;
                }
            }
            for (var i = fromPosition.y + 1; i < tileCountY; i++)
            {
                if (board[fromPosition.x, i] == null)
                    r.Add(new Vector2Int(fromPosition.x, i));
                if (board[fromPosition.x, i] != null)
                {
                    if (board[fromPosition.x, i].Team != Team)
                        r.Add(new Vector2Int(fromPosition.x, i));
                    break;
                }
            }
            for (var i = fromPosition.x - 1; i >= 0; i--)
            {
                if (board[i, fromPosition.y] == null)
                    r.Add(new Vector2Int(i, fromPosition.y));
                if (board[i, fromPosition.y] != null)
                {
                    if (board[i, fromPosition.y].Team != Team)
                        r.Add(new Vector2Int(i, fromPosition.y));
                    break;
                }
            }
            for (var i = fromPosition.x + 1; i < tileCountX; i++)
            {
                if (board[i, fromPosition.y] == null)
                    r.Add(new Vector2Int(i, fromPosition.y));
                if (board[i, fromPosition.y] != null)
                {
                    if (board[i, fromPosition.y].Team != Team)
                        r.Add(new Vector2Int(i, fromPosition.y));
                    break;
                }
            }
            return r;        }

        public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var i = CurrentY - 1; i >= 0; i--)
            {
                if (board[CurrentX, i] == null)
                    r.Add(new Vector2Int(CurrentX, i));
                if (board[CurrentX, i] != null)
                {
                    if (board[CurrentX, i].Team != Team)
                        r.Add(new Vector2Int(CurrentX, i));
                    break;
                }
            }
            for (var i = CurrentY + 1; i < tileCountY; i++)
            {
                if (board[CurrentX, i] == null)
                    r.Add(new Vector2Int(CurrentX, i));
                if (board[CurrentX, i] != null)
                {
                    if (board[CurrentX, i].Team != Team)
                        r.Add(new Vector2Int(CurrentX, i));
                    break;
                }
            }
            for (var i = CurrentX - 1; i >= 0; i--)
            {
                if (board[i, CurrentY] == null)
                    r.Add(new Vector2Int(i, CurrentY));
                if (board[i, CurrentY] != null)
                {
                    if (board[i, CurrentY].Team != Team)
                        r.Add(new Vector2Int(i, CurrentY));
                    break;
                }
            }
            for (var i = CurrentX + 1; i < tileCountX; i++)
            {
                if (board[i, CurrentY] == null)
                    r.Add(new Vector2Int(i, CurrentY));
                if (board[i, CurrentY] != null)
                {
                    if (board[i, CurrentY].Team != Team)
                        r.Add(new Vector2Int(i, CurrentY));
                    break;
                }
            }
            return r;
        }

        public override List<Vector2Int> GetSpecialAttack1Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var i = 0; i < tileCountX; i++)
                r.Add(new Vector2Int(i, cell.y));
            return r;
        }

        public override List<Vector2Int> GetSpecialAttack2Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var j = 0; j < tileCountY; j++)
                r.Add(new Vector2Int(cell.x, j));
            return r;
        }

        public override List<Vector2Int> GetAvailableMovesInConfrontation(ref ChessPiece[,] board, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var i = CurrentY - 1; i >= 0; i--)
                r.Add(new Vector2Int(CurrentX, i));
            for (var i = CurrentY + 1; i < tileCountY; i++)
                r.Add(new Vector2Int(CurrentX, i));
            for (var i = CurrentX - 1; i >= 0; i--)
                r.Add(new Vector2Int(i, CurrentY));
            for (var i = CurrentX + 1; i < tileCountX; i++)
                r.Add(new Vector2Int(i, CurrentY));
            return r;
        }
    }
}
