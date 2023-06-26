using System.Collections.Generic;
using UnityEngine;

namespace ChessPieces
{
    public class Queen : ChessPiece
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
            for (int x = fromPosition.x + 1, y = fromPosition.y + 1; x < tileCountX && y < tileCountY; x++, y++)
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
            for (int x = fromPosition.x - 1, y = fromPosition.y + 1; x >= 0 && y < tileCountY; x--, y++)
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
            for (int x = fromPosition.x + 1, y = fromPosition.y - 1; x < tileCountX && y >= 0; x++, y--)
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
            for (int x = fromPosition.x - 1, y = fromPosition.y - 1; x >= 0 && y >= 0; x--, y--)
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

        public override List<Vector2Int> GetSpecialAttack1Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var i = 0; i < tileCountX; i++)
                r.Add(new Vector2Int(i, cell.y));
            for (var j = 0; j < tileCountY; j++)
                r.Add(new Vector2Int(cell.x, j));
            return r;
        }
        public override List<Vector2Int> GetSpecialAttack2Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (int x = cell.x + 1, y = cell.y + 1; x < tileCountX && y < tileCountY; x++, y++) 
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x - 1, y = cell.y - 1; x >= 0 && y >= 0; x--, y--)
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x - 1, y = cell.y + 1; x >= 0 && y < tileCountY; x--, y++)
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x + 1, y = cell.y - 1; x < tileCountX && y >= 0; x++, y--)
                r.Add(new Vector2Int(x, y));
            r.Add(new Vector2Int(cell.x, cell.y));
            return r;
        }
        public override List<Vector2Int> GetSpecialAttack3Cells(Vector2Int cell, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            for (var i = 0; i < tileCountX; i++)
                r.Add(new Vector2Int(i, cell.y));
            for (var j = 0; j < tileCountY; j++)
                r.Add(new Vector2Int(cell.x, j));
            for (int x = cell.x + 1, y = cell.y + 1; x < tileCountX && y < tileCountY; x++, y++) 
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x - 1, y = cell.y - 1; x >= 0 && y >= 0; x--, y--)
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x - 1, y = cell.y + 1; x >= 0 && y < tileCountY; x--, y++)
                r.Add(new Vector2Int(x, y));
            for (int x = cell.x + 1, y = cell.y - 1; x < tileCountX && y >= 0; x++, y--)
                r.Add(new Vector2Int(x, y));
            return r;
        }
    }
}
