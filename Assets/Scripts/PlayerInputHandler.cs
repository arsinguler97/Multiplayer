using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    public string defaultMap;
    public string cannonMap;
    public string wheelMap;

    public bool usingCannon;
    public bool usingWheel;

    public CannonController cannon;
    public WheelController wheel;

    public event Action<PlayerInputHandler> RequestEnterCannon;
    public event Action<PlayerInputHandler> RequestExitCannon;
    public event Action<PlayerInputHandler> RequestEnterWheel;
    public event Action<PlayerInputHandler> RequestExitWheel;

    private PlayerInput _playerInput;
    private StarterAssetsInputs _inputs;
    private ThirdPersonController _motor;

    private float _rotateH;
    private float _rotateV;
    private float _turn;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputs = GetComponent<StarterAssetsInputs>();
        _motor = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        if (usingCannon)
        {
            if (_rotateH != 0) cannon.RotateHorizontal(_rotateH);
            if (_rotateV != 0) cannon.RotateVertical(_rotateV);
        }

        if (usingWheel)
        {
            if (_turn != 0) wheel.Turn(_turn);
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (usingCannon)
        {
            RequestExitCannon?.Invoke(this);
            return;
        }

        if (usingWheel)
        {
            RequestExitWheel?.Invoke(this);
            return;
        }

        _inputs.isInteracting = true;

        bool didRequest = false;

        if (RequestEnterCannon != null)
        {
            RequestEnterCannon.Invoke(this);
            didRequest = true;
        }

        if (RequestEnterWheel != null)
        {
            RequestEnterWheel.Invoke(this);
            didRequest = true;
        }

        if (!didRequest)
            _inputs.isInteracting = false;
    }
    
    public void OnRotateH(InputValue value)
    {
        _rotateH = value.Get<float>();
    }

    public void OnRotateV(InputValue value)
    {
        _rotateV = value.Get<float>();
    }

    public void OnFire(InputValue value)
    {
        if (!usingCannon) return;
        if (!value.isPressed) return;
        cannon.Fire();
    }

    public void OnExit(InputValue value)
    {
        if (!value.isPressed) return;

        if (usingCannon)
            RequestExitCannon?.Invoke(this);
        else if (usingWheel)
            RequestExitWheel?.Invoke(this);
    }

    public void OnTurn(InputValue value)
    {
        _turn = value.Get<float>();
    }

    public void EnterCannon(CannonController c)
    {
        usingCannon = true;
        cannon = c;
        _motor.enabled = false;

        _rotateH = 0;
        _rotateV = 0;

        _playerInput.SwitchCurrentActionMap(cannonMap);
    }

    public void ExitCannon()
    {
        usingCannon = false;
        cannon = null;
        _motor.enabled = true;

        _rotateH = 0;
        _rotateV = 0;

        _inputs.isInteracting = false;
        _playerInput.SwitchCurrentActionMap(defaultMap);
    }

    public void EnterWheel(WheelController w)
    {
        usingWheel = true;
        wheel = w;
        _motor.enabled = false;

        _turn = 0;

        _playerInput.SwitchCurrentActionMap(wheelMap);
    }

    public void ExitWheel()
    {
        usingWheel = false;
        wheel = null;
        _motor.enabled = true;

        _turn = 0;

        _inputs.isInteracting = false;
        _playerInput.SwitchCurrentActionMap(defaultMap);
    }
}
