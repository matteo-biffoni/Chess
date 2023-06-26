
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class PlaylistsModeManager : MonoBehaviour
{
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color NormalTextColor;
    [SerializeField] private Color NormalSubtextColor;
    [SerializeField] private Color HoverColor;
    [SerializeField] private Color HoverTextColor;
    [SerializeField] private Color HoverSubtextColor;
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color SelectedTextColor;
    [SerializeField] private Color SelectedSubtextColor;
    [SerializeField] private Button[] Selections;
    [SerializeField] private Button HostButton;
    private int _selectedIndex = -1;

    [SerializeField] private GameObject DispositionPanel;
    [SerializeField] private GameUI GameUI;

    public bool Visible = true;
    
    public void HoverButton(int i)
    {
        Selections[i].GetComponent<Image>().color = HoverColor;
        Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = HoverTextColor;
        Selections[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = HoverSubtextColor;
        Selections[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = HoverSubtextColor;
        Selections[i].transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().color = HoverSubtextColor;
        //Selections[i].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ClearHoverButton(int i)
    {
        if (i != _selectedIndex)
        {
            Selections[i].GetComponent<Image>().color = NormalColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = NormalTextColor;
            Selections[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
        }
        else
        {
            Selections[i].GetComponent<Image>().color = SelectedColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = SelectedTextColor;
            Selections[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;
        }
        //Selections[i].transform.GetChild(1).gameObject.SetActive(false);
    }

    public void SelectButton(int i)
    {
        AudioManager.Instance.PlayClip(SoundClip.ButtonPressed);
        if (_selectedIndex != i)
        {
            DeselectButton(_selectedIndex);
            _selectedIndex = i;
            Selections[i].GetComponent<Image>().color = SelectedColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = SelectedTextColor;
            Selections[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;
            Selections[i].transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().color = SelectedSubtextColor;

            var playlist = PlaylistSelection.Playlists[_selectedIndex];

            if(!playlist.GetFullboard()){
                DispositionPanel.SetActive(true);
                GameUI.SelectAttackingDisposition(DispositionType.Heavy);
            }
            else{
                DispositionPanel.SetActive(false);
                GameUI.SelectAttackingDisposition(DispositionType.None);
            }

            GameUI.SetFullboard(playlist.GetFullboard());
            GameUI.Set2Turns(playlist.Get2Turns());
            GameUI.SetMinigame(playlist.GetMinigame());

            
        }
    }

    private void DeselectButton(int i)
    {
        if (i == -1) return;
        Selections[i].GetComponent<Image>().color = NormalColor;
        Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = NormalTextColor;
        Selections[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
        Selections[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
        Selections[i].transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().color = NormalSubtextColor;
    }

    private void Update()
    {
        if (Visible)
        {
            HostButton.interactable = _selectedIndex != -1;
            DispositionPanel.SetActive(_selectedIndex < 4 && _selectedIndex != -1);
        }
    }
}

public class PlaylistSelection
{
    private bool _fullboard;
    private bool _2Turns;
    private bool _minigame;

    private PlaylistSelection(bool fullboard, bool twoTurns, bool minigame)
    {
        _fullboard = fullboard;
        _2Turns = twoTurns;
        _minigame = minigame;
    }

    public static readonly PlaylistSelection[] Playlists = 
    {
        new (false, false, false), // Minichess
        new (false, true, true), // Garden 2 Moves
        new (false, true, false), // SpeedChess
        new (false, false, true), // GardenChess
        new (true, true, true), // Chess+2Moves
        new (true, true, false), // Chess2Moves
        new (true, false, true), // Chess+Minigame
        new (true, false, false) // TraditionalChess
    };

    public bool GetFullboard()
    {
        return _fullboard;
    }

    public bool Get2Turns()
    {
        return _2Turns;
    }

    public bool GetMinigame()
    {
        return _minigame;
    }
}
