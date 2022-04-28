using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Hat : MonoBehaviour
{
    [SerializeField] private MegaBend bender;

    public void SetBendAmount(float bendAmount)
    {
        bender.angle = Mathf.Lerp(0f, 360f, bendAmount);
        bender.dir = Mathf.Lerp(0f, 18f, bendAmount);
    }

    public IEnumerator PrepareForPainting()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0f, transform.DORotate(new Vector3(-108f, 0, 0), .5f));
        sequence.Insert(0f, transform.DOJump(new Vector3(0f, 4.72f, 1.65f), 6f, 1, .5f));
        // sequence.Insert(0f, transform.DOLocalMoveY(4.72f, 1f));
        // sequence.Insert(0f, transform.DOLocalMoveZ(1.65f, 1f));
        
        yield return sequence.WaitForCompletion();
    }
}