using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Ensure you have the NavMesh components available

public class move : MonoBehaviour
{
    [SerializeField] private Transform trnasform;

    NavMeshAgent agent; 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation to control it manually
        agent.updateUpAxis = false; // Disable automatic up-axis updates if not needed
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(trnasform.position); // Set the destination to the transform's position
    }
}
