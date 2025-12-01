using UnityEngine;
using StarterAssets;

public class SailInteract : MonoBehaviour
{
    public SailController sail;

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

        _handler.RequestEnterSail += TryEnter;
        _handler.RequestExitSail += TryExit;

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
        p.EnterSail(sail);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (!_inUse) return;
        if (p != _handler) return;

        _inUse = false;
        p.ExitSail();
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;

        _handler.RequestEnterSail -= TryEnter;
        _handler.RequestExitSail -= TryExit;

        _handler = null;
        _inputs = null;

        _inUse = false;
        _subscribed = false;
    }
}