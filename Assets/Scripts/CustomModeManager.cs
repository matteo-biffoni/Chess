using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomModeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text BoardText;
    [SerializeField] private Slider BoardSlider;
    [SerializeField] private TMP_Text MovesText;
    [SerializeField] private Slider MovesSlider;
    [SerializeField] private Toggle MinigameToggle;
    [SerializeField] private GameObject DispositionPanel;
    [SerializeField] private GameUI GameUI;
    [SerializeField] private Button HostButton;
    public bool Visible;

    public void SetBoardSlider()
    {
        if ((int) BoardSlider.value == 1)
        {
            BoardText.text = "Full board";
            DispositionPanel.SetActive(false);
            GameUI.SetFullboard(true);
            GameUI.SelectAttackingDisposition(DispositionType.None);
        }
        else
        {
            BoardText.text = "Board subset";
            DispositionPanel.SetActive(true);
            GameUI.SetFullboard(false);
        }
    }

    public void SetMovesSlider()
    {
        if ((int) MovesSlider.value == 1)
        {
            MovesText.text = "2";
            GameUI.Set2Turns(true);
        }
        else
        {
            MovesText.text = "1";
            GameUI.Set2Turns(false);
        }
    }

    public void MinigameToggleValueChanged()
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        GameUI.SetMinigame(MinigameToggle.isOn);
    }

    private void Update()
    {
        if (Visible)
        {
            DispositionPanel.SetActive((int)BoardSlider.value == 0);
            if ((int)BoardSlider.value == 0 && GameUI.GetSelectedAttackingDisposition() == DispositionType.None)
                GameUI.SelectAttackingDisposition(DispositionType.Heavy);
            HostButton.interactable = true;
        }
    }
}
