using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSensor : MonoBehaviour
{
    public float collisionCheckDistance;
    public bool aboutToCollide;
    public float distanceToCollision;
    public Rigidbody rb;

    // Doesnt work
    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Working!");
    }

    // Works only once
    // void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Enter trigger: " + other.name);
    // }

    void Update()
    {
        RaycastHit hit;
        if (rb.SweepTest(transform.forward, out hit, collisionCheckDistance))
        {
            aboutToCollide = true;
            distanceToCollision = hit.distance;
        }

        if (Physics.CheckSphere(transform.position, collisionCheckDistance))
        {
            Debug.Log("Working!");
        }
    }
}
