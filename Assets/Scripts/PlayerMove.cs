using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed;

    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        var dir = new Vector3(h, v, 0);

        if (dir.magnitude == 0)
            return;
        
        var nextPosition = transform.position + dir.normalized * _speed * Time.deltaTime;

        if (!CircleCollider.IsColliding(gameObject, nextPosition))
            transform.position = nextPosition;
    }
}
