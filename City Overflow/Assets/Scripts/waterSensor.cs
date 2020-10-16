using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSensor : MonoBehaviour
{
    public bool overflowSensor;
    public WaterGateController controlledGateOnTrigger;

    void OnTriggerEnter(Collider collider)
    {
        
        if (collider.tag == "WaterSensor")
        {
            if (overflowSensor)
            {
                Debug.Log("overflow");
                if (controlledGateOnTrigger != null)
                {
                    // Close controlled gate
                    controlledGateOnTrigger.looping = false;
                    controlledGateOnTrigger.CloseGate();
                }
            } else
            {
                Debug.Log("Level reached");
            }
        }
    }
}
