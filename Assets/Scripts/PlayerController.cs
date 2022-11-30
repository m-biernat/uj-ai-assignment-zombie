using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed;

    Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        
        Move(h, v);

        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0.0f;
        var dir = (mousePosition - transform.position).normalized;

        Debug.DrawLine(transform.position, mousePosition);
        Debug.DrawRay(transform.position, dir, Color.red);

        Rotate(dir);
        Shoot(dir);
    }

    void Move(float h, float v)
    {
        var dir = new Vector3(h, v, 0);

        if (dir.magnitude == 0)
            return;
        
        var nextPosition = transform.position + dir.normalized * _speed * Time.deltaTime;

        if (!CircleCollider.IsColliding(gameObject, nextPosition))
            transform.position = nextPosition;
    }

    void Rotate(Vector3 dir) 
        => transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

    void Shoot(Vector3 dir)
    {
        GameObject hit;
        if (Ray.Cast(gameObject, transform.position, dir, out hit))
            Debug.Log(hit.name);
    }
}
