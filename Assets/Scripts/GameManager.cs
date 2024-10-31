using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Grid board;

    private BoardManager boardManager;

    private void Start()
    {
        boardManager = board.GetComponent<BoardManager>();
        boardManager.OnTileMove += BoardManager_OnTileMove;
        boardManager.OnMoveComplete += BoardManager_OnMoveComplete;
    }

    private void BoardManager_OnMoveComplete(object sender, EventArgs e)
    {
        // Perform all the necessary moves (largest addition)
    }

    private void BoardManager_OnTileMove(
        object sender, BoardManager.OnTileMoveEventArgs e)
    {
        string destName = e.DestTile.Tile == null ? "null" : e.DestTile.Tile.name;
        Debug.Log(e.OriginTile.Tile.name + " moved to " + destName);

        // Only if the origin and destination tiles are of the same type
        // we make an addition, otherwise ignore the move
        if (e.OriginTile.Tile == e.DestTile.Tile)
        {

        }
        else if (null == e.DestTile.Tile)
        {
            boardManager.PlaceTile(e.DestTile.Position, e.OriginTile.Tile);
            boardManager.RemoveTile(e.OriginTile.Position);
        }
    }
}
