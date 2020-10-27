using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private Material _originalMaterial;

    public Camera mainCamera;

    private bool _addingNewPipe = false;
    public float snapDistance = 1f;

    public Toggle transparent;

    public LayerMask pipeLayerMask;

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
        else
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(_mousePosition);

            if (Physics.Raycast(ray, out hit, 1000f, pipeLayerMask))
            {

                if (inputActions.Camera.Delete.triggered)
                {
                    if (pipes[0] != hit.transform.parent)
                    {

                        foreach (Transform pipeinlist in pipes)
                        {
                            if (pipeinlist == hit.transform.parent)
                            {
                                continue;
                            }

                            Pipe pipe = pipeinlist.GetComponent<Pipe>();

                            foreach (SnapPoint point in pipe.pipeSnapPoints)
                            {
                                if (point.taken && point.linkedPipe == hit.transform.parent)
                                {
                                    point.taken = false;
                                    point.linkedPipe = null;
                                }
                            }
                        }

                        hit.transform.parent.gameObject.SetActive(false);
                        hit.transform.parent.position = new Vector3(-9999f, -9999f, -9999f);
                    }
                }

            }
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
                //change back to original material
                meshRenderer.material = _originalMaterial;

                SetSnapPointsAsTaken(pipeToAdd,closestSnapPoint);

                ReEnableCollider();

                pipes.Add(pipeToAdd);

                _addingNewPipe = false;
            }
        }
    }

    private void ReEnableCollider()
    {
        //enable collision
        Transform pipeBody = pipeToAdd.GetChild(0);
        pipeBody.GetComponent<MeshCollider>().enabled = true;

        if (pipeToAdd.name.Contains("Pump"))
        {
            //enable rotor collider
            pipeBody.GetChild(0).GetComponent<MeshCollider>().enabled = true;
        }
    }

    private void SetSnapPointsAsTaken(Transform pipe, SnapPoint closestSnapPoint)
    {
        closestSnapPoint.transform.parent.parent.GetComponent<Pipe>().SetSnapPointAsTaken(closestSnapPoint.transform,pipe);
        foreach (SnapPoint snapPoint in pipe.GetComponent<Pipe>().pipeSnapPoints)
        {
            if (Vector3.Distance(snapPoint.transform.position, closestSnapPoint.transform.position) <= 1f)
            {
                snapPoint.taken = true;
                snapPoint.linkedPipe = pipe;
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
            return;
        }

        if (transparent.isOn && !pipeTransform.name.Contains("Transparant"))
        {
            return;
        }

        if (!transparent.isOn && pipeTransform.name.Contains("Transparant"))
        {
            return;
        }


        pipePrefabCopy = pipeTransform;

        pipeToAdd = CreateNewPipe(mainCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x,_mousePosition.y,0)), pipeTransform);

        _originalMaterial = pipeTransform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;

        _addingNewPipe = true;
    }

    public Transform CreateNewPipe(Vector3 position, Transform pipe)
    {
        if (pipeParent == null)
        {
            GameObject emptyGO = new GameObject("Pipes");
            pipeParent = emptyGO.transform;
        }

        Transform newPipe = Instantiate(pipe, position,pipe.rotation, pipeParent);


        //disable colliders
        Transform pipeBody = newPipe.GetChild(0);
        pipeBody.GetComponent<MeshCollider>().enabled = false;

        if (pipe.name.Contains("Pump"))
        {
            //disable rotor collider
            pipeBody.GetChild(0).GetComponent<MeshCollider>().enabled = false;
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
