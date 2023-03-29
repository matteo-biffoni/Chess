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

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        var direction = Team == 0 ? 1 : -1;
        if ((Team == 0 && CurrentY == 6) || (Team == 1 && CurrentY == 1))
            return SpecialMove.Promotion;
        // En Passant
        if (moveList.Count > 0)
        {
            var lastMove = moveList[^1];
            if (board[lastMove[1].x, lastMove[1].y].Type == ChessPieceType.Pawn)
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)
                    if (board[lastMove[1].x, lastMove[1].y].Team != Team)
                        if (lastMove[1].y == CurrentY)
                        {
                            if (lastMove[1].x == CurrentX - 1)
                            {
                                availableMoves.Add(new Vector2Int(CurrentX - 1, CurrentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            if (lastMove[1].x == CurrentX + 1)
                            {
                                availableMoves.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
        }
        return SpecialMove.None;
    }
}
