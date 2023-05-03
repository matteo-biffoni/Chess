using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfrontationListener : MonoBehaviour
{
    private bool _unloaded;
    public static bool IsAttacking;
    [SerializeField] private GameObject _chessBoard;
    [SerializeField] private TMP_Text ConfrontationLabel;
    [SerializeField] private GameObject InteractionCanvas;
    [SerializeField] private GameObject WaitingForConfrontationCanvas;

    private void Start()
    {
        
        /*ConfrontationLabel.text = Confrontation.GetActualConfrontationInfo();
        if (IsAttacking)
        {
            InteractionCanvas.SetActive(true);
            WaitingForConfrontationCanvas.SetActive(false);
        }
        else
        {
            WaitingForConfrontationCanvas.SetActive(true);
            InteractionCanvas.SetActive(false);
        }*/
    }

    private void Update()
    {
        if (Confrontation.GetCurrentOutcome() == Outcome.NotAvailable) return;
        if (!_unloaded)
        {
            _unloaded = true;
            Confrontation.ConfrontationSceneLoaded = false;
            SceneManager.UnloadSceneAsync(1);
        }
    }
}