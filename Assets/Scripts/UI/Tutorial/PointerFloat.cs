using System;
using UnityEngine;

public class PointerFloat : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float speed;
    private float startPivotX;
    private float time;

    private void Start()
    {
        startPivotX = GetComponent<RectTransform>().pivot.x;
    }

    void Update()
    {
        time += Time.deltaTime;
        GetComponent<RectTransform>().pivot = new Vector2(startPivotX + Floating(time, speed, amplitude), GetComponent<RectTransform>().pivot.y);
    }

    float Floating(float time, float speed, float amplitude)
    {
        return MathF.Sin(time * speed) * amplitude;
    }
}
