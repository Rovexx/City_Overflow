using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PipeController : MonoBehaviour
{
    public InputMaster inputActions;
    private Vector2 _mousePosition;

    public Transform pipePrefabCopy;
    public Transform pipeToAdd;
    public Transform pipeParent;
    public List<Transform> pipes;

    public Camera mainCamera;

    private bool _addingNewPipe = false;
    public float snapDistance = 1f;

    public Toggle transparent;

    void Awake()
    {
        inputActions = new InputMaster();
        inputActions.Camera.Mouse.performed += value => _mousePosition = value.ReadValue<Vector2>();
        inputActions.Camera.Exit.performed += value => ExitPressed();
        inputActions.Camera.RotateLeft.performed += value => RotateObjectLeft();
        inputActions.Camera.RotateRight.performed += value => RotateObjectRight();
    }

    private void ExitPressed()
    {
        if (_addingNewPipe)
        {
            Destroy(pipeToAdd.gameObject);
            _addingNewPipe = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_addingNewPipe)
        {
            float distance = GetDistanceToMouse();

            pipeToAdd.position = mainCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, distance));

            CheckForSnapPoints();
        }
    }

    private void CheckForSnapPoints()
    {
        //set pipe color to red
        Transform child = pipeToAdd.GetChild(0);
        MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.red;
        
        List<SnapPoint> points = GetClosestSnapPoints();

        if (points != null && points.Count != 0)
        {
            SnapPoint closestSnapPoint = points.First();
            
            //set pipe color to green
            meshRenderer.material.color = Color.green;

            pipeToAdd.position = closestSnapPoint.transform.position + (closestSnapPoint.transform.forward * 0.5f);
            
            RotatePipeToMatchSnapPoint(closestSnapPoint);

            //left mouse click
            if (inputActions.Camera.Click.triggered)
            {
                meshRenderer.material.color = Color.white;

                
                Transform pipe = CreateNewPipe(pipeToAdd.position, pipePrefabCopy, true);
                pipe.rotation = pipeToAdd.rotation;

                SetSnapPointsAsTaken(pipe,closestSnapPoint);

                Destroy(pipeToAdd.gameObject);
                pipes.Add(pipe);
                _addingNewPipe = false;
            }
        }
    }

    private void SetSnapPointsAsTaken(Transform pipe, SnapPoint closestSnapPoint)
    {
        closestSnapPoint.transform.parent.parent.GetComponent<Pipe>().SetSnapPointAsTaken(closestSnapPoint.transform);
        foreach (SnapPoint snapPoint in pipe.GetComponent<Pipe>().pipeSnapPoints)
        {
            if (Vector3.Distance(snapPoint.transform.position, closestSnapPoint.transform.position) <= 1f)
            {
                snapPoint.taken = true;
            }
        }
    }

    private void RotatePipeToMatchSnapPoint(SnapPoint snapPoint)
    {
        if (pipeToAdd.GetComponent<Pipe>().standingUp)
        {
            if (pipeToAdd.up != snapPoint.transform.forward)
            {
                pipeToAdd.up = snapPoint.transform.forward;
            }
        }
        else
        {
            if (pipeToAdd.forward != -snapPoint.transform.forward)
            {
                pipeToAdd.forward = -snapPoint.transform.forward;
            }
        }
    }

    private void RotateObjectLeft()
    {
        if (!_addingNewPipe) return;
        if (pipeToAdd.GetComponent<Pipe>().standingUp)
        {
            pipeToAdd.Rotate(0f, 45f, 0);
        }
        else
        {
            pipeToAdd.Rotate(0f, 0f, 45f);
        }
    }

    private void RotateObjectRight()
    {
        if (!_addingNewPipe) return;
        if (pipeToAdd.GetComponent<Pipe>().standingUp)
        {
            pipeToAdd.Rotate(0f, -45f, 0);
        }
        else
        {
            pipeToAdd.Rotate(0f, 0f, -45f);
        }
    }

    private float GetDistanceToMouse()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(_mousePosition);

        float distance;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            distance = Vector3.Distance(mainCamera.transform.position, hit.point);
            distance -= 1f;
        }
        else
        {
            distance = 100f;
        }

        return distance;
    }

    public void AddNewPipe(Transform pipeTransform)
    {
        if (_addingNewPipe)
        {
            Destroy(pipeToAdd.gameObject);
        }

        if (transparent.isOn && !pipeTransform.name.Contains("Transparant"))
        {
            return;
        }

        pipePrefabCopy = pipeTransform;

        pipeToAdd = CreateNewPipe(mainCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x,_mousePosition.y,0)), pipeTransform);
        
        _addingNewPipe = true;
    }

    public Transform CreateNewPipe(Vector3 position, Transform pipe, bool final = false)
    {
        if (pipeParent == null)
        {
            GameObject emptyGO = new GameObject("Pipes");
            pipeParent = emptyGO.transform;
        }

        Transform newPipe = Instantiate(pipe, position,pipe.rotation, pipeParent);

        if (!final)
        {
            //ignore raycast layer
            newPipe.gameObject.layer = 2;
            foreach (Transform child in newPipe)
            {
                child.gameObject.layer = 2;
            }
        }

        return newPipe;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
    private List<SnapPoint> GetClosestSnapPoints()
    {
        List<SnapPoint> pointsFromNewPipe = pipeToAdd.GetComponent<Pipe>().pipeSnapPoints;

        float lowestDistance = 100000f;
        List<SnapPoint> closestSnapPoints = new List<SnapPoint>();
        foreach (Transform pipe in pipes)
        {
            List<SnapPoint> snapPoints = pipe.GetComponent<Pipe>().pipeSnapPoints;
            foreach (SnapPoint snapPoint in snapPoints)
            {
                if (snapPoint.taken)
                {
                    continue;
                }

                //calculate distance for each point on the new pipe
                foreach (SnapPoint point in pointsFromNewPipe)
                {
                    float distance = Vector3.Distance(point.transform.position, snapPoint.transform.position);
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        closestSnapPoints.Clear();
                        closestSnapPoints.Add(snapPoint);
                        closestSnapPoints.Add(point);
                    }
                }
            }
        }

        
        if (lowestDistance <= snapDistance && closestSnapPoints.Count != 0)
        {
            return closestSnapPoints;
        }

        return null;
    }

}
