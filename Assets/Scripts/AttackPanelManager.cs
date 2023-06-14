using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Panels;
    private uint _selectedAttack;

    private void Start()
    {
        SelectAttack(0);
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