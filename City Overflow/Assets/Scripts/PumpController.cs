using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpController : MonoBehaviour
{
    public float pumpSpeed;
    public bool powered;
    public Transform rotor;

    void FixedUpdate()
    {
        if (powered)
        {
            rotor.Rotate (pumpSpeed, 0, 0 * Time.fixedDeltaTime);
        }
    }
}
