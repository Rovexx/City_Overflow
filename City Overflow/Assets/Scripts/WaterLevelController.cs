using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelController : MonoBehaviour
{
    public waterSensor[] trackedSensors;
    public float waterLevelPercentage = 0f;
    public bool overflowing = false;

    public WaterGateController controlledGate;

    private float triggeredSensors = 0;
    private float sensorAmount;

    void Start()
    {
        sensorAmount = trackedSensors.Length;
    }

    public void UpdateWaterSensorData()
    {
        triggeredSensors = 0;
        foreach (waterSensor waterSensor in trackedSensors)
        {
            if (waterSensor.triggered)
            {
                triggeredSensors ++;
                if (waterSensor.overflowSensor)
                {
                    overflowing = true;
                }
            }
        }
        calculateWaterLevelPercentage(triggeredSensors, sensorAmount);

        // Check if reservoir is full or overflowing
        if (triggeredSensors == (sensorAmount - 1))
        {
            // All sensors where triggered except for the overflow sensor
            OnFull();
        } else if (triggeredSensors == sensorAmount && overflowing)
        {
            // This reservoir is overflowing
            OnOverflow();
        }
    }

    private void calculateWaterLevelPercentage(float triggeredSensors, float sensorAmount)
    {
        waterLevelPercentage = (triggeredSensors / (sensorAmount - 1));
    }

    private void OnFull()
    {
        Debug.Log("Reservoir full!");
        StartCoroutine(GameManager.instance.UpdateWaterLevel(0f));
    }
    
    private void OnOverflow()
    {
        Debug.Log("Reservoir overflowing!");
        StartCoroutine(GameManager.instance.UpdateWaterLevel(0f));
        // Maybe close gate if we control one?
        // if (controlledGate != null)
        // {
        //     CloseControlledGate();
        // }
    }

    private void CloseControlledGate()
    {
        controlledGate.looping = false;
        controlledGate.CloseGate();
    }
}
