using System.Collections;
using UnityEngine;

public class HatMaker : MonoBehaviour
{
    private GameState currentState = GameState.JustStarted;
    private bool stateChanging;

    public Hat hat;
    
    [Header("Bending")]
    [Range(1f, 50f)]
    public float bendingSpeed = 25f;
    
    private float bendAmount;
    private Vector3 clickPos;
    
    private void Start()
    {
        StartCoroutine(SetState(GameState.Bending));
    }

    private void Update()
    {
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
                    var offset = currentPos.x - clickPos.x;
                    if (offset < 0) break;
                    
                    bendAmount += offset / (60f - bendingSpeed);

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
            yield return StartCoroutine(hat.PrepareForPainting());
        }
    }

    private void StateFinished(GameState state)
    {
        print(state + " finished");
    }
    
    private void StateStarted(GameState state)
    {
        print(state + " started");
    }

    private Vector3 GetMousePosition()
    {
        return CameraController.Instance.inputCam.ScreenToWorldPoint(Input.mousePosition);
    }
}