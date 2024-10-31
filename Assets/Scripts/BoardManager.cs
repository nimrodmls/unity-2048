using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public event EventHandler<OnMoveCompleteEventArgs> OnMoveComplete;
    public class OnMoveCompleteEventArgs : EventArgs
    {
        public bool IsMovePerformed { get; set; }
    }

    public event EventHandler<OnTileMoveEventArgs> OnTileMove;
    public class OnTileMoveEventArgs : EventArgs
    {
        public class TileData
        {
            public TileBase Tile { get; set; }
            public Vector3Int Position { get; set; }
        }

        public TileData OriginTile { get; set; }
        public TileData DestTile { get; set; }
    }

    [SerializeField] private Tilemap boardBase;
    [SerializeField] private Tilemap board;

    public void PlaceTile(Vector3Int position, TileBase tile)
    {
        board.SetTile(position, tile);
    }

    public void RemoveTile(Vector3Int position)
    {
        board.SetTile(position, null);
    }

    public bool TryGetRandomEmptyPosition(out Vector3Int emptyPosition)
    {
        List<Vector3Int> empty_tiles = new List<Vector3Int>();
        foreach (Vector3Int position in boardBase.cellBounds.allPositionsWithin)
        {
            if (!board.HasTile(position))
            {
                empty_tiles.Add(position);
            }
        }

        // The board is full
        if (0 == empty_tiles.Count)
        {
            emptyPosition = Vector3Int.zero;
            return false;
        }

        emptyPosition = empty_tiles[UnityEngine.Random.Range(0, empty_tiles.Count)];
        return true;
    }

    private void Start()
    {
        Player.Instance.OnPlayerMove += Player_OnPlayerMove;
    }

    private void Player_OnPlayerMove(object sender, Player.OnMoveEventArgs e)
    {
        // First allowing all the tiles to move, to perform addition
        // Note that we assume that the board is a square! (x == y)
        PerformAdjacencyMoves(e.Direction);

        // Then aligning the tiles to the edges, as per the movement direction
        bool isMovePerformed = PerformEdgeAlignments(e.Direction);

        OnMoveComplete?.Invoke(
            this, new OnMoveCompleteEventArgs { IsMovePerformed = isMovePerformed });
    }

    private void PerformAdjacencyMoves(Player.MoveDirection direction)
    {
        for (int i = 0; i < boardBase.cellBounds.size.x; i++)
        {
            for (int j = 0; j < boardBase.cellBounds.size.y; j++)
            {
                Vector3Int destinationPosition = GetDirectionRelativeCellPosition(i, j, direction);

                if (board.HasTile(destinationPosition))
                {
                    Vector3Int originPosition = 
                        LookupNearestTilePosition(destinationPosition, direction);
                    if (board.HasTile(originPosition))
                    {
                        OnTileMove?.Invoke(
                            this,
                            new OnTileMoveEventArgs
                            {
                                OriginTile = new OnTileMoveEventArgs.TileData
                                {
                                    Tile = board.GetTile(originPosition),
                                    Position = originPosition
                                },
                                DestTile = new OnTileMoveEventArgs.TileData
                                {
                                    Tile = board.GetTile(destinationPosition),
                                    Position = destinationPosition
                                }
                            });
                    }
                }
            }
        }
    }

    private bool PerformEdgeAlignments(Player.MoveDirection direction)
    {
        bool isMovePerformed = false;
        for (int i = 0; i < boardBase.cellBounds.size.x; i++)
        {
            for (int j = 0; j < boardBase.cellBounds.size.y; j++)
            {
                Vector3Int destinationPosition = GetDirectionRelativeCellPosition(i, j, direction);

                // Only moving to vacant positions
                if (!board.HasTile(destinationPosition))
                {
                    Vector3Int originPosition = 
                        LookupNearestTilePosition(destinationPosition, direction);
                    if (board.HasTile(originPosition))
                    {
                        // Replacing the tile on the board
                        PlaceTile(destinationPosition, board.GetTile(originPosition));
                        RemoveTile(originPosition);
                        isMovePerformed = true;
                    }
                }
            }
        }

        return isMovePerformed;
    }

    private Vector3Int LookupNearestTilePosition(
        Vector3Int tilePosition, Player.MoveDirection direction)
    {
        Vector3Int currentPosition = tilePosition;
        for (int i = 0; i < boardBase.cellBounds.size.x; i++)
        {
            currentPosition -= GetDirectionVector(direction);
            if (board.HasTile(currentPosition))
            {
                return currentPosition;
            }
        }

        return currentPosition;
    }

    private Vector3Int GetDirectionRelativeCellPosition(
        int axis1, int axis2, Player.MoveDirection direction)
    {
        // If the direction is left/right, then the primary axis is X
        // (means that the outer loop is X axis, and inner is Y axis)
        // Otherwise it's up/down and the primary axis is Y
        switch (direction)
        {
            case Player.MoveDirection.Up:
                return boardBase.origin +
                    new Vector3Int(axis2, boardBase.cellBounds.size.y - axis1 - 1, 0);
            case Player.MoveDirection.Down:
                return boardBase.origin + new Vector3Int(axis2, axis1, 0);
            case Player.MoveDirection.Left:
                return boardBase.origin + new Vector3Int(axis1, axis2, 0);
            case Player.MoveDirection.Right:
                return boardBase.origin +
                    new Vector3Int(boardBase.cellBounds.size.x - axis1 - 1, axis2, 0);
            default:
                Debug.LogError("Invalid move direction");
                return Vector3Int.zero;
        }
    }

    private Vector3Int GetDirectionVector(Player.MoveDirection direction)
    {
        switch (direction)
        {
            case Player.MoveDirection.Up:
                return Vector3Int.up;
            case Player.MoveDirection.Down:
                return Vector3Int.down;
            case Player.MoveDirection.Left:
                return Vector3Int.left;
            case Player.MoveDirection.Right:
                return Vector3Int.right;
            default:
                return Vector3Int.zero;
        }
    }
}
