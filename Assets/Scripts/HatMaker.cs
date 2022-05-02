using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HatMaker : MonoBehaviour
{
    private GameState currentState = GameState.JustStarted;
    private bool stateChanging;
    private float lastCraftingTableX;

    public Hat hat;
    public Transform rotatingShowcase;
    public Painter spray;
    public Button buttonOk;
    public Transform sprayCanPanel;
    public Transform capPanel;
    public Transform decorationPanel;
    public Transform craftingTables;
    public CanvasGroup loadingCanvasGroup;

    private readonly Dictionary<string, Color32> colorMap = new Dictionary<string, Color32>
    {
        { "Black", Color.black },
        { "Blue", new Color32(30, 144, 255, 255) },
        { "Red", Color.red },
        { "Yellow", Color.yellow },
        { "White", Color.white },
        { "Green", Color.green },
        { "Pink", new Color32(255, 20, 147, 255) },
        { "Purple", new Color32(148, 0, 211, 255) },
        { "Orange", new Color32(255, 140, 0, 255) },
    };

    public CapHolder[] caps;
    public DecorHolder[] decors;

    [Header("Bending")]
    [Range(1f, 50f)]
    public float bendingSpeed = 25f;

    public Transform bendingGesture;

    private float bendAmount;
    private Vector3 clickPos;

    [Header("Painting")]
    private float showcaseRotY;

    private string currentColor = "Red";

    [Header("Capping")]
    private CapHolder selectedCapHolder;

    [Header("Decorating")]
    private DecorHolder selectedDecorHolder;

    [Header("Showcasing")]
    public ParticleSystem confetti1;
    public ParticleSystem confetti2;

    private void Start()
    {
        lastCraftingTableX = craftingTables.position.x;
        
        loadingCanvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            loadingCanvasGroup.blocksRaycasts = false;
            StartCoroutine(SetState(GameState.Bending));
        });
    }

    private void Update()
    {
        RotateShowcase();
        if (stateChanging) return;

        switch (currentState)
        {
            case GameState.Bending:
                if (Input.GetMouseButtonDown(0))
                {
                    clickPos = GetMousePosition();
                }

                if (Input.GetMouseButton(0))
                {
                    var currentPos = GetMousePosition();
                    // var offset = currentPos.x - clickPos.x;
                    var offset = (currentPos - clickPos).magnitude;

                    bendAmount += Mathf.Abs(offset) / (60f - bendingSpeed);

                    hat.SetBendAmount(bendAmount);

                    if (bendAmount >= 1f)
                    {
                        bendAmount = 1f;
                        StartCoroutine(SetState(GameState.Painting));
                    }

                    clickPos = currentPos;
                }

                break;
            case GameState.Painting:

                break;
        }
    }

    private void RotateShowcase()
    {
        showcaseRotY += 50 * Time.deltaTime;
        var oldRot = rotatingShowcase.eulerAngles;
        oldRot.y = showcaseRotY;
        rotatingShowcase.rotation = Quaternion.Euler(oldRot);
    }

    private IEnumerator SetState(GameState state)
    {
        stateChanging = true;

        if (currentState != GameState.JustStarted)
        {
            yield return StateFinishing(currentState);
            StateFinished(currentState);
        }

        currentState = state;
        stateChanging = false;
        StateStarted(currentState);
    }

    private IEnumerator StateFinishing(GameState state)
    {
        switch (state)
        {
            case GameState.Bending:
                bendingGesture.gameObject.SetActive(false);
                BringNewTable();
                yield return StartCoroutine(hat.PrepareForPainting());
                break;
            case GameState.Painting:
                BringNewTable();
                hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
                break;
            case GameState.Capping:
            {
                if (selectedCapHolder != null)
                    yield return selectedCapHolder.cap.transform.DOLocalMoveY(20f, 1f).WaitForCompletion();
                UnparentAllCaps();
                BringNewTable();
                hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
                sprayCanPanel.gameObject.SetActive(false);
                spray.gameObject.SetActive(false);
                capPanel.gameObject.SetActive(false);
                break;
            }
            case GameState.Decorating:
                BringNewTable();
                hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
                decorationPanel.gameObject.SetActive(false);
                break;
        }
    }

    private void UnparentAllCaps()
    {
        foreach (var capHolder in caps)
        {
            capHolder.cap.transform.SetParent(transform.parent);
        }
    }

    private void StateFinished(GameState state)
    {
        print(state + " finished");
    }

    private void BringNewTable()
    {
        lastCraftingTableX -= 15f;
        craftingTables.DORewind();
        craftingTables.DOMoveX(lastCraftingTableX, .5f);
    }

    private void ShowcaseAllCaps()
    {
        foreach (var capHolder in caps)
        {
            capHolder.cap.transform.SetParent(rotatingShowcase);
        }
    }

    private void StateStarted(GameState state)
    {
        print(state + " started");
        switch (state)
        {
            case GameState.Bending:
                bendingGesture.gameObject.SetActive(true);
                break;
            case GameState.Painting:
                buttonOk.gameObject.SetActive(true);
                ShowcaseAllCaps();
                hat.transform.SetParent(rotatingShowcase);
                spray.gameObject.SetActive(true);
                sprayCanPanel.gameObject.SetActive(true);
                break;
            case GameState.Capping:
                spray.gameObject.SetActive(false);
                sprayCanPanel.gameObject.SetActive(false);
                capPanel.gameObject.SetActive(true);
                break;
            case GameState.Decorating:
                capPanel.gameObject.SetActive(false);
                decorationPanel.gameObject.SetActive(true);
                break;
            case GameState.Showcasing:
                StartCoroutine(Confetti());
                break;
        }
    }

    private IEnumerator Confetti()
    {
        yield return new WaitForSeconds(0.5f);
        confetti1.Play();
        confetti2.Play();
        yield return new WaitForSeconds(3f);
        yield return loadingCanvasGroup.DOFade(1f, 0.5f).WaitForCompletion();
        hat.gameObject.SetActive(false);
        hat.transform.SetParent(transform.parent);
        hat.MakeDontDestroyOnLoad();
        SceneManager.LoadScene("MainScene");
    }

    private static Vector3 GetMousePosition()
    {
        return CameraController.Instance.inputCam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OkayClicked()
    {
        switch (currentState)
        {
            case GameState.Painting:
                StartCoroutine(SetState(GameState.Capping));
                break;
            case GameState.Capping:
                StartCoroutine(SetState(GameState.Decorating));
                break;
            case GameState.Decorating:
                StartCoroutine(SetState(GameState.Showcasing));
                break;
        }
    }

    public void SprayCanSelected(string colorStr)
    {
        if (colorStr == currentColor) return;
        if (!colorMap.TryGetValue(colorStr, out var color)) return;
        spray.ColorChanged(color);
        currentColor = colorStr;
    }

    public void CapSelected(string capStr)
    {
        var capHolder = caps.FirstOrDefault(c => c.name == capStr);
        if (capHolder == null) return;
        selectedCapHolder = capHolder;
        capHolder.cap.gameObject.SetActive(true);
        hat.SetPaintingMask(capHolder.cap.maskTexture);
        capHolder.cap.transform.DOLocalMoveY(0f, 1f);
        capPanel.gameObject.SetActive(false);
        sprayCanPanel.gameObject.SetActive(true);
        spray.gameObject.SetActive(true);
    }

    public void DecorationSelected(string decorStr)
    {
        var decorHolder = decors.FirstOrDefault(d => d.name == decorStr);
        if (decorHolder == null) return;
        selectedDecorHolder = decorHolder;
        decorHolder.decor.transform.eulerAngles = Vector3.zero;
        DeactivateDecorsBut(decorStr);
        decorHolder.decor.transform.DORewind();
        decorHolder.decor.transform.DOPunchScale(Vector3.one * decorHolder.scalePower, .2f, 2, 2f);
    }

    private void DeactivateDecorsBut(string but)
    {
        foreach (var decorHolder in decors)
        {
            decorHolder.decor.transform.SetParent(decorHolder.name == but ? hat.transform : transform.parent);
            decorHolder.decor.gameObject.SetActive(decorHolder.name == but);
        }
    }

    [Serializable]
    public class CapHolder
    {
        public string name;
        public Cap cap;
    }

    [Serializable]
    public class DecorHolder
    {
        public string name;
        public GameObject decor;
        public float scalePower = 1f;
    }
}