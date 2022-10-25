using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePosition - transform.position);
    }
}
