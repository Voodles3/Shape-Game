using UnityEngine;

[CreateAssetMenu(fileName = "ShapeSettings", menuName = "Scriptable Objects/ShapeSettings")]
public class ShapeSettings : ScriptableObject
{
    public string shapeName;
    public float defaultHealth = 100f;
}
