using System.Collections.Generic;
using ChessPieces;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        var r = new List<Vector2Int>();
        var direction = Team == 0 ? 1 : -1;
        if (board[CurrentX, CurrentY + direction] == null)
            r.Add(new Vector2Int(CurrentX, CurrentY + direction));
        if (board[CurrentX, CurrentY + direction] == null)
        {
            if (Team == 0 && CurrentY == 1 && board[CurrentX, CurrentY + direction * 2] == null)
                r.Add(new Vector2Int(CurrentX, CurrentY + direction * 2));
            
            if (Team == 1 && CurrentY == 6 && board[CurrentX, CurrentY + direction * 2] == null)
                r.Add(new Vector2Int(CurrentX, CurrentY + direction * 2));
        }
        if (CurrentX != tileCountX - 1)
            if (board[CurrentX + 1, CurrentY + direction] != null && board[CurrentX + 1, CurrentY + direction].Team != Team)
                r.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
        
        if (CurrentX != 0)
            if (board[CurrentX - 1, CurrentY + direction] != null && board[CurrentX - 1, CurrentY + direction].Team != Team)
                r.Add(new Vector2Int(CurrentX - 1, CurrentY + direction));
        return r;
    }
}
