using System;
using System.Collections;
using System.Collections.Generic;
using ChessPieces;
using Unity.Mathematics;
using UnityEngine;

public class HoverPiecesCorrespondance : MonoBehaviour
{
    [SerializeField] private ChessBoard ChessBoard;
    [SerializeField] private KeyCode KeyToHover;

    [SerializeField] private GameObject[] Traditional;
    [SerializeField] private Material[] WhiteMaterials;
    [SerializeField] private Material[] BlackMaterials;

    private readonly List<GameObject> _instantiated = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyToHover))
        {
            foreach (var chessPiece in ChessBoard.GetChessPieces())
            {
                if (chessPiece != null)
                {
                    var chessPiecePosition = chessPiece.transform.position;
                    var point = new Vector3(chessPiecePosition.x, 0.55f,
                        chessPiecePosition.z);
                    var cpt = chessPiece.Type;
                    GameObject instantiated;
                    var newScale = new Vector3(0.03f, 0.03f, 0.03f);
                    switch (cpt)
                    {
                        case ChessPieceType.None:
                            break;
                        case ChessPieceType.Pawn:
                            instantiated = Instantiate(Traditional[0], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<Pawn>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[0] : BlackMaterials[0];
                            _instantiated.Add(instantiated);
                            break;
                        case ChessPieceType.Rook:
                            instantiated = Instantiate(Traditional[1], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<Rook>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[1] : BlackMaterials[1];
                            _instantiated.Add(instantiated);
                            break;
                        case ChessPieceType.Knight:
                            instantiated = Instantiate(Traditional[2], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<Knight>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[2] : BlackMaterials[2];
                            _instantiated.Add(instantiated);
                            break;
                        case ChessPieceType.Bishop:
                            instantiated = Instantiate(Traditional[3], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<Bishop>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[3] : BlackMaterials[3];
                            _instantiated.Add(instantiated);
                            break;
                        case ChessPieceType.Queen:
                            instantiated = Instantiate(Traditional[4], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<Queen>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[4] : BlackMaterials[4];
                            _instantiated.Add(instantiated);
                            break;
                        case ChessPieceType.King:
                            instantiated = Instantiate(Traditional[5], point, quaternion.Euler(0f, 0f, 0f));
                            instantiated.transform.localScale = newScale;
                            Destroy(instantiated.GetComponent<King>());
                            instantiated.GetComponent<MeshRenderer>().material =
                                chessPiece.Team == 0 ? WhiteMaterials[5] : BlackMaterials[5];
                            _instantiated.Add(instantiated);
                            break;
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyToHover))
        {
            foreach (var instantiated in _instantiated)
            {
                Destroy(instantiated);
            }
        }
    }
}
