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

    [SerializeField] private List<TileCombination> combinations;
    [SerializeField] private Grid board;

    private BoardManager boardManager;
    private Dictionary<TileBase, TileBase> combinationMap;

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
        }
    }
}
