using UnityEngine;

// This class is inheritable by our shape-specific attack scripts, and makes them require an Attack() method
public abstract class ShapeAttack : MonoBehaviour
{
    public abstract void Attack();
}
