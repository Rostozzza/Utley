using UnityEditor.Callbacks;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    [SerializeField] public bool chased;
    [SerializeField] public float speed;
    private bool onLadder;
    private Rigidbody rb;
    private Vector3 dir;
    private int laddersAmount;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        onLadder = laddersAmount > 0;

        if (chased)
        {
            rb.useGravity = !onLadder;
            dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * (onLadder ? 1f : 0f), 0f);
            rb.linearVelocity = new Vector3(speed * dir.x, speed * dir.y, 0f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ladder_room"))
        {
            laddersAmount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ladder_room"))
        {
            laddersAmount--;
        }
    }
}
