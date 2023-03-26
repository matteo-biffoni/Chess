using ChessPieces;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material TileMaterial;
    [SerializeField] private float TileSize = 1f;
    [SerializeField] private float YOffset = 0.2f;
    [SerializeField] private Vector3 BoardCenter = Vector3.zero;
    
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private Material[] BlackMaterials;
    [SerializeField] private Material[] WhiteMaterials;
    
    // Logic
    private ChessPiece[,] _chessPieces;
    private const int TileCountX = 8;
    private const int TileCountY = 8;
    private GameObject[,] _tiles;
    private Camera _mainCamera;
    private Vector2Int _currentHover;
    private Vector3 _bounds;
    
    private void Awake()
    {
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
        if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            var hitPosition = LookupTileIndex(info.transform.gameObject);
            if (_currentHover == -Vector2Int.one)
            {
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (_currentHover != hitPosition)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            if (_currentHover != -Vector2Int.one)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                _currentHover = -Vector2Int.one;
            }
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
        _chessPieces[x, y].transform.position = GetTileCenter(x, y);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * TileSize, YOffset, y * TileSize) - _bounds + new Vector3(TileSize / 2, 0, TileSize / 2);
    }
    
    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (var x = 0; x < TileCountX; x++)
            for (var y = 0; y < TileCountY; y++)
                if (_tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        return -Vector2Int.one;
    }
}
