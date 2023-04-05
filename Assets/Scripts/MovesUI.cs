using System;
using System.Collections;
using System.Collections.Generic;
using ChessPieces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovesUI : MonoBehaviour
{
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private ScrollRect ScrollRect;

    [SerializeField] private Sprite BlackKing;
    [SerializeField] private Sprite BlackQueen;
    [SerializeField] private Sprite BlackKnight;
    [SerializeField] private Sprite BlackBishop;
    [SerializeField] private Sprite BlackRook;
    [SerializeField] private Sprite BlackPawn;
    [SerializeField] private Sprite WhiteKing;
    [SerializeField] private Sprite WhiteQueen;
    [SerializeField] private Sprite WhiteKnight;
    [SerializeField] private Sprite WhiteBishop;
    [SerializeField] private Sprite WhiteRook;
    [SerializeField] private Sprite WhitePawn;

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        ScrollRect.verticalNormalizedPosition = 0;
    }

    public void AddMove(ChessMove move)
    {
        var newCell = Instantiate(ItemPrefab, transform);
        newCell.transform.GetChild(0).GetComponent<Image>().sprite = move.Piece switch
        {
            ChessPieceType.None => null,
            ChessPieceType.Pawn => move.Team == 0 ? WhitePawn : BlackPawn,
            ChessPieceType.Rook => move.Team == 0 ? WhiteRook : BlackRook,
            ChessPieceType.Knight => move.Team == 0 ? WhiteKnight : BlackKnight,
            ChessPieceType.Bishop => move.Team == 0 ? WhiteBishop : BlackBishop,
            ChessPieceType.Queen => move.Team == 0 ? WhiteQueen : BlackQueen,
            ChessPieceType.King => move.Team == 0 ? WhiteKing : BlackKing,
            _ => throw new ArgumentOutOfRangeException()
        };
        newCell.transform.GetChild(1).GetComponent<TMP_Text>().text = GetLabelFromPosition(move.From);
        newCell.transform.GetChild(3).GetComponent<TMP_Text>().text = GetLabelFromPosition(move.To);
        if (move.Confrontation)
        {
            newCell.transform.GetChild(5).GetComponent<Image>().sprite = move.Enemy switch
            {
                ChessPieceType.None => null,
                ChessPieceType.Pawn => move.Team == 1 ? WhitePawn : BlackPawn,
                ChessPieceType.Rook => move.Team == 1 ? WhiteRook : BlackRook,
                ChessPieceType.Knight => move.Team == 1 ? WhiteKnight : BlackKnight,
                ChessPieceType.Bishop => move.Team == 1 ? WhiteBishop : BlackBishop,
                ChessPieceType.Queen => move.Team == 1 ? WhiteQueen : BlackQueen,
                ChessPieceType.King => move.Team == 1 ? WhiteKing : BlackKing,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            newCell.transform.GetChild(4).gameObject.SetActive(false);
            newCell.transform.GetChild(5).gameObject.SetActive(false);
        }
        ScrollToBottom();
    }

    private static string GetLabelFromPosition(Vector2Int pos)
    {
        return $"{Convert.ToChar('A' + pos.x)}{pos.y + 1}";
    }
}
