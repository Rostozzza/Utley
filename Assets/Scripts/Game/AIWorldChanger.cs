using UnityEngine;
using System.Collections;

public class AIWorldChanger : MonoBehaviour
{
    [SerializeField] private Material corruptedMaterial;

    void Start()
    {
        EventManager.onAnyEnpower.AddListener(UnempowerEnergohoney);
        EventManager.onBuildedRoom.AddListener(UnempowerEnergohoney);
        UnempowerEnergohoney();
    }

    private void UnempowerEnergohoney()
    {
        foreach (var room in GameManager.Instance.allRooms)
        {
            if (room.TryGetComponent(out RoomScript roomScript))
            {
                StartCoroutine(DisableRoomDelayed(roomScript));
            }
        }
    }

    private IEnumerator DisableRoomDelayed(RoomScript room)
    {
        yield return null;
        yield return null;
        yield return null;
        room.renderersToChangeMaterialOnEventAI.ForEach(renderer => renderer.material = corruptedMaterial);
        //energohoney.gameObject.GetComponentsInChildren<Renderer>().ToList().Where(x => x.material == defaultMaterial).ToList().ForEach(y => y.material = corruptedMaterial);
        //energohoney.GetBaseOfRoom().GetComponent<Renderer>().material.SetTexture("", emissive);
    }

    void OnDestroy()
    {
        EventManager.onAnyEnpower.RemoveListener(UnempowerEnergohoney);
        EventManager.onBuildedRoom.RemoveListener(UnempowerEnergohoney);
    }
}
