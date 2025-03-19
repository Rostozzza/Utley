using UnityEngine;

public class SampleStorage : MonoBehaviour
{
    [SerializeField] public int rightAnswer;
    public int GetRightAnswer()
    {
        return rightAnswer;
    }
}
