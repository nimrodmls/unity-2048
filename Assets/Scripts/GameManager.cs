using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Serializable]
    private class TileCombination
    {
        public TileBase Origin;
        public TileBase Result;
    }

    [SerializeField] private TileBase tile2;
    [SerializeField] private TileBase tile4;
    [SerializeField] private List<TileCombination> combinations;
    [SerializeField] private Grid board;

    private BoardManager boardManager;
    private Dictionary<TileBase, TileBase> combinationMap;
    private bool isTurnMovePerformed = false;

    private void Awake()
    {
        combinationMap = new Dictionary<TileBase, TileBase>();

        // Converting the list of combinations to a dictionary
        foreach (TileCombination combination in combinations)
        {
            combinationMap.Add(combination.Origin, combination.Result);
        }
    }

    private void Start()
    {
        boardManager = board.GetComponent<BoardManager>();
        boardManager.OnTileMove += BoardManager_OnTileMove;
        boardManager.OnMoveComplete += BoardManager_OnMoveComplete;
    }

    private void BoardManager_OnMoveComplete(object sender, BoardManager.OnMoveCompleteEventArgs e)
    {
        if ((e.IsMovePerformed || isTurnMovePerformed) &&
            boardManager.TryGetRandomEmptyPosition(out Vector3Int emptyPosition))
        {
            // Randomly place a new tile on the board, 90% chance of 2 and 10% chance of 4
            int rand = UnityEngine.Random.Range(0, 10);
            if (rand < 9)
            {
                boardManager.PlaceTile(emptyPosition, tile2);
            }
            else
            {
                boardManager.PlaceTile(emptyPosition, tile4);
            }
        }

        isTurnMovePerformed = false;
    }

    private void BoardManager_OnTileMove(
        object sender, BoardManager.OnTileMoveEventArgs e)
    {
        // Only if the origin and destination tiles are of the same type
        // we make an addition, otherwise ignore the move
        if (e.OriginTile.Tile == e.DestTile.Tile)
        {
            boardManager.PlaceTile(e.DestTile.Position, combinationMap[e.DestTile.Tile]);
            boardManager.RemoveTile(e.OriginTile.Position);
            isTurnMovePerformed = true;
        }
    }
}
