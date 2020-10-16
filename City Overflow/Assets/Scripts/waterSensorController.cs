using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSensorController : MonoBehaviour
{
    public GameObject waterSensor;

    void Start()
    {
        StartCoroutine(ActivateSensor(5f));
    }

    private IEnumerator ActivateSensor(float time)
    {
        yield return new WaitForSeconds(time);
        waterSensor.SetActive(true);
    }
}
