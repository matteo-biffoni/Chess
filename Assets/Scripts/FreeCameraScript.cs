using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FreeCameraScript : MonoBehaviour
{
    [SerializeField] private GameUI GameUI;

    private bool _usingThisCamera;
    private GameObject _previousCam;
    private CinemachineVirtualCamera _cineMachineVirtualCamera;

    private float _dirX, _dirZ;
    [SerializeField] private float RotationSpeed = 40f;
    [SerializeField] private ChessBoard Chessboard;

    private void Start()
    {
        _cineMachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cineMachineVirtualCamera.enabled = false;
    }

    private void Update()
    {
        if (_usingThisCamera)
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                _usingThisCamera = false;
                _cineMachineVirtualCamera.enabled = false;
                _previousCam.SetActive(true);
                _previousCam = null;
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    transform.Translate(Vector3.forward * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    transform.Translate(Vector3.back * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.RotateAround(Chessboard.transform.position, Vector3.down, RotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.RotateAround(Chessboard.transform.position, Vector3.up, RotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Comma) && GameUI.IsInGame())
            {
                _usingThisCamera = true;
                _previousCam = GameUI.SetFreeCam();
                _cineMachineVirtualCamera.enabled = true;
            }
        }
    }
}
