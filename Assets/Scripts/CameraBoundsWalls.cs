using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBoundsWalls : MonoBehaviour
{
    public float thickness = 1f;
    public Transform leftWall, rightWall, topWall, bottomWall;

    void LateUpdate()
    {
        Camera cam = GetComponent<Camera>();

        Vector2 bottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 topRight = cam.ViewportToWorldPoint(new Vector2(1, 1));

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        leftWall.position = new Vector2(bottomLeft.x - thickness / 2f, cam.transform.position.y);
        leftWall.localScale = new Vector3(thickness, height, 1f);

        rightWall.position = new Vector2(topRight.x + thickness / 2f, cam.transform.position.y);
        rightWall.localScale = new Vector3(thickness, height, 1f);

        topWall.position = new Vector2(cam.transform.position.x, topRight.y + thickness / 2f);
        topWall.localScale = new Vector3(width, thickness, 1f);

        bottomWall.position = new Vector2(cam.transform.position.x, bottomLeft.y - thickness / 2f);
        bottomWall.localScale = new Vector3(width, thickness, 1f);
    }
}
