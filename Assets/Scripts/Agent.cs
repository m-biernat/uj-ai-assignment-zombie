using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgentController))]
public class Agent : MonoBehaviour
{
    public static List<Agent> Agents { get; private set; }

    Transform _tranform;

    public Vector3 Position { get => _tranform.position; }

    public Vector3 Forward { get => _tranform.forward; }

    public AgentController Controller { get; private set; }

    void Awake() 
    {
        if (Agents == null)
            Agents = new List<Agent>();
        
        Agents.Add(this);
        Controller = GetComponent<AgentController>();
    }
}
