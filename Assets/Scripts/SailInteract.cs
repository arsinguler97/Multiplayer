using UnityEngine;
using StarterAssets;

public class SailInteract : MonoBehaviour
{
    public SailController sail;
    [SerializeField] private InteractableHighlight interactableHighlight;

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

        interactableHighlight?.SetInteractable(true);
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
            interactableHighlight?.SetDefault();
        }
    }

    private void TryEnter(PlayerInputHandler p)
    {
        if (p != _handler) return;
        if (_inUse) return;
        if (_inputs == null || !_inputs.isInteracting) return;

        _inUse = true;
        interactableHighlight?.SetInUse(true);
        _inputs.isInteracting = false;
        p.EnterSail(sail);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (p != _handler) return;
        if (!_inUse) return;

        _inUse = false;
        interactableHighlight?.SetInUse(false);
        interactableHighlight?.SetInteractable(true);
        p.ExitSail();
    }
}
