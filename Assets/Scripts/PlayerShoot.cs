using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);

        Hit hit;
        if (Ray.Cast(transform.position, mousePosition, out hit))
            Debug.Log(hit.go.name);
    }
}
