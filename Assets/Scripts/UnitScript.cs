using System.Collections;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    [SerializeField] public bool chased;
    [SerializeField] public float speed = 5f;
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
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            job = Jobs.None;
            rb.useGravity = !onLadder;
            dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            rb.linearVelocity = new Vector3(speed * dir.x, onLadder ? speed * dir.y : rb.linearVelocity.y, 0f);
            if (Input.GetKeyDown(KeyCode.E) && nearWorkStation != null)
            {
                isBusy = true;
                chased = false;
                job = (Jobs)nearWorkStation.GetComponent<WorkStationScript>().GetJob();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                chased = false;
            }
        }
        else
        {
            if (job != Jobs.None)
            {
                StartCoroutine(WalkCycle());
            }
        }
    }

    private IEnumerator WalkCycle()
    {
        dir.x = Random.Range(0, 2) == 1 ? 1 : -1;
        float previousX = transform.position.x - 1f;
        while (job != Jobs.None)
        {
            float timer = 7f + Random.value;
            dir.x *= -1f;
            while (timer > 0f)
            {
                if (previousX == transform.position.x)
                {
                    yield return new WaitForSeconds(3f);

                }
                rb.linearVelocity = new Vector3(dir.x, onLadder ? speed * dir.y : rb.linearVelocity.y, 0f);
                timer -= Time.deltaTime;
            }
            previousX = transform.position.x;
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

    public void ChooseUnit()
    {
        chased = true;
    }

    enum Jobs
    {
        None,
        Energohoney,
        Asterium
    }
}
