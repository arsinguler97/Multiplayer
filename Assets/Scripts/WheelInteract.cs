using UnityEngine;
using StarterAssets;

public class WheelInteract : MonoBehaviour
{
    public WheelController wheel;

    private PlayerInputHandler _handler;
    private StarterAssetsInputs _inputs;

    private bool _subscribed;
    private bool _inUse;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_subscribed) return;

        var h = other.GetComponent<PlayerInputHandler>();
        if (h == null) return;

        _handler = h;
        _inputs = other.GetComponent<StarterAssetsInputs>();

        _handler.RequestEnterWheel += TryEnter;
        _handler.RequestExitWheel += TryExit;

        _subscribed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!_subscribed) return;

        Unsubscribe();
    }

    private void TryEnter(PlayerInputHandler p)
    {
        if (!_inputs.isInteracting) return;
        if (p != _handler) return;
        if (_inUse) return;

        _inUse = true;
        p.EnterWheel(wheel);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (!_inUse) return;
        if (p != _handler) return;

        _inUse = false;
        p.ExitWheel();
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;

        _handler.RequestEnterWheel -= TryEnter;
        _handler.RequestExitWheel -= TryExit;

        _handler = null;
        _inputs = null;

        _inUse = false;
        _subscribed = false;
    }
}