using TMPro;
using UnityEngine;

public class MainScriptFinderForSupply : MonoBehaviour
{
    [SerializeField] private SupplyRoomGraphExercise scriptToSendAnswer;
    [SerializeField] private TMP_InputField inputField;
    void Start()
    {
        if (inputField.onEndEdit == null)
        {
            //inputField.onEndEdit.
        }
    }
}
