using UnityEngine;

public class PointerFloat : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float speed;
    private float startPos = 0;
    private float time;

    void Update()
    {
        time += Time.deltaTime;
        //GetComponent<RectTransform>().pivot = new Vector2(Floating());
    }

    //float Floating(float time, float speed, float amplitude)
    //{
//
    //}
}
