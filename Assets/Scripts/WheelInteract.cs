using UnityEngine;
using StarterAssets;

public class WheelInteract : MonoBehaviour
{
    public WheelController wheel;
    [SerializeField] private InteractableHighlight interactableHighlight;

    private PlayerInputHandler _handler;
    private StarterAssetsInputs _inputs;

    private bool _subscribed;
    private bool _inUse;
    private bool _playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var h = other.GetComponent<PlayerInputHandler>();
        if (h == null) return;

        _playerInside = true;
        if (_subscribed) return;

        _handler = h;
        _inputs = other.GetComponent<StarterAssetsInputs>();

        _handler.RequestEnterWheel += TryEnter;
        _handler.RequestExitWheel += TryExit;

        _subscribed = true;
        interactableHighlight?.SetInteractable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_handler == null) return;

        var h = other.GetComponent<PlayerInputHandler>();
        if (h != _handler) return;

        _playerInside = false;
        if (_inUse) return;

        Unsubscribe();
        interactableHighlight?.SetDefault();
    }

    private void TryEnter(PlayerInputHandler p)
    {
        if (!_inputs.isInteracting) return;
        if (p != _handler) return;
        if (_inUse) return;

        _inUse = true;
        interactableHighlight?.SetInUse(true);
        p.EnterWheel(wheel);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (!_inUse) return;
        if (p != _handler) return;

        _inUse = false;
        interactableHighlight?.SetInUse(false);
        p.ExitWheel();

        if (_playerInside)
        {
            interactableHighlight?.SetInteractable(true);
        }
        else
        {
            Unsubscribe();
            interactableHighlight?.SetDefault();
        }
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
        _playerInside = false;
    }
}
