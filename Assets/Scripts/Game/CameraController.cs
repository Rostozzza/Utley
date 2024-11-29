using TMPro.Examples;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensetive = 0.1f;
    [SerializeField] private float zoom = -20;
    [SerializeField] private float k;
    [SerializeField] private float f;
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.position += new Vector3(Input.mousePositionDelta.x, Input.mousePositionDelta.y) * sensetive * -1f;
        }
        
        zoom = Mathf.Clamp(Input.mouseScrollDelta.y + zoom, -20, -10);

        Camera.main.transform.position = new Vector3(
        Mathf.Clamp(Camera.main.transform.position.x, -25f, 25f), 
        Mathf.Clamp(Camera.main.transform.position.y, -19f, 17.5f), 
        zoom
        );
    }
}