using System.Collections;
using DG.Tweening;
using PaintIn3D;
using UnityEngine;

public class Hat : MonoBehaviour
{
    [SerializeField] private MegaBend bender;
    [SerializeField] private P3dPaintableTexture texture;

    public void SetBendAmount(float bendAmount)
    {
        bender.angle = Mathf.Lerp(0f, 360f, bendAmount);
        bender.dir = Mathf.Lerp(0f, 18f, bendAmount);
    }

    public IEnumerator PrepareForPainting()
    {
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