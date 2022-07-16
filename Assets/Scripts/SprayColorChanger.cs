using System.Collections.Generic;
using UnityEngine;

public class SprayColorChanger : MonoBehaviour
{
    public HatMaker.SprayChangeEffect sprayChangeEffect = HatMaker.SprayChangeEffect.Bring;
    public Painter spray;
    
    private readonly Dictionary<string, Color32> _colorMap = new Dictionary<string, Color32>
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
    
    private string _currentColor = "Red";
    
    public void SprayCanSelected(string colorStr)
    {
        if (colorStr == _currentColor) return;
        if (!_colorMap.TryGetValue(colorStr, out var color)) return;
        spray.ColorChanged(color, sprayChangeEffect);
        _currentColor = colorStr;
    }
}