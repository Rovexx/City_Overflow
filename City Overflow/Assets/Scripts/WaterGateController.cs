using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGateController : MonoBehaviour
{
    public bool looping = true;
    public bool openState = false;
    public float switchTime = 5f;
    public float moveDistance = 2f;
    public bool opensX;
    public bool opensY;
    public bool opensZ;

    private bool movingGate = false;
    private Vector3 _closedPosition;
    private Vector3 _openedPosition;
    private Vector3 _targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        _closedPosition = transform.position;
        _openedPosition = transform.position;
        // Set correct open direction
        if (opensX)
        {
            _openedPosition.x = _openedPosition.x + moveDistance;
        } else if (opensY)
        {
            _openedPosition.y = _openedPosition.y + moveDistance;
        } else if (opensZ)
        {
            _openedPosition.z = _openedPosition.z + moveDistance;
        }
        
        StartCoroutine(SwitchGate(0f, openState));
    }

    private IEnumerator SwitchGate(float time, bool state)
    {
        yield return new WaitForSeconds(time);
        if (openState)
        {
            CloseGate();
        } else
        {
            OpenGate();
        }
        if (looping)
        {
            StartCoroutine(SwitchGate(switchTime, openState));
        }
    }

    public void OpenGate()
    {
        openState = true;
        movingGate = true;
        // Open the gate
        _targetPosition = _openedPosition;
    }

    public void CloseGate()
    {
        openState = false;
        movingGate = true;
        // Close the gate
        _targetPosition = _closedPosition;
    }

    private void MoveGate(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        // Did the gate reach the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Target position reached
            movingGate = false;
        }
    }

    private void FixedUpdate()
    {
        if (movingGate)
        {
            MoveGate(_targetPosition, 2f);
        }
    }
}
