using System.Collections.Generic;
using UnityEngine;

namespace ChessPieces
{
    public class Queen : ChessPiece
    {
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
            for (int x = CurrentX + 1, y = CurrentY + 1; x < tileCountX && y < tileCountY; x++, y++)
            {
                if (board[x, y] == null)
                    r.Add(new Vector2Int(x, y));
                else
                {
                    if (board[x, y].Team != Team)
                        r.Add(new Vector2Int(x, y));
                    break;
                }
            }
            for (int x = CurrentX - 1, y = CurrentY + 1; x >= 0 && y < tileCountY; x--, y++)
            {
                if (board[x, y] == null)
                    r.Add(new Vector2Int(x, y));
                else
                {
                    if (board[x, y].Team != Team)
                        r.Add(new Vector2Int(x, y));
                    break;
                }
            }
            for (int x = CurrentX + 1, y = CurrentY - 1; x < tileCountX && y >= 0; x++, y--)
            {
                if (board[x, y] == null)
                    r.Add(new Vector2Int(x, y));
                else
                {
                    if (board[x, y].Team != Team)
                        r.Add(new Vector2Int(x, y));
                    break;
                }
            }
            for (int x = CurrentX - 1, y = CurrentY - 1; x >= 0 && y >= 0; x--, y--)
            {
                if (board[x, y] == null)
                    r.Add(new Vector2Int(x, y));
                else
                {
                    if (board[x, y].Team != Team)
                        r.Add(new Vector2Int(x, y));
                    break;
                }
            }
            return r;
        }

        /*public override int ComputeDamageInPath(Vector2Int from, Vector2Int to, bool[,] fired)
        {
            var counter = 0;
            // Movimento sugli assi
            if (from.x == to.x || from.y == to.y)
            {
                if (from.x != to.x)
                {
                    for (var i = Mathf.Min(from.x, to.x); i <= Mathf.Max(from.x, to.x); i++)
                        if (fired[i, from.y])
                            counter++;
                }
                else
                {
                    for (var j = Mathf.Min(from.y, to.y); j <= Mathf.Max(from.y, to.y); j++)
                        if (fired[from.x, j])
                            counter++;
                }
            }
            // Movimento in diagonale
            else
            {
                for (var i = Mathf.Min(from.x, to.x); i <= Mathf.Max(from.x, to.x); i++)
                    for (var j = Mathf.Min(from.y, to.y); j <= Mathf.Max(from.y, to.y); j++)
                        if (fired[i, j])
                            counter++;
            }
            return counter;
        }*/

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
            for (int x = CurrentX + 1, y = CurrentY + 1; x < tileCountX && y < tileCountY; x++, y++)
                r.Add(new Vector2Int(x, y));
            for (int x = CurrentX - 1, y = CurrentY + 1; x >= 0 && y < tileCountY; x--, y++)
                r.Add(new Vector2Int(x, y));
            for (int x = CurrentX + 1, y = CurrentY - 1; x < tileCountX && y >= 0; x++, y--)
                r.Add(new Vector2Int(x, y));
            for (int x = CurrentX - 1, y = CurrentY - 1; x >= 0 && y >= 0; x--, y--)
                r.Add(new Vector2Int(x, y));
            return r;
        }
    }
}
