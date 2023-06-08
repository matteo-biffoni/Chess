using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    [SerializeField] private float MouseSensitivity = 3.0f;
    private float _rotationY;
    private float _rotationX;
    [SerializeField] private float DistanceFromTarget = 3.0f;
    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;
    [SerializeField] private float SmoothTime = 0.2f;
    [SerializeField] private Transform WhiteStartingPosition;
    [SerializeField] private Transform BlackStartingPosition;

    [SerializeField] private Transform ChessBoard;
    private bool _white;

    private bool _gameStarted;

    public void SetPosition(bool white)
    {
        _white = white;
        transform.position = white ? WhiteStartingPosition.position : BlackStartingPosition.position;
        transform.rotation = white ? WhiteStartingPosition.rotation : BlackStartingPosition.rotation;
        _gameStarted = true;
    }

    private void Update()
    {
        if (_gameStarted)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _currentRotation = transform.rotation.eulerAngles;
            }
            else if (Input.GetMouseButton(1))
            {
                var t = transform;
                var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
                var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;
                _rotationY += mouseX;
                _rotationX += mouseY;
                _rotationX = Mathf.Clamp(_rotationX, 10, 90);
                var nextRotation = new Vector3(_rotationX, _rotationY);
                _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, SmoothTime);
                transform.localEulerAngles = _currentRotation;
                t.position = ChessBoard.position - t.forward * DistanceFromTarget;
            }

            if (Input.GetMouseButtonDown(2))
            {
                SetPosition(_white);
            }
        }
    }
}
