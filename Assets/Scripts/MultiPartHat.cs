using System.Collections;
using System.Linq;
using UnityEngine;

public class MultiPartHat : MonoBehaviour
{
    public HatPart[] parts;

    private int _selectedPartIndex = -1;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => parts.All(p => p.IsMaterialReady()));
        SelectNextPart();
    }

    private void SelectNextPart()
    {
        _selectedPartIndex++;

        for (var i = 0; i < parts.Length; i++)
        {
            if (i == _selectedPartIndex)
            {
                parts[i].Select();
            }
            else
            {
                parts[i].Deselect();
            }
        }
    }

    private void MakeAllOpaque()
    {
        foreach (var hatPart in parts)
        {
            hatPart.SetReady();
        }
    }

    private void Update()
    {
        var oldRot = transform.eulerAngles;
        oldRot.y += 50 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(oldRot);
    }

    public void OkClicked()
    {
        if (_selectedPartIndex == parts.Length - 1)
        {
            MakeAllOpaque();
        }
        else if (_selectedPartIndex < parts.Length - 1)
        {
            SelectNextPart();
        }
    }
}