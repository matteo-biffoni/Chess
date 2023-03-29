using System;
using System.Collections.Generic;
using System.Linq;
using ChessPieces;
using UnityEngine;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class ChessBoard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material TileMaterial;
    [SerializeField] private float TileSize = 1f;
    [SerializeField] private float YOffset = 0.2f;
    [SerializeField] private Vector3 BoardCenter = Vector3.zero;
    [SerializeField] private float DeathSize = 0.03f;
    [SerializeField] private float DeathSpacing = 0.1f;
    [SerializeField] private GameObject VictoryScreen;
    
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private Material[] BlackMaterials;
    [SerializeField] private Material[] WhiteMaterials;
    
    // Logic
    private ChessPiece[,] _chessPieces;
    private ChessPiece _currentlyDragging;
    private List<Vector2Int> _availableMoves = new ();
    private readonly List<ChessPiece> _deadWhites = new ();
    private readonly List<ChessPiece> _deadBlacks = new ();
    private const int TileCountX = 8;
    private const int TileCountY = 8;
    private GameObject[,] _tiles;
    private Camera _mainCamera;
    private Vector2Int _currentHover;
    private Vector3 _bounds;
    private bool _isWhiteTurn;
    private SpecialMove _specialMove;
    private List<Vector2Int[]> _moveList = new ();
    
    private void Awake()
    {
        _isWhiteTurn = true;
        GenerateAllTiles(TileSize, TileCountX, TileCountY);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            var hitPosition = LookupTileIndex(info.transform.gameObject);
            if (_currentHover == -Vector2Int.one)
            {
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (_currentHover != hitPosition)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = ContainsValidMove(ref _availableMoves, _currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    if ((_chessPieces[hitPosition.x, hitPosition.y].Team == 0 && _isWhiteTurn) || (_chessPieces[hitPosition.x, hitPosition.y].Team == 1 && !_isWhiteTurn))
                    {
                        _currentlyDragging = _chessPieces[hitPosition.x, hitPosition.y];
                        _availableMoves = _currentlyDragging.GetAvailableMoves(ref _chessPieces, TileCountX, TileCountY);
                        _specialMove = _currentlyDragging.GetSpecialMoves(ref _chessPieces, ref _moveList, ref _availableMoves);
                        PreventCheck();
                        HighlightTiles();
                    }
                }
            }

            if (_currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                var previousPosition = new Vector2Int(_currentlyDragging.CurrentX, _currentlyDragging.CurrentY);
                var validMove = MoveTo(_currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                    _currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                _currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if (_currentHover != -Vector2Int.one)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = ContainsValidMove(ref _availableMoves, _currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                _currentHover = -Vector2Int.one;
            }

            if (_currentlyDragging && Input.GetMouseButtonUp(0))
            {
                _currentlyDragging.SetPosition(GetTileCenter(_currentlyDragging.CurrentX, _currentlyDragging.CurrentY));
                _currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        if (_currentlyDragging)
        {
            var horizontalPlane = new Plane(Vector3.up, Vector3.up * YOffset);
            if (horizontalPlane.Raycast(ray, out var distance))
                _currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * .6f);
        }
    }

    // Generate board
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
    
    // Pieces spawning
    private void SpawnAllPieces()
    {
        _chessPieces = new ChessPiece[TileCountX, TileCountY];
        const int whiteTeam = 0;
        const int blackTeam = 1;
        
        // White team
        _chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        _chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        _chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        _chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        _chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        _chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        _chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        _chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (var i = 0; i < TileCountX; i++)
            _chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        // Black team
        _chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        _chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        _chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        _chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        _chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        _chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        _chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        _chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (var i = 0; i < TileCountX; i++)
            _chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        var cp = Instantiate(Prefabs[(int) type - 1], transform).GetComponent<ChessPiece>();
        cp.Type = type;
        cp.Team = team;
        cp.GetComponent<MeshRenderer>().material = team switch
        {
            1 => BlackMaterials[(int)type - 1],
            0 => WhiteMaterials[(int)type - 1],
            _ => cp.GetComponent<MeshRenderer>().material
        };
        return cp;
    }
    
    // Positioning
    private void PositionAllPieces()
    {
        for(var x = 0; x < TileCountX; x++)
            for(var y = 0; y < TileCountY; y++)
                if (_chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        _chessPieces[x, y].CurrentX = x;
        _chessPieces[x, y].CurrentY = y;
        _chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * TileSize, YOffset, y * TileSize) - _bounds + new Vector3(TileSize / 2, 0, TileSize / 2);
    }
    
    // Highlight Tiles
    private void HighlightTiles()
    {
        foreach (var availableMove in _availableMoves)
            _tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("Highlight");
    }

    private void RemoveHighlightTiles()
    {
        foreach (var availableMove in _availableMoves)
            _tiles[availableMove.x, availableMove.y].layer = LayerMask.NameToLayer("Tile");
        _availableMoves.Clear();
    }
    
    // Checkmate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        VictoryScreen.SetActive(true);
        VictoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton()
    {
        VictoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        VictoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        VictoryScreen.SetActive(false);
        _currentlyDragging = null;
        _availableMoves.Clear();
        _moveList.Clear();
        for (var x = 0; x < TileCountX; x++)
            for (var y = 0; y < TileCountY; y++)
            {
                if (_chessPieces[x, y] != null)
                    Destroy(_chessPieces[x, y].gameObject);
                _chessPieces[x, y] = null;
            }
        foreach (var deadWhite in _deadWhites)
            Destroy(deadWhite.gameObject);
        foreach (var deadBlack in _deadBlacks)
            Destroy(deadBlack.gameObject);
        _deadWhites.Clear();
        _deadBlacks.Clear();
        
        SpawnAllPieces();
        PositionAllPieces();
        _isWhiteTurn = true;
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
    
    // Special Moves
    private void ProcessSpecialMove()
    {
        Vector2Int[] lastMove;
        switch (_specialMove)
        {
            case SpecialMove.EnPassant:
            {
                var newMove = _moveList[^1];
                var myPawn = _chessPieces[newMove[1].x, newMove[1].y];
                var targetPawnPosition = _moveList[^2];
                var enemyPawn = _chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];
                if (myPawn.CurrentX == enemyPawn.CurrentX)
                {
                    if (myPawn.CurrentY == enemyPawn.CurrentY - 1 || myPawn.CurrentY == enemyPawn.CurrentY + 1)
                    {
                        if (enemyPawn.Team == 0)
                        {
                            _deadWhites.Add(enemyPawn);
                            enemyPawn.SetScale(Vector3.one * DeathSize);
                            enemyPawn.SetPosition(new Vector3(8.5f * TileSize, YOffset, -1 * TileSize) - _bounds +
                                                  new Vector3(TileSize / 2, 0, TileSize / 2) + Vector3.forward * (DeathSpacing * _deadWhites.Count));
                        }
                        else
                        {
                            _deadBlacks.Add(enemyPawn);
                            enemyPawn.SetScale(Vector3.one * DeathSize);
                            enemyPawn.SetPosition(new Vector3(-1.5f * TileSize, YOffset, 8f * TileSize) - _bounds +
                                                  new Vector3(TileSize / 2, 0, TileSize / 2) + Vector3.back * (DeathSpacing * _deadBlacks.Count));
                        }
                        _chessPieces[enemyPawn.CurrentX, enemyPawn.CurrentY] = null;
                    }
                }

                break;
            }
            case SpecialMove.Promotion:
                lastMove = _moveList[^1];
                var target = _chessPieces[lastMove[1].x, lastMove[1].y];
                if (target.Type == ChessPieceType.Pawn)
                {
                    switch (target.Team)
                    {
                        case 0 when lastMove[1].y == 7:
                        {
                            var newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0);
                            newQueen.transform.position = _chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                            Destroy(_chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                            _chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                            PositionSinglePiece(lastMove[1].x, lastMove[1].y ,true);
                            break;
                        }
                        case 1 when lastMove[1].y == 0:
                        {
                            var newQueen = SpawnSinglePiece(ChessPieceType.Queen, 1);
                            newQueen.transform.position = _chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                            Destroy(_chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                            _chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                            PositionSinglePiece(lastMove[1].x, lastMove[1].y ,true);
                            break;
                        }
                    }
                }
                break;
            case SpecialMove.Castling:
            {
                lastMove = _moveList[^1];
                switch (lastMove[1].x)
                {
                    case 2:
                    {
                        ChessPiece rook;
                        switch (lastMove[1].y)
                        {
                            case 0:
                            {
                                rook = _chessPieces[0, 0];
                                _chessPieces[3, 0] = rook;
                                PositionSinglePiece(3, 0);
                                _chessPieces[0, 0] = null;
                                break;
                            }
                            case 7:
                                rook = _chessPieces[0, 7];
                                _chessPieces[3, 7] = rook;
                                PositionSinglePiece(3, 7);
                                _chessPieces[0, 7] = null;
                                break;
                        }

                        break;
                    }
                    case 6:
                    {
                        ChessPiece rook;
                        switch (lastMove[1].y)
                        {
                            case 0:
                            {
                                rook = _chessPieces[7, 0];
                                _chessPieces[5, 0] = rook;
                                PositionSinglePiece(5, 0);
                                _chessPieces[7, 0] = null;
                                break;
                            }
                            case 7:
                                rook = _chessPieces[7, 7];
                                _chessPieces[5, 7] = rook;
                                PositionSinglePiece(5, 7);
                                _chessPieces[7, 7] = null;
                                break;
                        }

                        break;
                    }
                }

                break;
            }
            case SpecialMove.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void PreventCheck()
    {
        ChessPiece targetKing = null;
        for (var x = 0; x < TileCountX; x++)
            for (var y = 0; y < TileCountY; y++)
                if (_chessPieces[x, y] != null)
                    if (_chessPieces[x, y].Type == ChessPieceType.King)
                        if (_chessPieces[x, y].Team == _currentlyDragging.Team)
                            targetKing = _chessPieces[x, y];

        SimulateMoveForSinglePiece(_currentlyDragging, ref _availableMoves, targetKing);
    }
    private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        var actualX = cp.CurrentX;
        var actualY = cp.CurrentY;
        var movesToRemove = new List<Vector2Int>();
        foreach (var move in moves)
        {
            var simX = move.x;
            var simY = move.y;
            var kingPositionThisSim = new Vector2Int(targetKing.CurrentX, targetKing.CurrentY);
            if (cp.Type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);
            var simulation = new ChessPiece[TileCountX, TileCountY];
            var simAttackingPieces = new List<ChessPiece>();
            for (var x = 0; x < TileCountX; x++)
                for (var y = 0; y < TileCountY; y++)
                    if (_chessPieces[x, y] != null)
                    {
                        simulation[x, y] = _chessPieces[x, y];
                        if (simulation[x, y].Team != cp.Team)
                            simAttackingPieces.Add(simulation[x, y]);
                    }
            simulation[actualX, actualY] = null;
            cp.CurrentX = simX;
            cp.CurrentY = simY;
            simulation[simX, simY] = cp;
            var deadPiece = simAttackingPieces.Find(c => c.CurrentX == simX && c.CurrentY == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);
            var simMoves = new List<Vector2Int>();
            foreach (var pieceMoves in simAttackingPieces.Select(simAttackingPiece => simAttackingPiece.GetAvailableMoves(ref simulation, TileCountX, TileCountY)))
            {
                simMoves.AddRange(pieceMoves);
            }
            if (ContainsValidMove(ref simMoves, kingPositionThisSim))
            {
                movesToRemove.Add(move);
            }
            cp.CurrentX = actualX;
            cp.CurrentY = actualY;
        }
        foreach (var moveToRemove in movesToRemove)
            moves.Remove(moveToRemove);
    }

    private bool CheckForCheckMate()
    {
        var lastMove = _moveList[^1];
        var targetTeam = _chessPieces[lastMove[1].x, lastMove[1].y].Team == 0 ? 1 : 0;
        var attackingPieces = new List<ChessPiece>();
        var defendingPieces = new List<ChessPiece>();
        ChessPiece targetKing = null;
        for (var x = 0; x < TileCountX; x++)
            for (var y = 0; y < TileCountY; y++)
                if (_chessPieces[x, y] != null)
                {
                    if (_chessPieces[x, y].Team == targetTeam)
                    {
                        defendingPieces.Add(_chessPieces[x, y]);
                        if (_chessPieces[x, y].Type == ChessPieceType.King)
                            targetKing = _chessPieces[x, y];
                    }
                    else
                    {
                        attackingPieces.Add(_chessPieces[x, y]);
                    }
                }
        var currentAvailableMoves = new List<Vector2Int>();
        foreach (var pieceMoves in attackingPieces.Select(attackingPiece => attackingPiece.GetAvailableMoves(ref _chessPieces, TileCountX, TileCountY)))
        {
            currentAvailableMoves.AddRange(pieceMoves);
        }
        if (targetKing == null)
        {
            Debug.Log("No king found");
            return false;
        }
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.CurrentX, targetKing.CurrentY)))
        {
            foreach (var defendingPiece in defendingPieces)
            {
                var defendingMoves = defendingPiece.GetAvailableMoves(ref _chessPieces, TileCountX, TileCountY);
                SimulateMoveForSinglePiece(defendingPiece, ref defendingMoves, targetKing);
                if (defendingMoves.Count != 0)
                    return false;
            }
            return true;
        }
        return false;
    }
    
    // Operations
    private static bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        return moves.Any(move => move.x == pos.x && move.y == pos.y);
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (var x = 0; x < TileCountX; x++)
            for (var y = 0; y < TileCountY; y++)
                if (_tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        return -Vector2Int.one;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if (!ContainsValidMove(ref _availableMoves, new Vector2Int(x, y)))
            return false;
        var previousPosition = new Vector2Int(_currentlyDragging.CurrentX, _currentlyDragging.CurrentY);
        if (_chessPieces[x, y] != null)
        {
            var ocp = _chessPieces[x, y];
            if (cp.Team == ocp.Team)
                return false;
            if (ocp.Team == 0)
            {
                if (ocp.Type == ChessPieceType.King)
                    CheckMate(1);
                _deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * DeathSize);
                ocp.SetPosition(new Vector3(8.5f * TileSize, YOffset, -1 * TileSize) - _bounds +
                                new Vector3(TileSize / 2, 0, TileSize / 2) + Vector3.forward * (DeathSpacing * _deadWhites.Count));
            }
            else
            {
                if (ocp.Type == ChessPieceType.King)
                    CheckMate(0);
                _deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * DeathSize);
                ocp.SetPosition(new Vector3(-1.5f * TileSize, YOffset, 8f * TileSize) - _bounds +
                                new Vector3(TileSize / 2, 0, TileSize / 2) + Vector3.back * (DeathSpacing * _deadBlacks.Count));
            }
        }
        _chessPieces[x, y] = cp;
        _chessPieces[previousPosition.x, previousPosition.y] = null;
        PositionSinglePiece(x, y);
        _isWhiteTurn = !_isWhiteTurn;
        _moveList.Add(new [] { previousPosition, new (x, y)});
        ProcessSpecialMove();
        if (CheckForCheckMate())
            CheckMate(cp.Team);
        return true;
    }
}
