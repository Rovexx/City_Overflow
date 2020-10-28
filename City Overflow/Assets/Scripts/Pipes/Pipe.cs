using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public List<SnapPoint> pipeSnapPoints;
    public bool standingUp = false;
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
        foreach (SnapPoint snapPoint in pipeSnapPoints)
        {
            if (snapPoint.transform.gameObject.activeSelf)
            {
                snapPoint.transform.gameObject.SetActive(false);
            }
        }
    }

    public void ShowSnapPoints()
    {
        foreach (SnapPoint snapPoint in pipeSnapPoints)
        {
            if (!snapPoint.transform.gameObject.activeSelf && !snapPoint.taken)
            {
                snapPoint.transform.gameObject.SetActive(true);
            }
        }
    }

    public void SetSnapPointAsTaken(Transform snapPoint, Transform pipe)
    {
       SnapPoint point = pipeSnapPoints.FirstOrDefault(output => output.transform.position == snapPoint.position);
       if (point != null)
       {
           point.taken = true;
           point.linkedPipe = pipe;
       }
    }
}
