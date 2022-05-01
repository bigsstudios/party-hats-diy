using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
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

    private readonly Dictionary<string, Color32> colorMap = new Dictionary<string, Color32>
    {
        { "Black" ,Color.black },
        { "Blue" , new Color32(30, 144, 255, 255) },
        { "Red" ,Color.red },
        { "Yellow" ,Color.yellow },
        { "White" ,Color.white },
        { "Green" ,Color.green },
        { "Pink" , new Color32(255, 20, 147, 255) },
        { "Purple" , new Color32(148, 0, 211, 255) },
        { "Orange" , new Color32(255, 140, 0, 255)},
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

    [Header("Capping")]
    private CapHolder selectedCapHolder;

    [Header("Decorating")]
    private DecorHolder selectedDecorHolder;

    private void Start()
    {
        StartCoroutine(SetState(GameState.Bending));
        lastCraftingTableX = craftingTables.position.x;
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
        if (state == GameState.Bending)
        {
            bendingGesture.gameObject.SetActive(false);
            BringNewTable();
            yield return StartCoroutine(hat.PrepareForPainting());
        }
        else if (state == GameState.Painting)
        {
            BringNewTable();
            hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
        }
        else if (state == GameState.Capping)
        {
            if(selectedCapHolder != null)
                yield return selectedCapHolder.cap.transform.DOLocalMoveY(20f, 1f).WaitForCompletion();
            BringNewTable();
            hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
            sprayCanPanel.gameObject.SetActive(false);
            spray.gameObject.SetActive(false);
            capPanel.gameObject.SetActive(false);
        }
        else if (state == GameState.Decorating)
        {
            BringNewTable();
            hat.transform.DOJump(new Vector3(0, 0, 0), 6f, 1, .5f);
            sprayCanPanel.gameObject.SetActive(false);
            spray.gameObject.SetActive(false);
            capPanel.gameObject.SetActive(false);
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
        }
    }

    private static Vector3 GetMousePosition()
    {
        return CameraController.Instance.inputCam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OkayClicked()
    {
        if (currentState == GameState.Painting)
        {
            StartCoroutine(SetState(GameState.Capping));
        } else if (currentState == GameState.Capping)
        {
            StartCoroutine(SetState(GameState.Decorating));
        }
    }

    public void SprayCanSelected(string colorStr)
    {
        if (colorMap.TryGetValue(colorStr, out var color))
        {
            spray.SetColor(color);
        }
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
        decorHolder.decor.transform.SetParent(rotatingShowcase);
        decorHolder.decor.SetActive(true);
        decorHolder.decor.transform.DORewind();
        decorHolder.decor.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 1f, 2, 2f);
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
    }
}