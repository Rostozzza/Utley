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
            CameraMove();
        }
        
        if (Input.mouseScrollDelta.y != 0)
        {
            zoom = GameManager.Instance.mode == GameManager.Mode.Build ? -20f : Mathf.Clamp(Input.mouseScrollDelta.y + zoom, -20, -10);
            CameraMove();
        }
        
    }

    public void CameraMove()
    {
        Camera.main.transform.position = new Vector3(
        Mathf.Clamp(Camera.main.transform.position.x, -25f, 25f), 
        Mathf.Clamp(Camera.main.transform.position.y, -19f, 17.5f), 
        GameManager.Instance.mode == GameManager.Mode.Build ? -20f : zoom
        );
    }
}