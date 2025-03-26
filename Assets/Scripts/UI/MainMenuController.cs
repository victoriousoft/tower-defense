using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
	private VisualElement ui;
	private List<Button> buttons = new();

	private readonly string[] levelScenes = { "Assets/Scenes/Levels/Sandbox.unity" };

	void Awake()
	{
		ui = GetComponent<UIDocument>().rootVisualElement;
		buttons = ui.Query<Button>().Where(x => x.ClassListContains("level-btn")).ToList();

		for (int i = 0; i < buttons.Count; i++)
		{
			int index = i;
			if (index >= levelScenes.Length)
			{
				buttons[i].clicked += () => Debug.Log("Level not found");
				continue;
			}

			PlayerStatsManager.currentLevel = index;
			buttons[i].clicked += () => LoadLevel(levelScenes[index]);
		}
	}

	public void LockLevels()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			if (i >= levelScenes.Length)
			{
				buttons[i].SetEnabled(false);
			}
		}

		Debug.Log("Locking levels");
		Debug.Log("Level stars count: " + PlayerStatsManager.levelStars.Count);
		Debug.Log("Buttons count: " + buttons.Count);
		Debug.Log("Level stars: " + string.Join(", ", PlayerStatsManager.levelStars));
		for (int i = PlayerStatsManager.levelStars.Count + 1; i < buttons.Count; i++)
		{
			buttons[i].AddToClassList("lock-overlay");
			buttons[i].clicked -= () => LoadLevel(levelScenes[i]);
			buttons[i].clicked += () => Debug.Log("Level not unlocked yet");
		}
	}

	void LoadLevel(string levelPath)
	{
		string sceneName = levelPath.Replace("Assets/", "").Replace(".unity", "");
		PlayerStatsManager.ResetStats();
		PlayerStatsManager.currentLevel = System.Array.IndexOf(levelScenes, levelPath);
		SceneManager.LoadScene(sceneName);
	}
}
