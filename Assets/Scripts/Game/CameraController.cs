using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensetive = 0.1f;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position += new Vector3(Input.mousePositionDelta.x, Input.mousePositionDelta.y) * sensetive * -1f;
        }
    }
}
