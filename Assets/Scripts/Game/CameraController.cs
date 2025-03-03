using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensetive = 0.1f;
    [SerializeField] private float zoom = -20;
    private int left, top, right, bottom;
    private bool needToMoveByMousePos; 
    private bool isScroll = true;
    Coroutine moving = null;
    Coroutine changingZoom = null;
    public bool isCameraLocked = false;

    private void Start()
    {
        InitPixels();
    }

    public void InitPixelsDelayed()
    {
        Invoke(nameof(InitPixels), 1f);
    }

    public void InitPixels()
    {
        left = 0;
        right = Camera.main.pixelWidth;
        top = Camera.main.pixelHeight;
        bottom = 0;
    }

    void Update()
    {
        if (!isCameraLocked) CameraHolder();
    }

    private void CameraHolder()
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
        if ((Input.mouseScrollDelta.y != 0 && isScroll) || (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (Input.mouseScrollDelta.y != 0 && isScroll)
            {
                zoom = GameManager.Instance.mode == GameManager.Mode.Build ? -20f : Mathf.Clamp(Input.mouseScrollDelta.y + zoom, -20, -10);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                changingZoom ??= StartCoroutine(ZoomChange());
            }
            CameraMove();
        }
    }

    private IEnumerator ZoomChange()
    {
        int value;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            value = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            value = -1;
        }
        else
        {
            value = 0;
        }
        zoom += value * Time.deltaTime * 10;
        CameraMove();
        while ((Input.GetKey(KeyCode.UpArrow) && value == 1) || (Input.GetKey(KeyCode.DownArrow) && value == -1))
        {

            zoom += value * Time.deltaTime * 10;
            CameraMove();
            yield return null;
        }
        changingZoom = null;
    }

    public void CameraMove()
    {
        Camera.main.transform.position = new Vector3(
        Mathf.Clamp(Camera.main.transform.position.x, -25f, 25f), 
        Mathf.Clamp(Camera.main.transform.position.y, -19f, 17.5f), 
        GameManager.Instance.mode == GameManager.Mode.Build ? -20f : Mathf.Clamp(zoom, -20, -10)
        );
    }

    public void MoveToPoint(Vector2 pos)
    {
        if (moving != null)
        {
            StopCoroutine(moving);
        }
        moving = StartCoroutine(SmoothMove(pos));
    }

    private IEnumerator SmoothMove(Vector2 pos)
    {
        float timer = 0.75f;
        while (Vector2.Distance(transform.position, pos) > 0.1f && timer > 0)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, transform.position.z), Time.deltaTime * 5);
            timer -= Time.deltaTime;
            yield return null;
        }
        moving = null;
    }

    //private bool IsTouch()
    //{
    //    return EventSystem.current.IsPointerOverGameObject();
    //}

    public void SetScroll(bool set)
    {
        isScroll = set;
    }

    public void SetCameraLock(bool set)
    {
        isCameraLocked = set;
    }
}