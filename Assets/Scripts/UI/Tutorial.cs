using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    private int pageNumber = 0;

    private void Start()
    {
        //pages = GameObject.FindGameObjectsWithTag("tutorial_page").ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextPage();
        }
    }

    private void ShowNextPage()
    {
        pages[pageNumber].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        pageNumber++;
        if (pageNumber > pages.Count - 1)
        {
            pageNumber = 0;
        }
        pages[pageNumber].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }
}
