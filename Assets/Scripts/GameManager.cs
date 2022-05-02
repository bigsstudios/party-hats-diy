using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text startText;
    public CanvasGroup loadingCanvasGroup;
    public Transform hatParent;
    public Animator customerAnimator;
    private static readonly int Dancing = Animator.StringToHash("Dancing");

    private void Start()
    {
        if (Hat.Instance == null)
        {
            startText.gameObject.SetActive(true);
            startText.transform.DOScale(1.3f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            PlaceHatAndDance();
        }

        loadingCanvasGroup.DOFade(0f, 0.5f);
    }

    private void PlaceHatAndDance()
    {
        var hatTransform = Hat.Instance.transform;
        hatTransform.SetParent(hatParent);
        hatTransform.localScale = Vector3.one;
        hatTransform.localEulerAngles = Vector3.zero;
        hatTransform.localPosition = Vector3.zero;
        hatTransform.gameObject.SetActive(transform);
        customerAnimator.SetBool(Dancing, true);
    }

    public void LoadCreativeScene()
    {
        loadingCanvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            if (Hat.Instance != null)
            {
                Destroy(Hat.Instance.gameObject);
            }

            SceneManager.LoadScene("BendScene");
        });
    }
}