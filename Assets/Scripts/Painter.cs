using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;

public class Painter : MonoBehaviour
{
    private Vector3 clickPos;
    private Vector3 startPos;
    public ParticleSystem sprayParticle;
    public P3dPaintSphere paintSphere;
    public MeshRenderer canRenderer;

    public void SetColor(Color32 color)
    {
        var sprayParticleMain = sprayParticle.main;
        var gradient = new ParticleSystem.MinMaxGradient
        {
            color = color
        };
        sprayParticleMain.startColor = gradient;
        paintSphere.Color = color;
        canRenderer.materials[1].color = color;
    }
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return;
        
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
