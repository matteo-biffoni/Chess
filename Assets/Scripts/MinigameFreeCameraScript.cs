using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameFreeCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject FreeCamera;

    private float _dirX, _dirZ;
    [SerializeField] private float RotationSpeed = 40f;
    [SerializeField] private Transform ChessboardTransform;

    private void Update()
    {
        if (FreeCamera.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                FreeCamera.SetActive(false);
                MainCamera.SetActive(true);
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    FreeCamera.transform.Translate(Vector3.forward * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    FreeCamera.transform.Translate(Vector3.back * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    FreeCamera.transform.RotateAround(ChessboardTransform.position, Vector3.down, RotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    FreeCamera.transform.RotateAround(ChessboardTransform.position, Vector3.up, RotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                MainCamera.SetActive(false);
                FreeCamera.SetActive(true);
            }
        }
    }
}
