using UnityEngine;

[ExecuteInEditMode]
public class RayDrawer : MonoBehaviour
{
    [SerializeField] bool show;

    void OnDrawGizmos()
    {
        if (show)
        {
            //Debug.Log("Рисуем лучи космодрома");
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.rotation * Vector3.right * 2f);
        }
    }
}