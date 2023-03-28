using System.Collections.Generic;
using ChessPieces;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        var r = new List<Vector2Int>();
        if (CurrentX + 1 < tileCountX)
        {
            if (board[CurrentX + 1, CurrentY] == null)
                r.Add(new Vector2Int(CurrentX + 1, CurrentY));
            else if(board[CurrentX + 1, CurrentY].Team != Team)
                r.Add(new Vector2Int(CurrentX + 1, CurrentY));
            
            if (CurrentY + 1 < tileCountY)
                if (board[CurrentX + 1, CurrentY + 1] == null)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY + 1));
                else if(board[CurrentX + 1, CurrentY + 1].Team != Team)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY + 1));
            if (CurrentY - 1 >= 0)
                if (board[CurrentX + 1, CurrentY - 1] == null)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
                else if(board[CurrentX + 1, CurrentY - 1].Team != Team)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
        }

        if (CurrentX - 1 >= 0)
        {
            if (board[CurrentX - 1, CurrentY] == null)
                r.Add(new Vector2Int(CurrentX - 1, CurrentY));
            else if(board[CurrentX + 1, CurrentY].Team != Team)
                r.Add(new Vector2Int(CurrentX - 1, CurrentY));
            
            if (CurrentY + 1 < tileCountY)
                if (board[CurrentX - 1, CurrentY + 1] == null)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
                else if(board[CurrentX - 1, CurrentY + 1].Team != Team)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
            if (CurrentY - 1 >= 0)
                if (board[CurrentX - 1, CurrentY - 1] == null)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
                else if(board[CurrentX - 1, CurrentY - 1].Team != Team)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
        }
        if (CurrentY + 1 < tileCountY)
            if (board[CurrentX, CurrentY + 1] == null || board[CurrentX, CurrentY + 1].Team != Team)
                r.Add(new Vector2Int(CurrentX, CurrentY + 1));
        
        if (CurrentY - 1 >= 0)
            if (board[CurrentX, CurrentY - 1] == null || board[CurrentX, CurrentY - 1].Team != Team)
                r.Add(new Vector2Int(CurrentX, CurrentY - 1));
        return r;
    }
}
