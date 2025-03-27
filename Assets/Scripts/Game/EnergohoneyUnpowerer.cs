using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnergohoneyUnpowerer : MonoBehaviour
{
    [SerializeField] private Material corruptedMaterial;
    [SerializeField] private List<Renderer> toChange;

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
            if (room.TryGetComponent(out EnergohoneyRoom energohoney))
            {
                StartCoroutine(DisableRoomDelayed(energohoney));
            }
        }
    }

    private IEnumerator DisableRoomDelayed(EnergohoneyRoom energohoney)
    {
        yield return null;
        yield return null;
        yield return null;
        energohoney.Unpower();
        energohoney.GetBaseOfRoom().GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        energohoney.SetConeierScreenShow(false);
        //energohoney.gameObject.GetComponentsInChildren<Renderer>().ToList().Where(x => x.material == defaultMaterial).ToList().ForEach(y => y.material = corruptedMaterial);
        //energohoney.GetBaseOfRoom().GetComponent<Renderer>().material.SetTexture("", emissive);
    }

    void OnDestroy()
    {
        EventManager.onAnyEnpower.RemoveListener(UnempowerEnergohoney);
        EventManager.onBuildedRoom.RemoveListener(UnempowerEnergohoney);
    }
}
