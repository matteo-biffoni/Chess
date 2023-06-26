using System.Collections.Generic;
using UnityEngine;

namespace ChessPieces
{
    public class Pawn : ChessPiece
    {
        public override List<Vector2Int> GetPossibleMovesFromPosition(ref ChessPiece[,] board, int tileCountX, int tileCountY, Vector2Int fromPosition)
        {
            var r = new List<Vector2Int>();
            var direction = Team == 0 ? 1 : -1;
            if (board[fromPosition.x, fromPosition.y + direction] == null)
                r.Add(new Vector2Int(fromPosition.x, fromPosition.y + direction));
            if (board[fromPosition.x, fromPosition.y + direction] == null)
            {
                if (Team == 0 && fromPosition.y == 1 && board[fromPosition.x, fromPosition.y + direction * 2] == null)
                    r.Add(new Vector2Int(fromPosition.x, fromPosition.y + direction * 2));
            
                if (Team == 1 && fromPosition.y == 6 && board[fromPosition.x, fromPosition.y + direction * 2] == null)
                    r.Add(new Vector2Int(fromPosition.x, fromPosition.y + direction * 2));
            }
            if (fromPosition.x != tileCountX - 1)
                if (board[fromPosition.x + 1, fromPosition.y + direction] != null && board[fromPosition.x + 1, fromPosition.y + direction].Team != Team)
                    r.Add(new Vector2Int(fromPosition.x + 1, fromPosition.y + direction));
        
            if (fromPosition.x != 0)
                if (board[fromPosition.x - 1, fromPosition.y + direction] != null && board[fromPosition.x - 1, fromPosition.y + direction].Team != Team)
                    r.Add(new Vector2Int(fromPosition.x - 1, fromPosition.y + direction));
            return r;        }

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

        public override List<Vector2Int> GetAvailableMovesInConfrontation(ref ChessPiece[,] board, int tileCountX, int tileCountY)
        {
            var r = new List<Vector2Int>();
            if (CurrentX != tileCountX - 1)
            {
                if (CurrentY != tileCountY - 1)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY + 1));
                if (CurrentY != 0)
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
            }
            if (CurrentX != 0)
            {
                if (CurrentY != 0)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
                if (CurrentY != tileCountY - 1)
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
            }
            if (CurrentY != 0)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY - 1));
            }
            if (CurrentY != tileCountY - 1)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY + 1));
            }
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
}
