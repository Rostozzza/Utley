using UnityEditor.Callbacks;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    [SerializeField] public bool chased;
    [SerializeField] public float speed;
    [SerializeField] public bool isBusy;
    private bool onLadder;
    private Rigidbody rb;
    private Vector3 dir;
    private int laddersAmount;
    private Jobs job;
    private GameObject nearWorkStation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        onLadder = laddersAmount > 0;

        if (chased)
        {
            job = Jobs.None;
            rb.useGravity = !onLadder;
            dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            rb.linearVelocity = new Vector3(speed * dir.x, onLadder ? speed * dir.y : rb.linearVelocity.y, 0f);
            if (Input.GetKeyDown(KeyCode.E))
            {
                isBusy = true;
                chased = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ladder_room"))
        {
            laddersAmount++;
        }
        else if (other.CompareTag("work_station"))
        {
            nearWorkStation = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ladder_room"))
        {
            laddersAmount--;
        }
    }

    enum Jobs
    {
        None,
        Energohoney,
        Asterium
    }
}
