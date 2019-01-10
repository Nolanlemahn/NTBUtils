//Inspired by https://forum.unity.com/threads/changing-how-animation-curve-window-looks.488841/#post-3426316

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(CurveAttribute))]
public class CurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var curve = this.attribute as CurveAttribute;
        EditorGUI.CurveField(position, property, curve.Color, new Rect(
            curve.MinX, curve.MinY,
            curve.MaxX - curve.MinX, curve.MaxY - curve.MinY));
    }
}
#endif

public class CurveAttribute : PropertyAttribute
{
    public float MinX, MinY;
    public float MaxX, MaxY;
    public Color Color;

    public CurveAttribute(float minX, float maxX, float minY, float maxY,
        float r = 0f, float g = 1f, float b = 0f, float a = 1f)
    {
        this.MinX = minX;
        this.MinY = minY;
        this.MaxX = maxX;
        this.MaxY = maxY;
        this.Color = new Color(r, g, b, a);
    }
}