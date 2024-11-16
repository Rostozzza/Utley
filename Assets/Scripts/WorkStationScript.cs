using UnityEngine;

public class WorkStationScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("unit"))
        {
            //other.GetComponent<UnitScript>().
        }
    }
}
