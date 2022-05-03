using System.Collections.Generic;
using DG.Tweening;
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
    private bool dragging;

    public void ColorChanged(Color32 color, HatMaker.SprayChangeEffect effect)
    {
        var transform1 = transform;

        if (effect == HatMaker.SprayChangeEffect.Bring)
        {
            transform1.DORewind();
            transform1.DOMoveZ(-20f, 0.2f).OnComplete(() =>
            {
                SetColor(color);
                transform1.DOMoveZ(-5.74f, 0.2f);
            });
        } else if (effect == HatMaker.SprayChangeEffect.Shake)
        {
            SetColor(color);
            transform1.DORewind();
            transform1.DOShakeRotation(0.2f, 7f, 4);
        }
    }

    private void SetColor(Color32 color)
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

    private static bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void Update()
    {
        if (!dragging && IsPointerOverUIObject()) return;

        var fingerPos = CameraController.Instance.inputCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            DraggingStarted();
            startPos = transform.localPosition;
            clickPos = fingerPos;

            sprayParticle.Play();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DraggingFinished();
            sprayParticle.Stop();
        }

        if (!Input.GetMouseButton(0)) return;

        var diff = (clickPos - fingerPos) / 0.5f;

        var newPos = startPos;
        newPos.x -= diff.x;
        newPos.y -= diff.y;
        newPos.z = transform.position.z;

        newPos.x = Mathf.Clamp(newPos.x, -4f, 4f);
        newPos.y = Mathf.Clamp(newPos.y, -1f, 10f);

        transform.localPosition = newPos;
    }

    private void DraggingStarted()
    {
        dragging = true;
        BendCan(true);
    }

    private void DraggingFinished()
    {
        dragging = false;
        BendCan(false);
    }

    private void BendCan(bool started)
    {
        var rot = transform.eulerAngles;
        rot.x = started ? 18f : 0f;
        transform.DORotateQuaternion(Quaternion.Euler(rot), 0.2f);
    }
}