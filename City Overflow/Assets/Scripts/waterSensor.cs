using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSensor : MonoBehaviour
{
    public WaterLevelController myWaterLevelController;
    public bool overflowSensor = false;
    public bool triggered = false;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "WaterSensor")
        {
            triggered = true;
            myWaterLevelController.UpdateWaterSensorData();
        }
    }
}
