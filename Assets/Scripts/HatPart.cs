using PaintIn3D;
using UnityEngine;

public class HatPart : MonoBehaviour
{
    public MeshCollider meshCollider;
    public P3dPaintable paintable;
    public MeshRenderer meshRenderer;
    public P3dMaterialCloner materialCloner;

    private Material _material;
    private bool _isMaterialReady;

    private void Awake()
    {
        materialCloner.MaterialCloned += () =>
        {
            _material = meshRenderer.material;
            _isMaterialReady = true;
        };
    }

    public void SetReady()
    {
        meshCollider.enabled = false;
        paintable.enabled = false;
        MakeOpaque();
    }

    public void Select()
    {
        meshCollider.enabled = true;
        paintable.enabled = true;
        MakeOpaque();
    }

    public void Deselect()
    {
        meshCollider.enabled = false;
        paintable.enabled = false;
        MakeTransparent();
    }

    private void MakeOpaque()
    {
        var color = _material.GetColor("_Color");
        color.a = 1f;
        _material.SetColor("_Color", color);
    }

    private void MakeTransparent()
    {
        var color = _material.GetColor("_Color");
        color.a = .3f;
        _material.SetColor("_Color", color);
    }

    public bool IsMaterialReady()
    {
        return _isMaterialReady;
    }
}