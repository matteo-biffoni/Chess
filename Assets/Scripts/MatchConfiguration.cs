using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConfiguration
{
    public bool Attacking;
    public bool FullBoard;
    public DispositionType DispositionAttacking = DispositionType.None;
    public DispositionType DispositionDefending = DispositionType.None;
    public bool Turns;
    public bool MiniGame = true;

    private static MatchConfiguration _fromGameUI;

    public static MatchConfiguration GetGameUIConfiguration()
    {
        return _fromGameUI;
    }

    public static void SetGameUIConfigurationP1(bool fullBoard, DispositionType dispositionAttacking, bool turns, bool miniGame)
    {
        _fromGameUI = new MatchConfiguration
        {
            Attacking = true,
            FullBoard = fullBoard,
            DispositionAttacking = dispositionAttacking,
            Turns = turns,
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

    public static void SetChessboardFromPlayer1(bool fullBoard, DispositionType dispositionAttacking, bool turns,
        bool miniGame)
    {
        if (ChessBoard.MatchConfiguration == null)
        {
            ChessBoard.MatchConfiguration = new MatchConfiguration
            {
                FullBoard = fullBoard,
                DispositionAttacking = dispositionAttacking,
                Turns = turns,
                MiniGame = miniGame
            };
        }
        else
        {
            ChessBoard.MatchConfiguration.FullBoard = fullBoard;
            ChessBoard.MatchConfiguration.DispositionAttacking = dispositionAttacking;
            ChessBoard.MatchConfiguration.Turns = turns;
            ChessBoard.MatchConfiguration.MiniGame = miniGame;
        }
    }

    public static void SetChessboardFromPlayer2(DispositionType dispositionDefending)
    {
        if (ChessBoard.MatchConfiguration == null)
        {
            ChessBoard.MatchConfiguration = new MatchConfiguration
            {
                DispositionDefending = dispositionDefending
            };
        }
        else
        {
            ChessBoard.MatchConfiguration.DispositionDefending = dispositionDefending;
        }
        Debug.Log($"Chessboard configuration from p2: {ChessBoard.MatchConfiguration}");
    }

    public string ToStringReduced()
    {
        var turns = Turns ? "2" : "1";
        return Attacking ? $"FullBoard: {FullBoard}, Disposition attacking: {DispositionAttacking}, Turns: {turns}, Minigame: {MiniGame}" : $"DispositionDefending: {DispositionDefending}";

    }

    public override string ToString()
    {
        var turns = Turns ? "2" : "1";
        return $"Attacking: {Attacking}, FullBoard: {FullBoard}, Disposition attacking: {DispositionAttacking}, DispositionDefending: {DispositionDefending}, Turns: {turns}, Minigame: {MiniGame}";
    }

    public static MatchConfiguration GetDifferenceFromConf(MatchConfiguration conf1, MatchConfiguration conf2)
    {
        var r = new MatchConfiguration();
        if (conf1.DispositionDefending != DispositionType.None)
        {
            r.FullBoard = conf2.FullBoard;
            r.DispositionAttacking = conf2.DispositionAttacking;
            r.DispositionDefending = conf1.DispositionDefending;
            r.Turns = conf2.Turns;
            r.MiniGame = conf2.MiniGame;
        }
        else if (conf2.DispositionDefending != DispositionType.None)
        {
            r.FullBoard = conf1.FullBoard;
            r.DispositionAttacking = conf1.DispositionAttacking;
            r.DispositionDefending = conf2.DispositionDefending;
            r.Turns = conf1.Turns;
            r.MiniGame = conf1.MiniGame;
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

