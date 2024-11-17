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
    private Coroutine randomWalk;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        onLadder = laddersAmount > 0;

        if (chased)
        {
            Debug.Log(randomWalk);
            if (randomWalk != null)
            {
                StopCoroutine(randomWalk);
                randomWalk = null;
            }
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -20f);
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
            if (randomWalk == null)
            {
                randomWalk = StartCoroutine(WalkCycle());
                Debug.Log("стартовали корутину");
            }
            rb.useGravity = !onLadder;
            rb.linearVelocity = new Vector3(dir.x, onLadder ? 0f : rb.linearVelocity.y, 0f);
        }
    }

    private IEnumerator WalkCycle()
    {
        RaycastHit hit;
        dir.x = Random.Range(0, 2) == 1 ? 1 : -1;
        while (!chased)
        {
            float timer = 7f + Random.value;
            while (timer > 0f)
            {
                //rb.linearVelocity = new Vector3(dir.x, onLadder ? 0f : rb.linearVelocity.y, 0f);
                // Uncomment if wanna see cool rays-detectors
                Debug.DrawRay(transform.position, Vector3.right, Color.yellow); 
                Debug.DrawRay(transform.position, Vector3.left, Color.yellow); 
                if (((Physics.Raycast(transform.position, Vector3.right, out hit, 1f) && dir.x == 1f) || (Physics.Raycast(transform.position, Vector3.left, out hit, 1f) && dir.x == -1f)) && hit.transform.gameObject.layer == 0)
                {
                    Debug.Log("разворот");
                    yield return new WaitForSeconds(3f);
                    break;
                }
                timer -= Time.deltaTime;
                yield return null;
            }
            dir.x *= -1f;
            yield return null;
        }
        yield return null;
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
