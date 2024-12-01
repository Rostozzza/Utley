using TMPro.Examples;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensetive = 0.1f;
    [SerializeField] private float zoom = -20;
    private int left, top, right, bottom;
    private bool needToMoveByMousePos; 

    private void Start()
    {
        left = 0;
        right = Camera.main.pixelWidth;
        top = Camera.main.pixelHeight;
        bottom = 0;
    }

    void Update()
    {
        needToMoveByMousePos = Input.mousePosition.x < (left + 5) || Input.mousePosition.x > (right - 5) || Input.mousePosition.y < (bottom + 5) || Input.mousePosition.y > (top - 5);
        if (needToMoveByMousePos || Input.GetMouseButton(2) || Input.touchCount > 1)
        {
            if (needToMoveByMousePos)
            {
                Camera.main.transform.position += (Input.mousePosition - new Vector3(right / 2, top / 2)).normalized * sensetive * 3f;
            }
            else
            {
                Camera.main.transform.position += new Vector3(Input.mousePositionDelta.x, Input.mousePositionDelta.y) * sensetive * -1f;
            }
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