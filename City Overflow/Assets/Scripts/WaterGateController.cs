using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGateController : MonoBehaviour
{
    private Vector3 _closedPosition;
    private Vector3 _openedPosition;
    public bool automatic = true;
    public float switchTime = 5f;
    public float moveDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _closedPosition = transform.position;
        _openedPosition = transform.position;
        _openedPosition.x = _openedPosition.x + moveDistance;
        StartCoroutine(CloseGate(0f));
    }

    private IEnumerator CloseGate(float time)
    {
        yield return new WaitForSeconds(time);
        transform.position = _closedPosition;
        if (automatic)
        {
            StartCoroutine(OpenGate(switchTime));
        }
    }

    private IEnumerator OpenGate(float time)
    {
        yield return new WaitForSeconds(time);
        transform.position = _openedPosition;
        if (automatic)
        {
            StartCoroutine(CloseGate(switchTime));
        }
    }
}
