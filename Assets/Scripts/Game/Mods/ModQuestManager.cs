using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModQuestManager : MonoBehaviour
{
	private List<Quest> conditions = new List<Quest>();
	private List<GameObject> questViews = new List<GameObject>();
	[SerializeField] private Transform questsParent;
	[SerializeField] private GameObject questViewPrefab;

	public void AddCondition(Quest condition)
	{
		conditions.Add(condition);
	}

	public void Awake()
	{
		SceneManager.activeSceneChanged += (Scene old, Scene newS) =>
		{
			if (MenuManager.Instance.isModded)
			{
				if (newS.buildIndex == 0)
				{
					ClearAllConditions();
					return;
				}
				if (newS.buildIndex == 1)
				{
					InitializeConditions();
					return;
				}
			}
		};
	}

	public void ClearAllConditions()
	{
		Debug.Log("CLR");
		StopAllCoroutines();
		conditions = new List<Quest>();
		if (questViews.Count > 0)
		{
			foreach (var quest in questViews)
			{
				Destroy(quest);
			}
		}
		questViews = new List<GameObject>();
	}

	private void InitializeConditions()
	{
		Debug.Log("INIT");
		foreach (var condition in conditions)
		{
			var conditionView = Instantiate(questViewPrefab, questsParent);
			questViews.Add(conditionView);
			Texture2D SpriteTexture = new Texture2D(2, 2);
			SpriteTexture.LoadImage(File.ReadAllBytes(condition.icon_path));
			Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));
			conditionView.GetComponentsInChildren<Image>()[1].sprite = NewSprite;
			conditionView.GetComponentInChildren<TextMeshProUGUI>().text = $"<b>>><u>{condition.name}</b></u>: {condition.description}";
			StartCoroutine(StartConditionChecker(condition));
		}
	}

	private IEnumerator StartConditionChecker(Quest quest)
	{
		Debug.Log("CKR");
		switch (quest.condition.Key)
		{
			case "SurviveFor":
				while (true)
				{
					if (GameManager.Instance.GetTimePast() >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
				break;
			case "Astroluminite":
				while (true)
				{
					if (GameManager.Instance.GetAstroluminite().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
				break;
			case "Energohoney":
				while (true)
				{
					if (GameManager.Instance.GetHoney().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						break;
					}
					yield return null;
				}
				break;
			case "Ursowax":
				while (true)
				{
					if (GameManager.Instance.GetUrsowaks().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
			case "Prototype":
				while (true)
				{
					if (GameManager.Instance.GetPrototype().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
				break;
			case "HNY":
				while (true)
				{
					if (GameManager.Instance.GetHNY().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
				break;
			case "Asterium":
				while (true)
				{
					if (GameManager.Instance.GetAsteriy().Result >= quest.condition.Value)
					{
						questViews[conditions.IndexOf(quest)].GetComponent<Image>().enabled = true;
						conditions.Remove(quest);
						yield break;
					}
					yield return null;
				}
				break;
		}
	}
}
