using UnityEngine;
using StarterAssets;

public class SailInteract : MonoBehaviour
{
    public SailController sail;
    [SerializeField] private InteractableHighlight interactableHighlight;
    [SerializeField] private Transform outlineRoot;
    [SerializeField] private Outline[] outlines;
    [SerializeField] private bool autoCollectOutlines = true;

    private PlayerInputHandler _handler;
    private StarterAssetsInputs _inputs;

    private bool _subscribed;
    private bool _inUse;
    private bool _playerInside;

    private void Awake()
    {
        RefreshOutlineCache();
        SetOutlineActive(false);
    }

    private void OnValidate()
    {
        RefreshOutlineCache();
    }

    private void OnDisable()
    {
        SetOutlineActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var h = other.GetComponent<PlayerInputHandler>();
        if (h == null) return;

        if (_handler != null && h != _handler && _inUse) return;

        _playerInside = true;

        if (_subscribed) return;

        _handler = h;
        _inputs = other.GetComponent<StarterAssetsInputs>();

        _handler.RequestEnterSail += TryEnter;
        _handler.RequestExitSail += TryExit;

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
        if (_inputs == null || !_inputs.isInteracting) return;
        if (p != _handler) return;
        if (_inUse) return;

        _inUse = true;
        _inputs.isInteracting = false;

        interactableHighlight?.SetInUse(true);
        SetOutlineActive(true);
        p.EnterSail(sail);
    }

    private void TryExit(PlayerInputHandler p)
    {
        if (!_inUse) return;
        if (p != _handler) return;

        _inUse = false;

        interactableHighlight?.SetInUse(false);
        SetOutlineActive(false);

        p.ExitSail();

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

        _handler.RequestEnterSail -= TryEnter;
        _handler.RequestExitSail -= TryExit;

        _handler = null;
        _inputs = null;

        _inUse = false;
        _subscribed = false;
        _playerInside = false;

        SetOutlineActive(false);
    }

    private void RefreshOutlineCache()
    {
        if (!autoCollectOutlines) return;

        var root = outlineRoot;
        if (root == null && sail != null && sail.sailMesh != null)
        {
            root = sail.sailMesh;
        }
        if (root == null)
        {
            root = transform;
        }

        outlines = root.GetComponentsInChildren<Outline>(true);
    }

    private void SetOutlineActive(bool active)
    {
        if (outlines == null || outlines.Length == 0) return;

        for (int i = 0; i < outlines.Length; i++)
        {
            var outline = outlines[i];
            if (outline == null) continue;
            if (outline.enabled != active) outline.enabled = active;
        }
    }
}
