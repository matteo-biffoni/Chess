using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChessPieces;
using Net;
using Net.NetMessages;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class ChessBoardConfrontation : MonoBehaviour
{
    private ChessPiece _attacking;
    private ChessPiece _defending;
    private ChessPiece _defendingConfrontation;

    private static ChessBoardConfrontation Instance { get; set; }

    [SerializeField] private GameObject BackgroundGo;
    [SerializeField] private AttackPanelManager AttackPanelManager;

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
    private bool _timerTextAnimation;
    [SerializeField] private Slider HpSlider;
    [SerializeField] private int FireDuration = 5;
    [SerializeField] private KeyCode SpecialAttack1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode SpecialAttack2Key = KeyCode.Alpha2;
    [SerializeField] private KeyCode SpecialAttack3Key = KeyCode.Alpha3;

    [SerializeField] private GameObject MovableTexturePrefab;
    [SerializeField] private GameObject FiredTexturePrefab;

    [SerializeField] private MinigameEffectsManager MinigameEffectsManager;

    [SerializeField] private AudioSource DamagingAudioSource;
    [SerializeField] private AudioSource MovementAudioSource;

    [SerializeField] private Sprite Ant;
    [SerializeField] private Sprite Beetle;
    [SerializeField] private Sprite Butterfly;
    [SerializeField] private Sprite Ladybug;
    [SerializeField] private Sprite Mantis;
    [SerializeField] private Sprite Bumblebee;
    [SerializeField] private Sprite Tomato;
    [SerializeField] private Sprite Leek;
    [SerializeField] private Sprite Carrot;
    [SerializeField] private Sprite Zuchini;
    [SerializeField] private Sprite Eggplant;
    [SerializeField] private Sprite Broccoli;
    [SerializeField] private Image SliderImage;

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
        AudioManager.Instance.SetMinigameBackgroundSource(GetComponent<AudioSource>());
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
        HpSlider.value = _defending.GetHp();
        //Debug.Log($"Hp slider value: {HpSlider.value}");
        //Debug.Log($"Defender hp: {_defending.GetHp()}");
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
            StartCoroutine(FireCell(new Vector2Int(nnaic.DestinationX, nnaic.DestinationY), _attacking.Team == 0 ? AttackType.NormalBugs : AttackType.NormalVegs, 0));
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
            MovementAudioSource.Play();
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

    private IEnumerator FireCell(Vector2Int cell, AttackType attackType, int which)
    {
        yield return MinigameEffectsManager.SpawnProjectile(GetTileCenter(cell.x, cell.y));
        AudioManager.Instance.PlayClip(which != 0 ? SoundClip.SpecialAttack : SoundClip.NormalAttack);
        StartCoroutine(MinigameEffectsManager.SpawnAttack(attackType, GetTileCenter(cell.x, cell.y)));
        StartCoroutine(ResetNormalAttackAfterInterval());
        if (ConfrontationListener.IsAttacking)
        {
            switch (attackType)
            {
                case AttackType.NormalBugs or AttackType.NormalVegs:
                    StartCoroutine(AttackPanelManager.Attack(0, NormalAttackInterval));
                    break;
                case AttackType.SpecialBugs or AttackType.SpecialVegs:
                    switch (which)
                    {
                        case 1:
                            StartCoroutine(ResetSpecialAttack1AfterInterval());
                            StartCoroutine(AttackPanelManager.Attack(1, SpecialAttack1Interval));
                            break;
                        case 2:
                            StartCoroutine(ResetSpecialAttack2AfterInterval());
                            StartCoroutine(AttackPanelManager.Attack(2, SpecialAttack2Interval));
                            break;
                        case 3:
                            StartCoroutine(ResetSpecialAttack3AfterInterval());
                            StartCoroutine(AttackPanelManager.Attack(3, SpecialAttack3Interval));
                            break;
                    }

                    break;
            }
        }

        // Mettere preavviso
        _firedCells[cell.x, cell.y]++;
        _tiles[cell.x, cell.y].transform.GetComponent<TileInConfrontationHandler>().SetFired(true);
        //_tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("FiredCell");
        StartCoroutine(UnFireCellAfterXSec(cell, FireDuration));
    }

    private void SpecialAttack1(Vector2Int cell)
    {
        if (_attacking.Type == ChessPieceType.None) return;
        foreach (var firedCell in _attacking.GetSpecialAttack1Cells(cell, TileCountX, TileCountY))
            StartCoroutine(FireCell(firedCell, _attacking.Team == 0 ? AttackType.SpecialBugs : AttackType.SpecialVegs, 1));
    }

    private void SpecialAttack2(Vector2Int cell)
    {
        if (_attacking.Type is ChessPieceType.Pawn or ChessPieceType.None) return;
        foreach (var firedCell in _attacking.GetSpecialAttack2Cells(cell, TileCountX, TileCountY))
            StartCoroutine(FireCell(firedCell, _attacking.Team == 0 ? AttackType.SpecialBugs : AttackType.SpecialVegs, 2));
    }
    

    private void SpecialAttack3(Vector2Int cell)
    {
        if (_attacking.Type != ChessPieceType.Queen) return;
        foreach (var firedCell in _attacking.GetSpecialAttack3Cells(cell, TileCountX, TileCountY))
            StartCoroutine(FireCell(firedCell, _attacking.Team == 0 ? AttackType.SpecialBugs : AttackType.SpecialVegs, 3));
    }

    private IEnumerator UnFireCellAfterXSec(Vector2Int cell, int sec)
    {
        yield return new WaitForSeconds(sec);
        _firedCells[cell.x, cell.y]--;
        if (_firedCells[cell.x, cell.y] == 0)
            _tiles[cell.x, cell.y].transform.GetComponent<TileInConfrontationHandler>().SetFired(false);
        /*if (_firedCells[cell.x, cell.y] == 0)
            _tiles[cell.x, cell.y].layer = LayerMask.NameToLayer("Tile");*/
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
            if (Input.GetKey(SpecialAttack1Key) && _attacking.Type != ChessPieceType.Pawn)
                _selectedSpecialAttack = 1;
            else if (Input.GetKey(SpecialAttack2Key) &&
                     _attacking.Type != ChessPieceType.Pawn &&
                     _attacking.Type != ChessPieceType.King &&
                     _attacking.Type != ChessPieceType.Knight)
                _selectedSpecialAttack = 2;
            else if (Input.GetKey(SpecialAttack3Key) && _attacking.Type == ChessPieceType.Queen)
                _selectedSpecialAttack = 3;
            else
                _selectedSpecialAttack = 0;
            if (_selectedSpecialAttack != AttackPanelManager.GetSelectedAttack())
            {
                AttackPanelManager.SelectAttack(_selectedSpecialAttack);
            }
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
                            StartCoroutine(FireCell(hitPosition, _attacking.Team == 0 ? AttackType.NormalBugs : AttackType.NormalVegs, 0));
                            var nnaic = new NetNormalAttackInConfrontation
                            {
                                DestinationX = hitPosition.x,
                                DestinationY = hitPosition.y
                            };
                            Client.Instance.SendToServer(nnaic);
                            break;
                        case 1 when _canFireSpecialAttack1:
                            _canFireSpecialAttack1 = false;
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
                    if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile")))
                    /*if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("HighlightHard", "FiredCell")))*/
                    {
                        var hitPosition = LookupTileIndex(info.transform.gameObject);
                        if (_availableMoves.Contains(hitPosition))
                        {
                            _aim = GetTileCenter(hitPosition.x, hitPosition.y);
                            _defending.CurrentX = hitPosition.x;
                            _defending.CurrentY = hitPosition.y;
                            _defending.SetConfrontationAim(_aim);
                            _isMoving = true;
                            MovementAudioSource.Play();
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
            if (CheckForDamage(GetCellForPlayerPosition()))
            {
                if (!DamagingAudioSource.isPlaying)
                    DamagingAudioSource.Play();
            }
            else
            {
                if (DamagingAudioSource.isPlaying)
                    DamagingAudioSource.Stop();
            }
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

    private bool CheckForDamage(Vector2Int tile)
    {
        if (_firedCells[tile.x, tile.y] != 0)
        {
            _defending.DamagePiece();
            // Mandare messaggio all'attaccante
            Client.Instance.SendToServer(new NetHitInConfrontation());
            return true;
        }
        return false;
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
            _tiles[availableMove.x, availableMove.y].transform.GetComponent<TileInConfrontationHandler>()
                .SetMovable(true);
            // Aggiungere un layer per far capire che ci puoi andare ma che è a fuoco
            /*if (_tiles[availableMove.x, availableMove.y].layer == LayerMask.NameToLayer("Tile"))
                _tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("HighlightHard");*/
    }

    private void RemoveHighlightTiles()
    {
        foreach (var availableMove in _availableMoves)
            _tiles[availableMove.x, availableMove.y].transform.GetComponent<TileInConfrontationHandler>()
                .SetMovable(false);
            //_tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("Tile");
        _availableMoves.Clear();
    }

    private void SetupChessBoard()
    {
        var i = Confrontation.GetCurrentDefending().CurrentX;
        var j = Confrontation.GetCurrentDefending().CurrentY;
        BackgroundGo.transform.position = new Vector3(7 - 2 * i, 0f, -3 - 2 * j);
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
        MinigameEffectsManager.SetStartingPoint(GetTileCenter(9, 4));
        
        _board[4, 4] = _defending;
        if (!ConfrontationListener.IsAttacking)
        {
            SetAvailablePath();
            StartCoroutine(CheckForDamageWrapper());
            Destroy(AttackPanelManager.gameObject);
        }
        else
        {
            var numberOfAttacksToRemove = _attacking.Type switch
            {
                ChessPieceType.Pawn => 3,
                ChessPieceType.Knight => 2,
                ChessPieceType.King => 2,
                ChessPieceType.Queen => 0,
                _ => 1
            };
            AttackPanelManager.EnableAppropriateAttacks(numberOfAttacksToRemove);
            AttackPanelManager.SetAttackingType(_attacking.Type);
        }
        HpSlider.value = _defending.GetHp();
        SetSpriteFromDefendingPieceType(_defending.Team, _defending.Type);
        //Debug.Log($"Initial hp slider value: {HpSlider.value}");
        //Debug.Log($"Initial defender hp: {_defending.GetHp()}");
        StartCoroutine(MiniGameTimer());
    }

    private void SetSpriteFromDefendingPieceType(int team, ChessPieceType pieceType)
    {
        switch (pieceType)
        {
            case ChessPieceType.None:
                break;
            case ChessPieceType.Pawn:
                SliderImage.sprite = team == 0 ? Ant : Tomato;
                break;
            case ChessPieceType.Rook:
                SliderImage.sprite = team == 0 ? Beetle : Leek;
                break;
            case ChessPieceType.Knight:
                SliderImage.sprite = team == 0 ? Butterfly : Carrot;
                break;
            case ChessPieceType.Bishop:
                SliderImage.sprite = team == 0 ? Ladybug : Zuchini;
                break;
            case ChessPieceType.Queen:
                SliderImage.sprite = team == 0 ? Mantis : Eggplant;
                break;
            case ChessPieceType.King:
                SliderImage.sprite = team == 0 ? Bumblebee : Broccoli;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null);
        }
    }

    private IEnumerator MiniGameTimer()
    {
        var timeElapsed = MiniGameDuration;
        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            TimerText.text = ((int) timeElapsed).ToString();
            if (!_timerTextAnimation && timeElapsed < 6)
            {
                _timerTextAnimation = true;
                StartCoroutine(TimerTextAnimation(13f));
            }
            yield return null;
        }
        _timeElapsed = true;
    }

    private IEnumerator TimerTextAnimation(float speed)
    {
        while (true)
        {
            while (Vector3.Distance(TimerText.transform.localScale, new Vector3(1.3f, 1.3f, 1.3f)) > 0.01f)
            {
                TimerText.transform.localScale = Vector3.Lerp(TimerText.transform.localScale,
                    new Vector3(1.3f, 1.3f, 1.3f), Time.deltaTime * speed);
                yield return null;
            }

            while (Vector3.Distance(TimerText.transform.localScale, new Vector3(0.7f, 0.7f, 0.7f)) > 0.01f)
            {
                TimerText.transform.localScale = Vector3.Lerp(TimerText.transform.localScale,
                    new Vector3(0.7f, 0.7f, 0.7f), Time.deltaTime * speed);
                yield return null;
            }

            if (Confrontation.GetCurrentOutcome() != Outcome.NotAvailable)
                break;
        }
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
        tileObject.AddComponent<TileInConfrontationHandler>();
        
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
        var bCollider = tileObject.AddComponent<BoxCollider>();
        var movableTextureObj = Instantiate(MovableTexturePrefab, tileObject.transform);
        //Debug.Log($"Tile object: {tileObject.transform.localPosition}");
        //Debug.Log($"Movable before: {movableTextureObj.transform.position}");
        movableTextureObj.transform.position = bCollider.center;
        //Debug.Log($"Movable after: {movableTextureObj.transform.position}");
        var firedTextureObj = Instantiate(FiredTexturePrefab, tileObject.transform);
        firedTextureObj.transform.position = bCollider.center;
        return tileObject;
    }
}
