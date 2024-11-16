using UnityEngine;

public class WorkStationScript : MonoBehaviour
{
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("unit"))
    //    {
    //        //other.GetComponent<UnitScript>().
    //    }
    //}
    [SerializeField] Works work;

    public Works GetJob()
    {
        return work;
    }

    public enum Works
    {
        None,
        Energohoney,
        Asterium
    }
}
