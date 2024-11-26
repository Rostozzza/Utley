using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensetive = 0.1f;
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.position += new Vector3(Input.mousePositionDelta.x, Input.mousePositionDelta.y) * sensetive * -1f;
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -25f, 25f), Mathf.Clamp(Camera.main.transform.position.y, -19f, 17.5f), Camera.main.transform.position.z);
        }
    }
}
