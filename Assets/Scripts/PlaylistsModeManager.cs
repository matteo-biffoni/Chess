
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class PlaylistsModeManager : MonoBehaviour
{
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color NormalTextColor;
    [SerializeField] private Color HoverColor;
    [SerializeField] private Color HoverTextColor;
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color SelectedTextColor;
    [SerializeField] private Button[] Selections;
    private int _selectedIndex = -1;
    
    public void HoverButton(int i)
    {
        Selections[i].GetComponent<Image>().color = HoverColor;
        Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = HoverTextColor;
        Selections[i].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ClearHoverButton(int i)
    {
        if (i != _selectedIndex)
        {
            Selections[i].GetComponent<Image>().color = NormalColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = NormalTextColor;
        }
        else
        {
            Selections[i].GetComponent<Image>().color = SelectedColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = SelectedTextColor;
        }
        Selections[i].transform.GetChild(1).gameObject.SetActive(false);
    }

    public void SelectButton(int i)
    {
        if (_selectedIndex != i)
        {
            DeselectButton(_selectedIndex);
            _selectedIndex = i;
            Selections[i].GetComponent<Image>().color = SelectedColor;
            Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = SelectedTextColor;
        }
    }

    private void DeselectButton(int i)
    {
        if (i == -1) return;
        Selections[i].GetComponent<Image>().color = NormalColor;
        Selections[i].transform.GetChild(0).GetComponent<TMP_Text>().color = NormalTextColor;
    }
}
