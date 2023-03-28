using System.Collections.Generic;
using ChessPieces;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        var r = new List<Vector2Int>();
        var x = CurrentX + 1;
        var y = CurrentY + 2;
        if (x < tileCountX && y < tileCountY)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX + 2;
        y = CurrentY + 1;
        if (x < tileCountX && y < tileCountY)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX - 1;
        y = CurrentY + 2;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX - 2;
        y = CurrentY + 1;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX + 1;
        y = CurrentY - 2;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX + 2;
        y = CurrentY - 1;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX - 1;
        y = CurrentY - 2;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        
        x = CurrentX - 2;
        y = CurrentY - 1;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].Team != Team)
                r.Add(new Vector2Int(x, y));
        return r;
    }
}
