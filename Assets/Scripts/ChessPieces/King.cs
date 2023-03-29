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
            else if(board[CurrentX - 1, CurrentY].Team != Team)
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

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        var r = SpecialMove.None;
        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == (Team == 0 ? 0 : 7));
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == (Team == 0 ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == (Team == 0 ? 0 : 7));
        if (kingMove == null && CurrentX == 4)
        {
            if (Team == 0)
            {
                if (leftRook == null)
                    if (board[0, 0].Type == ChessPieceType.Rook)
                        if (board[0, 0].Team == 0)
                            if (board[3, 0] == null)
                                if (board[2, 0] == null)
                                    if (board[1, 0] == null)
                                    {
                                        availableMoves.Add(new Vector2Int(2, 0));
                                        r = SpecialMove.Castling;
                                    }
                if (rightRook == null)
                    if (board[7, 0].Type == ChessPieceType.Rook)
                        if (board[7, 0].Team == 0)
                            if (board[5, 0] == null)
                                if (board[6, 0] == null)
                                {
                                    availableMoves.Add(new Vector2Int(6, 0));
                                    r = SpecialMove.Castling;
                                }
            }
            else
            {
                if (leftRook == null)
                    if (board[0, 7].Type == ChessPieceType.Rook)
                        if (board[0, 7].Team == 1)
                            if (board[3, 7] == null)
                                if (board[2, 7] == null)
                                    if (board[1, 7] == null)
                                    {
                                        availableMoves.Add(new Vector2Int(2, 7));
                                        r = SpecialMove.Castling;
                                    }
                if (rightRook == null)
                    if (board[7, 7].Type == ChessPieceType.Rook)
                        if (board[7, 7].Team == 1)
                            if (board[5, 7] == null)
                                if (board[6, 7] == null)
                                {
                                    availableMoves.Add(new Vector2Int(6, 7));
                                    r = SpecialMove.Castling;
                                }
            }
        }
        return r;
    }
}
