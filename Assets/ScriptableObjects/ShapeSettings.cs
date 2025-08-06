using UnityEngine;

[CreateAssetMenu(fileName = "ShapeSettings", menuName = "Scriptable Objects/ShapeSettings")]
public class ShapeSettings : ScriptableObject
{
    [Header("General Settings")]
    public string shapeName;

    [Header("Health Settings")]
    public float defaultMaxHealth = 100f;

}
