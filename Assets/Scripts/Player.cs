using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { private set; get; }

    public event EventHandler<OnMoveEventArgs> OnPlayerMove;
    public class OnMoveEventArgs : EventArgs
    {
        public MoveDirection Direction { get; set; }
    }

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private GameInput gameInput;

    private void Awake()
    {
        if (null != Instance)
        {
            Debug.LogError("Player instance already exists!");
        }

        Instance = this;
    }

    private void Start()
    {
        gameInput.OnMove += GameInput_OnMove;
    }

    private void GameInput_OnMove(object sender, GameInput.OnMoveEventArgs e)
    {
        OnMoveEventArgs onMoveEventArgs = new OnMoveEventArgs();
        if (Vector2.down == e.Movement)
        {
            onMoveEventArgs.Direction = MoveDirection.Down;
        }
        else if (Vector2.up == e.Movement)
        {
            onMoveEventArgs.Direction = MoveDirection.Up;
        }
        else if (Vector2.left == e.Movement)
        {
            onMoveEventArgs.Direction = MoveDirection.Left;
        }
        else if (Vector2.right == e.Movement)
        {
            onMoveEventArgs.Direction = MoveDirection.Right;
        }

        OnPlayerMove?.Invoke(this, onMoveEventArgs);
    }
}
