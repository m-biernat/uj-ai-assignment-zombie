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
        
        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);

        Move(h, v);
        Rotate(mousePosition);
        Shoot(mousePosition);
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

    void Rotate(Vector3 mousePosition) 
        => transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePosition - transform.position);

    void Shoot(Vector3 mousePosition)
    {
        Hit hit;
        if (Ray.Cast(transform.position, mousePosition, out hit))
            Debug.Log(hit.go.name);
    }
}
