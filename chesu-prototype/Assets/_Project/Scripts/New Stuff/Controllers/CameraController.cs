using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public Vector2 panOffset;
    public float scrollSpeed = 20f;
    public bool scrollInvert = true;
    public float minY;
    public float maxY;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness){
            pos.z += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness){
            pos.z -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness){
            pos.x += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness){
            pos.x -= panSpeed * Time.deltaTime;
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.x = Mathf.Clamp(pos.x, -panLimit.x + panOffset.x, panLimit.x + panOffset.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y + panOffset.y, panLimit.y + panOffset.y);
        transform.position = pos;
    }
}
