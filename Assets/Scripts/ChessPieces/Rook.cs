using System.Collections.Generic;
using ChessPieces;
using UnityEngine;

public class Rook : ChessPiece
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
        return r;
    }
}
