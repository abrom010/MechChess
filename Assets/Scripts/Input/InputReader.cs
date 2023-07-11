using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, GameInput.IBoardActions, GameInput.IBattleActions, GameInput.IUIActions
{
    private GameInput _gameInput;

    private void OnEnable()
    {
        if(_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Board.SetCallbacks(this);
            _gameInput.Battle.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            SetUI();
        }
    }

    public void SetBoard()
    {
        _gameInput.UI.Disable();
        _gameInput.Battle.Disable();
        _gameInput.Board.Enable();
    }

    public void SetBattle()
    {
        _gameInput.UI.Disable();
        _gameInput.Board.Disable();
        _gameInput.Battle.Enable();
    }

    public void SetUI()
    {
        _gameInput.Board.Disable();
        _gameInput.Battle.Disable();
        _gameInput.UI.Enable();
    }

    public event Action<Vector2> MoveEvent;

    public event Action RookSupportEvent;
    public event Action KnightSupportEvent;
    public event Action BishopSupportEvent;

    public event Action ClickEvent;
    public event Action RightClickEvent;
    public event Action RightClickCancelledEvent;

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRookSupport(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            RookSupportEvent?.Invoke();
        }
    }

    public void OnKnightSupport(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            KnightSupportEvent?.Invoke();
        }
    }

    public void OnBishopSupport(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            BishopSupportEvent?.Invoke();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            ClickEvent?.Invoke();
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            RightClickEvent?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            RightClickCancelledEvent?.Invoke();
        }
    }
}
