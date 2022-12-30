using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgentController))]
public class Agent : MonoBehaviour
{
    public static List<Agent> Collection { get; private set; }

    Transform _tranform;

    public Vector3 Position { get => _tranform.position; }

    public Vector3 Forward { get => _tranform.forward; }

    public AgentController Controller { get; private set; }

    void Awake() 
    {
        if (Collection == null)
            Collection = new List<Agent>();
        
        Collection.Add(this);
        Controller = GetComponent<AgentController>();
    }

    void OnDestroy() => Collection.Remove(this);
}
