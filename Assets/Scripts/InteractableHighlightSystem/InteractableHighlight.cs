using System.Collections.Generic;
using UnityEngine;

// Caches child renderers tagged with the interactable rendering layer and toggles their highlight layer masks at runtime.
public class InteractableHighlight : MonoBehaviour
{
    [Header("Rendering Layer Masks")]
    [SerializeField] private uint interactableMask = 1u;
    [SerializeField] private uint inUseMask = 1u;
    [SerializeField] private uint defaultMask = 1u;
    [SerializeField] private bool includeInactive = true;
    [SerializeField] private bool setDefaultOnEnable = true;

    private readonly List<Renderer> _renderers = new List<Renderer>();
    private readonly Dictionary<Renderer, uint> _originalMasks = new Dictionary<Renderer, uint>();

    private bool _isInteractable;
    private bool _isInUse;

    private void OnEnable()
    {
        CacheRenderers();
        if (setDefaultOnEnable)
            SetDefault();
        else
            ApplyState();
    }

    private void OnDisable()
    {
        RestoreOriginalMasks();
    }

    public void SetInteractable(bool enabled)
    {
        _isInteractable = enabled;
        ApplyState();
    }

    public void SetDefault()
    {
        _isInteractable = false;
        _isInUse = false;
        ApplyState();
    }

    public void SetInUse(bool enabled)
    {
        _isInUse = enabled;
        ApplyState();
    }

    public void RefreshCache()
    {
        CacheRenderers();
        ApplyState();
    }

    private void CacheRenderers()
    {
        _renderers.Clear();
        _originalMasks.Clear();

        GetComponentsInChildren(includeInactive, _renderers);
        Renderer self = GetComponent<Renderer>();
        if (self != null && !_renderers.Contains(self))
            _renderers.Add(self);
        for (int i = _renderers.Count - 1; i >= 0; i--)
        {
            Renderer r = _renderers[i];
            if (r == null)
            {
                _renderers.RemoveAt(i);
                continue;
            }

            if ((r.renderingLayerMask & interactableMask) == 0u)
            {
                _renderers.RemoveAt(i);
                continue;
            }

            if (setDefaultOnEnable)
            {
                _originalMasks[r] = defaultMask;
                r.renderingLayerMask = defaultMask;
            }
            else
            {
                _originalMasks[r] = r.renderingLayerMask;
            }
        }
    }

    private void ApplyState()
    {
        if (_renderers.Count == 0)
            return;

        uint targetMask = 0u;
        bool useHighlight = false;

        if (_isInUse)
        {
            targetMask = inUseMask;
            useHighlight = true;
        }
        else if (_isInteractable)
        {
            targetMask = interactableMask;
            useHighlight = true;
        }

        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer r = _renderers[i];
            if (r == null) continue;

            if (useHighlight)
            {
                r.renderingLayerMask = targetMask;
            }
            else if (_originalMasks.TryGetValue(r, out uint original))
            {
                r.renderingLayerMask = original;
            }
        }
    }

    private void RestoreOriginalMasks()
    {
        foreach (var kvp in _originalMasks)
        {
            if (kvp.Key != null)
                kvp.Key.renderingLayerMask = kvp.Value;
        }
    }
}
