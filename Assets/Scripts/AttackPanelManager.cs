using System;
using System.Collections;
using System.Collections.Generic;
using ChessPieces;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanelManager : MonoBehaviour
{
    //prova
    [SerializeField] private GameObject[] Panels;
    [SerializeField] private Sprite NormalAttack;
    //[SerializeField] private Sprite Pawn1;
    [SerializeField] private Sprite Bishop1;
    [SerializeField] private Sprite Bishop2;
    [SerializeField] private Sprite Knight1;
    [SerializeField] private Sprite Rook1;
    [SerializeField] private Sprite Rook2;
    [SerializeField] private Sprite Queen1;
    [SerializeField] private Sprite Queen2;
    [SerializeField] private Sprite Queen3;
    [SerializeField] private Sprite King1;
    private uint _selectedAttack;

    private void Start()
    {
        SelectAttack(0);
    }

    public void SetAttackingType(ChessPieceType chessPieceType)
    {
        Panels[0].GetComponent<Image>().sprite = NormalAttack;
        switch (chessPieceType)
        {
            case ChessPieceType.None:
                break;
            case ChessPieceType.Pawn:
                break;
            case ChessPieceType.Rook:
                Panels[1].GetComponent<Image>().sprite = Rook1;
                Panels[2].GetComponent<Image>().sprite = Rook2;
                break;
            case ChessPieceType.Knight:
                Panels[1].GetComponent<Image>().sprite = Knight1;
                break;
            case ChessPieceType.Bishop:
                Panels[1].GetComponent<Image>().sprite = Bishop1;
                Panels[2].GetComponent<Image>().sprite = Bishop2;
                break;
            case ChessPieceType.Queen:
                Panels[1].GetComponent<Image>().sprite = Queen1;
                Panels[2].GetComponent<Image>().sprite = Queen2;
                Panels[3].GetComponent<Image>().sprite = Queen3;
                break;
            case ChessPieceType.King:
                Panels[1].GetComponent<Image>().sprite = King1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(chessPieceType), chessPieceType, null);
        }
    }

    public void EnableAppropriateAttacks(int attacksToRemove)
    {
        while (attacksToRemove > 0)
        {
            Panels[4 - attacksToRemove].SetActive(false);
            attacksToRemove--;
        }
    }

    public void SelectAttack(uint which)
    {
        _selectedAttack = which;
        for (var i = 0; i < Panels.Length; i++)
        {
            Panels[i].transform.localScale = i != which ? Vector3.one : new Vector3(2f, 2f, 2f);
        }
    }

    public uint GetSelectedAttack()
    {
        return _selectedAttack;
    }

    public IEnumerator Attack(uint which, float duration)
    {
        var image = Panels[which].transform.GetChild(0).GetComponent<Image>();
        image.fillAmount = 1;
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            image.fillAmount = Mathf.Lerp(1, 0, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        image.fillAmount = 0;
    }
}