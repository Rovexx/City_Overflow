using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform pipeInput;
    public List<SnapPoint> pipeOutputs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideSnapPoints()
    {
        foreach (SnapPoint snapPoint in pipeOutputs)
        {
            if (snapPoint.transform.gameObject.activeSelf)
            {
                snapPoint.transform.gameObject.SetActive(false);
            }
        }
    }

    public void ShowSnapPoints()
    {
        foreach (SnapPoint snapPoint in pipeOutputs)
        {
            if (!snapPoint.transform.gameObject.activeSelf && !snapPoint.taken)
            {
                snapPoint.transform.gameObject.SetActive(true);
            }
        }
    }

    public void SetSnapPointAsTaken(Transform snapPoint)
    {
       SnapPoint point = pipeOutputs.FirstOrDefault(output => output.transform.position == snapPoint.position);
       if (point != null)
       {
           point.taken = true;
       }
    }
}
