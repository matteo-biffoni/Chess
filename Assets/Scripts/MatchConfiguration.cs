using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConfiguration
{
    public bool Attacking;
    public bool FullBoard;
    public DispositionType DispositionAttacking;
    public DispositionType DispositionDefending;
    public bool OneMoveTurn;
    public bool MiniGame;

    private static MatchConfiguration _fromGameUI = GetDefault();

    public static MatchConfiguration GetDefault()
    {
        return new MatchConfiguration
        {
            FullBoard = false,
            DispositionAttacking = DispositionType.None,
            DispositionDefending = DispositionType.None,
            OneMoveTurn = true,
            MiniGame = true
        };
    }

    public static MatchConfiguration GetGameUIConfiguration()
    {
        return _fromGameUI;
    }

    public static void SetGameUIConfigurationP1(bool fullBoard, DispositionType dispositionAttacking, bool oneMoveTurn, bool miniGame)
    {
        _fromGameUI = new MatchConfiguration
        {
            Attacking = true,
            FullBoard = fullBoard,
            DispositionAttacking = dispositionAttacking,
            OneMoveTurn = oneMoveTurn,
            MiniGame = miniGame
        };
    }

    public static void SetGameUIConfigurationP2(DispositionType dispositionDefending)
    {
        _fromGameUI = new MatchConfiguration
        {
            Attacking = false,
            DispositionDefending = dispositionDefending
        };
    }

    public static void SetChessboardFromPlayer1(bool fullBoard, DispositionType dispositionAttacking, bool oneMoveTurn,
        bool miniGame)
    {
        ChessBoard.MatchConfiguration.FullBoard = fullBoard;
        ChessBoard.MatchConfiguration.DispositionAttacking = dispositionAttacking;
        ChessBoard.MatchConfiguration.OneMoveTurn = oneMoveTurn;
        ChessBoard.MatchConfiguration.MiniGame = miniGame;
        Debug.Log($"Chessboard configuration from p1: {ChessBoard.MatchConfiguration}");
    }

    public static void SetChessboardFromPlayer2(DispositionType dispositionDefending)
    {
        ChessBoard.MatchConfiguration.DispositionDefending = dispositionDefending;
        Debug.Log($"Chessboard configuration from p2: {ChessBoard.MatchConfiguration}");
    }

    public override string ToString()
    {
        return
            $"Attacking: {Attacking}, FullBoard: {FullBoard}, Disposition attacking: {DispositionAttacking}, DispositionDefending: {DispositionDefending}, Minigame: {MiniGame}, One move turn: {OneMoveTurn}";
    }

    public static MatchConfiguration GetDifferenceFromConf(MatchConfiguration conf1, MatchConfiguration conf2)
    {
        var r = new MatchConfiguration();
        if (conf1.DispositionDefending != DispositionType.None)
        {
            r.FullBoard = conf2.FullBoard;
            r.DispositionAttacking = conf2.DispositionAttacking;
            r.DispositionDefending = conf1.DispositionDefending;
            r.MiniGame = conf2.MiniGame;
            r.OneMoveTurn = conf2.OneMoveTurn;
        }
        else if (conf2.DispositionDefending != DispositionType.None)
        {
            r.FullBoard = conf1.FullBoard;
            r.DispositionAttacking = conf1.DispositionAttacking;
            r.DispositionDefending = conf2.DispositionDefending;
            r.MiniGame = conf1.MiniGame;
            r.OneMoveTurn = conf1.OneMoveTurn;
        }
        return r;
    }
}

public enum DispositionType
{
    None = 0,
    NotEmpty = 1
}

public class ChessBoardDisposition
{

    public static ChessBoardDisposition GetDispositionFromType(DispositionType type)
    {
        return null;
    }
    
}


