using UnityEngine;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    public static List<Vector3> Positions { get; private set; }

    void Awake() 
    {
        if (Positions == null)
            Positions = new List<Vector3>();
        
        Positions.Add(transform.position);
    }
}
