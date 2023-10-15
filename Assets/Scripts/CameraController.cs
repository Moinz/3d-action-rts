using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _deadZone = 0.25f;

    [SerializeField]
    private float _speed = 5f;

    private Camera camera;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            return;
        
        var forward = Vector3.forward;
        var right = Vector3.right;

        if (!camera)
            camera = Camera.main;
        
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
    }
    
    
// c#
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
