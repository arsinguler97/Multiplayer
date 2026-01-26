using UnityEngine;
using StarterAssets;

public class SailInteract : MonoBehaviour
{
    public SailController sail;

    private PlayerInputHandler _handler;
    private StarterAssetsInputs _inputs;
    private bool _inUse;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var h = other.GetComponent<PlayerInputHandler>();
        if (h == null) return;

        if (_inUse && h != _handler) return;

        _handler = h;
        _inputs = other.GetComponent<StarterAssetsInputs>();

        _handler.RequestEnterSail -= TryEnter;
        _handler.RequestExitSail -= TryExit;

        _handler.RequestEnterSail += TryEnter;
        _handler.RequestExitSail += TryExit;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_handler != null && other.GetComponent<PlayerInputHandler>() == _handler)
        {
            if (_inUse)
                _handler.ExitSail();

            _handler.RequestEnterSail -= TryEnter;
            _handler.RequestExitSail -= TryExit;

            _handler = null;
            _inputs = null;
            _inUse = false;
        }
    }

    private void TryEnter(PlayerInputHandler p)
    {
        if (p != _handler) return;
        if (_inUse) return;
        if (_inputs == null || !_inputs.isInteracting) return;

        _inUse = true;
        _inputs.isInteracting = false;
        p.EnterSail(sail);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (p != _handler) return;
        if (!_inUse) return;

        _inUse = false;
        p.ExitSail();
    }
}