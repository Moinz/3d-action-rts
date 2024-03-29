using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _deadZone = 0.25f;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private Vector2 _bounds;


    private Camera camera;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            return;

        if (!camera)
            camera = Camera.main;

        if (!IsWithinScreen())
            return;
        
        Move();
        Zoom();
    }

    private bool IsWithinScreen()
    {
        var mousePos = Input.mousePosition;
        var mouseWorldPos = camera.ScreenToViewportPoint(mousePos);
        
        if (mouseWorldPos.x > 1f || mouseWorldPos.x < 0f)
            return false;
        
        if (mouseWorldPos.y	> 1f || mouseWorldPos.y < 0f)
            return false;

        return true;
    }

    private void Move()
    {
        var forward = Vector3.forward;
        var right = Vector3.right;
        
        var mousePos = Input.mousePosition;
        var mouseWorldPos = camera.ScreenToViewportPoint(mousePos);
        
        var x = mouseWorldPos.x;
        var y = mouseWorldPos.y;
        
        var moveX = 0f;
        var moveY = 0f;
        
        // Map X
        if (x < _deadZone)
            moveX = map(x, 0, _deadZone, -1, 0);
        else if (x > 1 - _deadZone)
            moveX = map(x, 1 - _deadZone, 1, 0, 1);
        
        // Map Y
        if (y < _deadZone)
            moveY = map(y, 0, _deadZone, -1, 0);
        else if (y > 1 - _deadZone)
            moveY = map(y, 1 - _deadZone, 1, 0, 1);

        transform.position += (forward * moveY + right * moveX) * (_speed * Time.deltaTime);
        transform.position = Clamp(transform.position);
    }
        
    public float zoomScale = 0.01f;
    public float currentScale = 0.5f;
    public Vector2 fovClamp = new(45, 90);

    public static float CurrentScale;
    
    private void Zoom()
    {
        var scroll = Mouse.current.scroll;

        var up = scroll.up.value;
        var down = scroll.down.value;
        
        if (up > 0f)
            up = (up / 120) * zoomScale * Time.timeScale;
        
        if (down > 0f)
            down = (down / 120) * (zoomScale * Time.timeScale) * -1;
        
        currentScale += (up + down);
        currentScale = Mathf.Clamp01(currentScale);

        var fovRange = fovClamp;
        var newFov = map(currentScale, 0f, 1f, fovRange.x, fovRange.y);

        CurrentScale = currentScale;
        camera.fieldOfView = newFov;
    }

    private Vector3 Clamp(Vector3 transformPosition)
    {
        var x = Mathf.Clamp(transformPosition.x, -_bounds.x, _bounds.x);
        var z = Mathf.Clamp(transformPosition.z, -_bounds.y, _bounds.y);

        return new Vector3(x, transformPosition.y, z);
    }


    // c#
    public static float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
