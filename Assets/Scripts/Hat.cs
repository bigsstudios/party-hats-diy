using System.Collections;
using DG.Tweening;
using PaintIn3D;
using UnityEngine;

public class Hat : MonoBehaviour
{
    public static Hat Instance { get; private set; }
    
    [SerializeField]
    private MegaBend bender;

    [SerializeField]
    private MegaModifyObject modifyObject;

    [SerializeField]
    private P3dPaintableTexture texture;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void MakeDontDestroyOnLoad()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetBendAmount(float bendAmount)
    {
        bender.angle = Mathf.Lerp(0f, 360f, bendAmount);
        bender.dir = Mathf.Lerp(0f, 18f, bendAmount);
    }

    public IEnumerator PrepareForPainting()
    {
        modifyObject.recalcCollider = false;
        modifyObject.recalcnorms = false;

        var sequence = DOTween.Sequence();
        sequence.Insert(0f, transform.DORotateQuaternion(Quaternion.Euler(Vector3.zero), .5f));
        sequence.Insert(0f, transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f));

        yield return sequence.WaitForCompletion();
    }

    public void SetPaintingMask(Texture maskTexture)
    {
        texture.LocalMaskTexture = maskTexture;
    }
}