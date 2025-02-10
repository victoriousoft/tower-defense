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

			buttons[i].clicked += () => LoadLevel(levelScenes[index]);
		}
	}

	void LoadLevel(string levelPath)
	{
		string sceneName = levelPath.Replace("Assets/", "").Replace(".unity", "");
		SceneManager.LoadScene(sceneName);
	}
}
