using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInConfrontationHandler : MonoBehaviour
{
    private bool _movable;
    private bool _lastMovable;
    private bool _fired;
    private bool _lastFired;

    /*public bool GetMovable()
    {
        return _movable;
    }*/

    public void SetMovable(bool value)
    {
        _movable = value;
    }

    /*public bool GetFired()
    {
        return _fired;
    }*/

    public void SetFired(bool value)
    {
        _fired = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movable != _lastMovable)
        {
            _lastMovable = _movable;
            // Attivare oggetto in sovraimpressione per movable
            transform.GetChild(0).gameObject.SetActive(_movable);
        }

        if (_fired != _lastFired)
        {
            _lastFired = _fired;
            // Attivare oggetto in sovraimpressione per fired
            transform.GetChild(1).gameObject.SetActive(_fired);
        }
    }
}
