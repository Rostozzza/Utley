using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    private int pageNumber = 0;
    public bool isHide = false;
    [SerializeField] private GameObject button;

    private void Start()
    {
        ShowNextPage();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return) && !isHide)
    //    {
    //        ShowNextPage();
    //    }
    //}

    public void ShowNextPage()
    {
        if (pageNumber == 0)
        {
            StartCoroutine(TaskMove(pages[pageNumber], new Vector3(pages[pageNumber].transform.position.x, 
                                                                   pages[pageNumber].transform.position.y + 176, 
                                                                   pages[pageNumber].transform.position.z)));
            pageNumber++;
        }
        else
        {
            if (pageNumber == 1) StartCoroutine(TaskHide(pages[pageNumber - 1]));
            pageNumber++;
            if (pageNumber > pages.Count - 1)
            {
                StartCoroutine(TaskHide(pages[pageNumber - 1]));
                ToggleTablet();
                button.SetActive(false);
                gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(TaskSequence(pages[pageNumber - 1], pages[pageNumber], new Vector3(pages[pageNumber].transform.position.x, 
                                                                                                  pages[pageNumber].transform.position.y + 176, 
                                                                                                  pages[pageNumber].transform.position.z)));
            }
            
        }
        //pages[pageNumber].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    private IEnumerator TaskMove(GameObject task, Vector3 pos)
    {
        while (Vector3.Distance(task.transform.position, pos) > 0.1f)
        {
            task.transform.position = Vector3.Lerp(task.transform.position, pos, Time.deltaTime * 2);
            yield return null;
        }
    }

    private IEnumerator TaskHide(GameObject task)
    {
        //TextMeshProUGUI text = task.GetComponentInChildren<TextMeshProUGUI>();
        //while (text.color.a > 0.1f)
        //{
        //    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1, 0, Time.deltaTime));
        //    yield return null;
        //}
        //text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        task.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        yield return null;
    }

    private IEnumerator TaskSequence(GameObject previousTask, GameObject nextTask, Vector3 pos)
    {
        yield return StartCoroutine(TaskHide(previousTask));
        yield return StartCoroutine(TaskMove(nextTask, pos));
    }

    public void ToggleTablet()
    {
        StopAllCoroutines();
        isHide = !isHide;
        if (isHide)
        {
            transform.parent.localPosition = new Vector3(-0.00124570006f, -275.299988f, 0);
        }
        else
        {
            transform.parent.localPosition = new Vector3(-0.00124570006f, -175.176788f, 0);
        }
    }

    //private IEnumerator TaskMonitor()
    //{
    //    yield return Task(1);
    //    yield return Task(2);
    //}

    //private IEnumerator Task(int num)
    //{
    //    switch (num)
    //    {
    //        case 1:
    //            yield return null;
    //            break;
    //        case 2:
    //            yield return null;
    //            break;
    //    }
    //}
}
