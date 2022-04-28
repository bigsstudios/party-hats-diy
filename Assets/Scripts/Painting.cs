using CW.Common;
using UnityEngine;
using UnityEngine.EventSystems;
public class Painting : MonoBehaviour
{
    public static Painting Instance { get; private set; }
    
    private Vector3 clickPos;
    private Vector3 startPos;
    public ParticleSystem sprayParticle;

    
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
    
    private void Update()
    { 

        var fingerPos = CameraController.Instance.inputCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            startPos = transform.localPosition;
            clickPos = fingerPos;

            sprayParticle.Play();
           
        }

        if (Input.GetMouseButtonUp(0))
        {
            sprayParticle.Stop();
            
        }

        if (Input.GetMouseButton(0))
        {
            var diff = (clickPos - fingerPos) / 0.5f;

            var newPos = startPos;
            newPos.x -= diff.x;
            newPos.y -= diff.y;

            //newPos.x = Mathf.Clamp(newPos.x, -0.4f, 1.4f);
            //newPos.z = Mathf.Clamp(newPos.z, -0.85f, 1f);

            transform.localPosition = newPos;
        }
        
        
        // Color change menu
        
        var group = GetComponent<CanvasGroup>();
        
        if (group != null)
        {
            var alpha = 1.0f;

            /*if (isolateTarget != null)
            {
                alpha = isolateTarget.gameObject.activeInHierarchy == true ? 1.0f : 0.5f;
            }*/

            group.alpha          = alpha;
            group.blocksRaycasts = alpha > 0.0f;
            group.interactable   = alpha > 0.0f;
        }
    }
    
}
