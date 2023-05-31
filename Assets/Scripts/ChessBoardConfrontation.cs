using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChessPieces;
using Net;
using Net.NetMessages;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;

[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class ChessBoardConfrontation : MonoBehaviour
{
    private ChessPiece _attacking;
    private ChessPiece _defending;
    private ChessPiece _defendingConfrontation;

    public static ChessBoardConfrontation Instance { get; set; }

    [SerializeField] private Material TileMaterial;
    [SerializeField] private float TileSize = 1f;
    [SerializeField] private float YOffset = 0.2f;
    private const int TileCountX = 8;
    private const int TileCountY = 8;
    [SerializeField] private Vector3 BoardCenter = Vector3.zero;
    [SerializeField] private float NormalAttackInterval = 1f;
    [SerializeField] private float SpecialAttack1Interval = 3f;
    [SerializeField] private float SpecialAttack2Interval = 3f;
    [SerializeField] private float SpecialAttack3Interval = 10f;
    
    [SerializeField] private GameObject[] BugsPrefabs;
    [SerializeField] private GameObject[] VegetablesPrefabs;
    [SerializeField] private float MiniGameDuration = 10f;

    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text DefendingHpText;
    [SerializeField] private int FireDuration = 5;
    [SerializeField] private KeyCode SpecialAttack1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode SpecialAttack2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode SpecialAttack3Key = KeyCode.Alpha3;

    private bool _timeElapsed;
    
    private GameObject[,] _tiles;
    private Vector3 _bounds;
    private bool _canFireNormalAttack = true;
    private bool _canFireSpecialAttack1 = true;
    private bool _canFireSpecialAttack2 = true;
    private bool _canFireSpecialAttack3 = true;
    
    private ChessPiece[,] _board;

    private bool _isMoving;

    private List<Vector2Int> _availableMoves;

    private Vector3 _aim;

    private Camera _mainCamera;

    private int[,] _firedCells;

    private uint _selectedSpecialAttack;

    private void Awake()
    {
        Instance = this;
        SetupChessBoard();
        RegisterEvents();
    }

    private static void RegisterEvents()
    {
        NetUtility.CMakeMoveInConfrontation += Instance.OnMakeMoveInConfrontationClient;
        NetUtility.CNormalAttackInConfrontation += Instance.OnNormalAttackInConfrontationClient;
        NetUtility.CHitInConfrontation += Instance.OnHitInConfrontationClient;
        NetUtility.CSpecialAttackInConfrontation += Instance.OnSpecialAttackInConfrontationClient;
        NetUtility.SMakeMoveInConfrontation += Instance.OnMakeMoveInConfrontationServer;
        NetUtility.SNormalAttackInConfrontation += Instance.OnNormalAttackInConfrontationServer;
        NetUtility.SHitInConfrontation += Instance.OnHitInConfrontationServer;
        NetUtility.SSpecialAttackInConfrontation += Instance.OnSpecialAttackInConfrontationServer;
    }

    public static void UnregisterEvents()
    {
        NetUtility.CMakeMoveInConfrontation -= Instance.OnMakeMoveInConfrontationClient;
        NetUtility.CNormalAttackInConfrontation -= Instance.OnNormalAttackInConfrontationClient;
        NetUtility.CHitInConfrontation -= Instance.OnHitInConfrontationClient;
        NetUtility.CSpecialAttackInConfrontation -= Instance.OnSpecialAttackInConfrontationClient;
        NetUtility.SMakeMoveInConfrontation -= Instance.OnMakeMoveInConfrontationServer;
        NetUtility.SNormalAttackInConfrontation -= Instance.OnNormalAttackInConfrontationServer;
        NetUtility.SHitInConfrontation -= Instance.OnHitInConfrontationServer;
        NetUtility.SSpecialAttackInConfrontation -= Instance.OnSpecialAttackInConfrontationServer;
    }

    private void OnSpecialAttackInConfrontationClient(NetMessage msg)
    {
        if (msg is not NetSpecialAttackInConfrontation nsaic)
        {
            Debug.LogError("[C] Could not cast NetMessage to NetSpecialAttackInConfrontation");
            return;
        }
        if (!ConfrontationListener.IsAttacking)
        {
            switch (nsaic.SpecialAttackI)
            {
                case 1:
                    SpecialAttack1(new Vector2Int(nsaic.CellX, nsaic.CellY));
                    break;
                case 2:
                    SpecialAttack2(new Vector2Int(nsaic.CellX, nsaic.CellY));
                    break;
                case 3:
                    SpecialAttack3(new Vector2Int(nsaic.CellX, nsaic.CellY));
                    break;
            }
        }
    }

    private void OnHitInConfrontationClient(NetMessage msg)
    {
        if (msg is not NetHitInConfrontation)
        {
            Debug.LogError("[C] Could not cast NetMessage to NetHitInConfrontation");
            return;
        }
        if (ConfrontationListener.IsAttacking)
        {
            _defending.DamagePiece();
        }
        DefendingHpText.text = "Piece hp: " + _defending.GetHp();
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
            _defending.SetConfrontationAim(_aim);
        }
    }

    private void OnSpecialAttackInConfrontationServer(NetMessage msg, NetworkConnection cnn)
    {
        if (msg is not NetSpecialAttackInConfrontation nsaic)
        {
            Debug.LogError("[S] Could not cast NetMessage to NetSpecialAttackInConfrontation");
            return;
        }
        Server.Instance.Broadcast(nsaic);
    }

    private void OnHitInConfrontationServer(NetMessage msg, NetworkConnection cnn)
    {
        if (msg is not NetHitInConfrontation nhic)
        {
            Debug.LogError("[S] Could not cast NetMessage to NetHitInConfrontation");
            return;
        }
        Server.Instance.Broadcast(nhic);
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
        _firedCells[cell.x, cell.y]++;
        _tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("FiredCell");
        StartCoroutine(UnFireCellAfterXSec(cell, FireDuration));
    }

    private void SpecialAttack1(Vector2Int cell)
    {
        if (_attacking.Type == ChessPieceType.None) return;
        foreach (var firedCell in _attacking.GetSpecialAttack1Cells(cell, TileCountX, TileCountY))
            FireCell(firedCell);
    }

    private void SpecialAttack2(Vector2Int cell)
    {
        if (_attacking.Type is ChessPieceType.Pawn or ChessPieceType.None) return;
        foreach (var firedCell in _attacking.GetSpecialAttack2Cells(cell, TileCountX, TileCountY))
            FireCell(firedCell);
    }
    

    private void SpecialAttack3(Vector2Int cell)
    {
        if (_attacking.Type != ChessPieceType.Queen) return;
        foreach (var firedCell in _attacking.GetSpecialAttack3Cells(cell, TileCountX, TileCountY))
            FireCell(firedCell);
    }

    private IEnumerator UnFireCellAfterXSec(Vector2Int cell, int sec)
    {
        yield return new WaitForSeconds(sec);
        _firedCells[cell.x, cell.y]--;
        if (_firedCells[cell.x, cell.y] == 0)
            _tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("Tile");
        if (!ConfrontationListener.IsAttacking) SetAvailablePath();
    }

    private IEnumerator ResetNormalAttackAfterInterval()
    {
        yield return new WaitForSeconds(NormalAttackInterval);
        _canFireNormalAttack = true;
    }

    private IEnumerator ResetSpecialAttack1AfterInterval()
    {
        yield return new WaitForSeconds(SpecialAttack1Interval);
        _canFireSpecialAttack1 = true;
    }

    private IEnumerator ResetSpecialAttack2AfterInterval()
    {
        yield return new WaitForSeconds(SpecialAttack2Interval);
        _canFireSpecialAttack2 = true;
    }

    private IEnumerator ResetSpecialAttack3AfterInterval()
    {
        yield return new WaitForSeconds(SpecialAttack3Interval);
        _canFireSpecialAttack3 = true;
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

            if (Input.GetKey(SpecialAttack1Key))
                _selectedSpecialAttack = 1;
            else if (Input.GetKey(SpecialAttack2Key) && _attacking.Type != ChessPieceType.Pawn)
                _selectedSpecialAttack = 2;
            else if (Input.GetKey(SpecialAttack3Key) && _attacking.Type == ChessPieceType.Queen)
                _selectedSpecialAttack = 3;
            else
                _selectedSpecialAttack = 0;
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile")))
                {
                    var hitPosition = LookupTileIndex(info.transform.gameObject);
                    switch (_selectedSpecialAttack)
                    {
                        case 0 when _canFireNormalAttack:
                            _canFireNormalAttack = false;
                            StartCoroutine(ResetNormalAttackAfterInterval());
                            FireCell(hitPosition);
                            var nnaic = new NetNormalAttackInConfrontation
                            {
                                DestinationX = hitPosition.x,
                                DestinationY = hitPosition.y
                            };
                            Client.Instance.SendToServer(nnaic);
                            break;
                        case 1 when _canFireSpecialAttack1:
                            _canFireSpecialAttack1 = false;
                            StartCoroutine(ResetSpecialAttack1AfterInterval());
                            SpecialAttack1(hitPosition);
                            var nsaic1 = new NetSpecialAttackInConfrontation
                            {
                                CellX = hitPosition.x,
                                CellY = hitPosition.y,
                                SpecialAttackI = 1
                            };
                            Client.Instance.SendToServer(nsaic1);
                            break;
                        case 2 when _canFireSpecialAttack2:
                            _canFireSpecialAttack2 = false;
                            StartCoroutine(ResetSpecialAttack2AfterInterval());
                            SpecialAttack2(hitPosition);
                            var nsaic2 = new NetSpecialAttackInConfrontation
                            {
                                CellX = hitPosition.x,
                                CellY = hitPosition.y,
                                SpecialAttackI = 2
                            };
                            Client.Instance.SendToServer(nsaic2);
                            break;
                        case 3 when _canFireSpecialAttack3:
                            _canFireSpecialAttack3 = false;
                            StartCoroutine(ResetSpecialAttack3AfterInterval());
                            SpecialAttack3(hitPosition);
                            var nsaic3 = new NetSpecialAttackInConfrontation
                            {
                                CellX = hitPosition.x,
                                CellY = hitPosition.y,
                                SpecialAttackI = 3
                            };
                            Client.Instance.SendToServer(nsaic3);
                            break;
                    }
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
                    if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("HighlightHard", "FiredCell")))
                    {
                        var hitPosition = LookupTileIndex(info.transform.gameObject);
                        if (_availableMoves.Contains(hitPosition))
                        {
                            _aim = GetTileCenter(hitPosition.x, hitPosition.y);
                            _defending.CurrentX = hitPosition.x;
                            _defending.CurrentY = hitPosition.y;
                            _defending.SetConfrontationAim(_aim);
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

    private Vector2Int GetCellForPlayerPosition()
    {
        var dist = float.MaxValue;
        var cell = new Vector2Int(-1, -1);
        for (var x = 0; x < TileCountX; x++)
        {
            for (var y = 0; y < TileCountY; y++)
            {
                var actDist = Vector3.Distance(GetTileCenter(x, y), _defending.transform.position);
                if (dist > actDist)
                {
                    dist = actDist;
                    cell = new Vector2Int(x, y);
                } 
            }
        }
        return cell;
    }

    private IEnumerator CheckForDamageWrapper()
    {
        while (true)
        {
            CheckForDamage(GetCellForPlayerPosition());
            // SE pezzo morto o fine gioco uscire dal ciclo
            if (_defending.IsDead() || _timeElapsed) break;
            yield return new WaitForSeconds(0.05f);
        }
        // SE siamo fuori dal ciclo perchè è morto il pezzo => WIN
        // Altrimenti => LOSE
        _defendingConfrontation.SetHp(_defending.GetHp());
        Confrontation.SetCurrentOutcome(_defending.IsDead() ? Outcome.Success : Outcome.Failure);
        yield return null;
    }

    private void CheckForDamage(Vector2Int tile)
    {
        if (_firedCells[tile.x, tile.y] != 0)
        {
            _defending.DamagePiece();
            // Mandare messaggio all'attaccante
            Client.Instance.SendToServer(new NetHitInConfrontation());
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
            // Aggiungere un layer per far capire che ci puoi andare ma che è a fuoco
            if (_tiles[availableMove.x, availableMove.y].layer == LayerMask.NameToLayer("Tile"))
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
        _defendingConfrontation = Confrontation.GetCurrentDefending();
        var attackingConfrontation = Confrontation.GetCurrentAttacking();
        _board = new ChessPiece[TileCountX, TileCountY];
        _firedCells = new int[TileCountX, TileCountY];
        GenerateAllTiles(TileSize, TileCountX, TileCountY);
        
        _defending = Instantiate(_defendingConfrontation.Team == 0 ? BugsPrefabs[(int) _defendingConfrontation.Type - 1] : VegetablesPrefabs[(int) _defendingConfrontation.Type - 1], transform).GetComponent<ChessPiece>();
        _defending.Type = _defendingConfrontation.Type;
        _defending.Team = _defendingConfrontation.Team;
        _defending.SetHp(_defendingConfrontation.GetHp());
        _defending.CurrentX = 4;
        _defending.CurrentY = 4;
        _defending.SetConfrontationAim(GetTileCenter(4, 4));
        
        _attacking = Instantiate(attackingConfrontation.Team == 0 ? BugsPrefabs[(int) attackingConfrontation.Type - 1] : VegetablesPrefabs[(int) attackingConfrontation.Type - 1], transform).GetComponent<ChessPiece>();
        _attacking.Type = attackingConfrontation.Type;
        _attacking.Team = attackingConfrontation.Team;
        _attacking.CurrentX = 9;
        _attacking.CurrentY = 4;
        _attacking.SetConfrontationAim(GetTileCenter(9, 4));
        
        _board[4, 4] = _defending;
        if (!ConfrontationListener.IsAttacking)
        {
            SetAvailablePath();
            StartCoroutine(CheckForDamageWrapper());
        }
        DefendingHpText.text = "Piece hp: " + _defending.GetHp();
        StartCoroutine(MiniGameTimer());
    }

    private IEnumerator MiniGameTimer()
    {
        var timeElapsed = MiniGameDuration;
        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            TimerText.text = "Timer: " + (int) timeElapsed;
            yield return null;
        }
        _timeElapsed = true;
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
