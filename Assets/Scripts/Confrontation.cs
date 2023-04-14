using ChessPieces;
using UnityEngine;

public enum Outcome
{
    Success,
    Failure,
    NotAvailable,
}

public class Confrontation : MonoBehaviour
{
    private static Confrontation Instance { get; set; }

    private ChessPiece _currentAttacking;
    private ChessPiece _currentDefending;
    private Outcome _currentOutcome;

    public static bool ConfrontationSceneLoaded;


    public static void StartConfrontation(ChessPiece attacking, ChessPiece defending)
    {
        Instance._currentAttacking = attacking;
        Instance._currentDefending = defending;
    }

    public static string GetActualConfrontationInfo()
    {
        return Instance._currentAttacking + " is attacking " + Instance._currentDefending;
    }

    public static void SetCurrentOutcome(Outcome outcome)
    {
        Instance._currentOutcome = outcome;
    }

    public static Outcome GetCurrentOutcome()
    {
        var currentOutcome = Instance._currentOutcome;
        return currentOutcome;
    }

    public static void ResetConfrontation()
    {
        Instance._currentOutcome = Outcome.NotAvailable;
        Instance._currentAttacking = null;
        Instance._currentDefending = null;
    }

    public static ChessPiece GetCurrentAttacking()
    {
        return Instance._currentAttacking;
    }

    public static ChessPiece GetCurrentDefending()
    {
        return Instance._currentDefending;
    }

    private void Awake()
    {
        Instance = this;
        _currentAttacking = null;
        _currentDefending = null;
        _currentOutcome = Outcome.NotAvailable;
    }
    
    
}