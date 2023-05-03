using System;
using System.Collections;
using System.Collections.Generic;
using ChessPieces;
using Net;
using Net.NetMessages;
using Unity.Networking.Transport;
using UnityEngine;

public class ChessBoardConfrontation : MonoBehaviour
{
    private ChessPiece _attacking;
    private ChessPiece _defending;

    [SerializeField] private Material TileMaterial;
    [SerializeField] private float TileSize = 1f;
    [SerializeField] private float YOffset = 0.2f;
    private const int TileCountX = 8;
    private const int TileCountY = 8;
    [SerializeField] private Vector3 BoardCenter = Vector3.zero;
    
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private Material[] BlackMaterials;
    [SerializeField] private Material[] WhiteMaterials;
    
    private GameObject[,] _tiles;
    private Vector3 _bounds;
    
    private ChessPiece[,] _board;

    private bool _isMoving;

    private List<Vector2Int> _availableMoves;

    private Vector3 _aim;

    private Camera _mainCamera;

    private void Awake()
    {
        SetupChessBoard();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        NetUtility.CMakeMoveInConfrontation += OnMakeMoveInConfrontationClient;
        NetUtility.CNormalAttackInConfrontation += OnNormalAttackInConfrontationClient;
        NetUtility.SMakeMoveInConfrontation += OnMakeMoveInConfrontationServer;
        NetUtility.SNormalAttackInConfrontation += OnNormalAttackInConfrontationServer;
    }

    private void UnregisterEvents()
    {
        NetUtility.CMakeMoveInConfrontation -= OnMakeMoveInConfrontationClient;
        NetUtility.CNormalAttackInConfrontation -= OnNormalAttackInConfrontationClient;
        NetUtility.SMakeMoveInConfrontation -= OnMakeMoveInConfrontationServer;
        NetUtility.SNormalAttackInConfrontation -= OnNormalAttackInConfrontationServer;
    }

    private void OnNormalAttackInConfrontationClient(NetMessage msg)
    {
        if (msg is not NetNormalAttackInConfrontation nnaic)
        {
            Debug.LogError("[C] Could not cast NetMessage to NetNormalAttackInConfrontation");
            return;
        }
        if (!ConfrontationListener.IsAttacking)
        {
            FireCell(new Vector2Int(nnaic.DestinationX, nnaic.DestinationY));
        }
    }

    private void OnMakeMoveInConfrontationClient(NetMessage msg)
    {
        if (msg is not NetMakeMoveInConfrontation mmic)
        {
            Debug.LogError("[C] Could not cast NetMessage to NetMakeMoveInConfrontation");
            return;
        }
        if (ConfrontationListener.IsAttacking)
        {
            _aim = GetTileCenter(mmic.DestinationX, mmic.DestinationY);
            _defending.CurrentX = mmic.DestinationX;
            _defending.CurrentY = mmic.DestinationY;
            _defending.SetPosition(_aim);
        }
    }

    private void OnNormalAttackInConfrontationServer(NetMessage msg, NetworkConnection cnn)
    {
        if (msg is not NetNormalAttackInConfrontation nnaic)
        {
            Debug.LogError("[S] Could not cast NetMessage to NetNormalAttackInConfrontation");
            return;
        }
        Server.Instance.Broadcast(nnaic);
    }

    private void OnMakeMoveInConfrontationServer(NetMessage msg, NetworkConnection cnn)
    {
        if (msg is not NetMakeMoveInConfrontation mmic)
        {
            Debug.LogError("[S] Could not cast NetMessage to NetMakeMoveInConfrontation");
            return;
        }
        Server.Instance.Broadcast(mmic);
    }

    private void SetAvailablePath()
    {
        _availableMoves = _defending.GetAvailableMovesInConfrontation(ref _board, TileCountX, TileCountY);
        HighlightTiles();
    }

    private void FireCell(Vector2Int cell)
    {
        // Mettere preavviso
        _tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("FiredCell");
        StartCoroutine(UnFireCellAfterXSec(cell, 1));
    }

    private IEnumerator UnFireCellAfterXSec(Vector2Int cell, int sec)
    {
        yield return new WaitForSeconds(sec);
        _tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("Tile");
    }

    private void Update()
    {
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
            return;
        }
        if (ConfrontationListener.IsAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile")))
                {
                    var hitPosition = LookupTileIndex(info.transform.gameObject);
                    FireCell(hitPosition);
                    var nnaic = new NetNormalAttackInConfrontation
                    {
                        DestinationX = hitPosition.x,
                        DestinationY = hitPosition.y
                    };
                    Client.Instance.SendToServer(nnaic);
                }
            }
        }
        else
        {
            if (!_isMoving)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("HighlightHard")))
                    {
                        var hitPosition = LookupTileIndex(info.transform.gameObject);
                        _aim = GetTileCenter(hitPosition.x, hitPosition.y);
                        _defending.CurrentX = hitPosition.x;
                        _defending.CurrentY = hitPosition.y;
                        _defending.SetPosition(_aim);
                        _isMoving = true;
                        RemoveHighlightTiles();
                        var mmic = new NetMakeMoveInConfrontation
                        {
                            DestinationX = _defending.CurrentX,
                            DestinationY = _defending.CurrentY
                        };
                        Client.Instance.SendToServer(mmic);
                    }
                }
            }

            if (_isMoving)
            {
                if (Vector3.Distance(_defending.transform.position, _aim) < 0.01f)
                {
                    _isMoving = false;
                    SetAvailablePath();
                }
            }
        }
    }
    
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (var x = 0; x < TileCountX; x++)
        for (var y = 0; y < TileCountY; y++)
            if (_tiles[x, y] == hitInfo)
                return new Vector2Int(x, y);
        return -Vector2Int.one;
    }
    
    private void HighlightTiles()
    {
        foreach (var availableMove in _availableMoves)
            _tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("HighlightHard");
    }

    private void RemoveHighlightTiles()
    {
        foreach (var availableMove in _availableMoves)
            _tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("Tile");
        _availableMoves.Clear();
    }

    private void SetupChessBoard()
    {
        var defendingConfrontation = Confrontation.GetCurrentDefending();
        var attackingConfrontation = Confrontation.GetCurrentAttacking();
        _board = new ChessPiece[TileCountX, TileCountY];
        GenerateAllTiles(TileSize, TileCountX, TileCountY);
        _defending = Instantiate(Prefabs[(int)defendingConfrontation.Type - 1], transform).GetComponent<ChessPiece>();
        _defending.Type = defendingConfrontation.Type;
        _defending.Team = defendingConfrontation.Team;
        _defending.GetComponent<MeshRenderer>().material = defendingConfrontation.Team switch
        {
            1 => BlackMaterials[(int)defendingConfrontation.Type - 1],
            0 => WhiteMaterials[(int)defendingConfrontation.Type - 1],
            _ => defendingConfrontation.GetComponent<MeshRenderer>().material
        };
        _defending.CurrentX = 4;
        _defending.CurrentY = 4;
        _defending.SetPosition(GetTileCenter(4, 4));
        _attacking = Instantiate(Prefabs[(int)attackingConfrontation.Type - 1], transform).GetComponent<ChessPiece>();
        _attacking.Type = attackingConfrontation.Type;
        _attacking.Team = attackingConfrontation.Team;
        _attacking.GetComponent<MeshRenderer>().material = attackingConfrontation.Team switch
        {
            1 => BlackMaterials[(int)attackingConfrontation.Type - 1],
            0 => WhiteMaterials[(int)attackingConfrontation.Type - 1],
            _ => attackingConfrontation.GetComponent<MeshRenderer>().material
        };
        _attacking.CurrentX = 9;
        _attacking.CurrentY = 4;
        _attacking.SetPosition(GetTileCenter(9, 4));
        
        _board[4, 4] = _defending;
        if (!ConfrontationListener.IsAttacking) SetAvailablePath();
    }
    
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * TileSize, YOffset, y * TileSize) - _bounds + new Vector3(TileSize / 2, 0, TileSize / 2);
    }
    
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        YOffset += transform.position.y;
        _bounds = new Vector3((tileCountX / 2f) * tileSize, 0, (tileCountX / 2f) * tileSize) + BoardCenter;
        _tiles = new GameObject[tileCountX, tileCountY];
        for (var x = 0; x < tileCountX; x++)
        for (var y = 0; y < tileCountY; y++)
            _tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        var tileObject = new GameObject($"X:{x}, Y:{y}")
        {
            transform =
            {
                parent = transform
            }
        };
        var mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material  = TileMaterial;
        var vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, YOffset, y * tileSize) - _bounds;
        vertices[1] = new Vector3(x * tileSize, YOffset, (y + 1) * tileSize) - _bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, YOffset, y * tileSize) - _bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, YOffset, (y + 1) * tileSize) - _bounds;
        var tris = new [] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();
        return tileObject;
    }
}
