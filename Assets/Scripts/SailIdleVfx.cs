using System.Collections.Generic;
using UnityEngine;

public class SailIdleVfx : MonoBehaviour
{
    [SerializeField] private GameObject idleVfxRoot;
    [SerializeField] private Renderer[] idleRenderers;
    [SerializeField] private bool autoCollectRenderersFromRoot = true;

    private readonly HashSet<PlayerInputHandler> _playersInside = new HashSet<PlayerInputHandler>();
    private bool _canToggleRootObject;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    private void Awake()
    {
        CacheRootToggleSafety();
        RefreshRendererCache();
    }

    private void OnValidate()
    {
        CacheRootToggleSafety();
        RefreshRendererCache();
    }

    private void OnEnable()
    {
        SetVfxActive(true);
    }

    private void OnDisable()
    {
        SetVfxActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerInputHandler>();
        if (player == null) return;

        _playersInside.Add(player);
        SetVfxActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponentInParent<PlayerInputHandler>();
        if (player == null) return;

        _playersInside.Remove(player);
        CleanupInsidePlayers();

        if (_playersInside.Count == 0)
        {
            SetVfxActive(true);
        }
    }

    private void SetVfxActive(bool active)
    {
        if (_canToggleRootObject && idleVfxRoot != null && idleVfxRoot.activeSelf != active)
        {
            idleVfxRoot.SetActive(active);
        }

        if (idleRenderers == null || idleRenderers.Length == 0) return;

        for (int i = 0; i < idleRenderers.Length; i++)
        {
            var r = idleRenderers[i];
            if (r == null) continue;
            if (r.enabled != active) r.enabled = active;
        }
    }

    private void RefreshRendererCache()
    {
        if (!autoCollectRenderersFromRoot) return;

        var root = idleVfxRoot != null ? idleVfxRoot : gameObject;
        idleRenderers = root.GetComponentsInChildren<Renderer>(true);
    }

    private void CleanupInsidePlayers()
    {
        _playersInside.RemoveWhere(p => p == null || !p.gameObject.activeInHierarchy);
    }

    private void CacheRootToggleSafety()
    {
        if (idleVfxRoot == null)
        {
            _canToggleRootObject = false;
            return;
        }

        // Never disable the object that hosts this trigger script or its parent chain.
        _canToggleRootObject = !transform.IsChildOf(idleVfxRoot.transform);
    }
}
