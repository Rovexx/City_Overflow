using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PipeController : MonoBehaviour
{
    public PlayerInputActions inputActions;
    private Vector2 _mousePosition;
    public Transform pipeToAdd;
    public Camera mainCamera;
    public LayerMask snapPointLayerMask;
    public List<Transform> pipes;
    private RaycastHit _lastRaycastHit;

    public bool addingNewPipe = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.MousePosition.performed += value => _mousePosition = value.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (addingNewPipe)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(_mousePosition);
            
            if (Physics.Raycast(ray, out hit, 1000f, snapPointLayerMask))
            {
                _lastRaycastHit = hit;

                MeshRenderer meshRendererHit = hit.transform.GetComponent<MeshRenderer>();
                meshRendererHit.material.color = Color.red;

                if (inputActions.Player.Fire.triggered)
                {
                    hit.transform.parent.GetComponent<Pipe>().SetSnapPointAsTaken(hit.transform);
                    CreateNewPipe(hit.point,pipeToAdd);
                }
            }
            else
            {
                if (_lastRaycastHit.transform != null)
                {
                    MeshRenderer meshRendererHit = _lastRaycastHit.transform.GetComponent<MeshRenderer>();
                    meshRendererHit.material.color = Color.white;
                }
            }
        }
    }

    public void AddNewPipe(Transform pipeTransform)
    {
        pipeToAdd = pipeTransform;
        addingNewPipe = true;
        ShowPipeSnapPoints();
    }

    public void CreateNewPipe(Vector3 position, Transform pipe)
    {
        Transform newPipe = Instantiate(pipe, position, Quaternion.identity);
        pipes.Add(newPipe);
        addingNewPipe = false;
        HidePipeSnapPoints();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    private void ShowPipeSnapPoints()
    {
        foreach (Transform pipe in pipes)
        {
            pipe.GetComponent<Pipe>().ShowSnapPoints();
        }
    }
    private void HidePipeSnapPoints()
    {
        foreach (Transform pipe in pipes)
        {
            pipe.GetComponent<Pipe>().HideSnapPoints();
        }
    }
}
