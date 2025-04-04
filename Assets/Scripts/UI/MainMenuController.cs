using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
	private VisualElement ui;
	private List<Button> buttons = new();
	private SliderInt volumeSlider;

	public static MainMenuController instance;

	private void Awake()
	{
		Debug.Log("MainMenuController awake");

		if (instance == null)
		{
			instance = this;
			ui = GetComponent<UIDocument>().rootVisualElement;
			buttons = ui.Query<Button>().Where(x => x.ClassListContains("level-btn")).ToList();
			volumeSlider = ui.Query<SliderInt>().Where(x => x.ClassListContains("volume-slider")).First();
		}
		else
		{
			Debug.LogWarning("MainMenuController already exists, destroying this instance.");
			Destroy(gameObject);
		}
	}

	void Start()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			if (i >= GlobalData.instance.levelSheet.levels.Length)
			{
				buttons[i].SetEnabled(false);
				continue;
			}

			LevelSheet.Level level = GlobalData.instance.levelSheet.levels[i];
			buttons[i].text = level.LevelName;
			buttons[i].clicked += () => LoadLevel(level.SceneName);
		}

		volumeSlider.value = GlobalData.instance.volume;

		volumeSlider.RegisterValueChangedCallback(evt =>
		{
			SetVolume(evt.newValue);
		});

		LockLevels();
	}

	public static void LockLevels()
	{
		Debug.Log("Locking levels, buttons: " + instance.buttons);
		for (int i = 0; i < instance.buttons.Count; i++)
		{
			if (i >= GlobalData.instance.levelSheet.levels.Length)
			{
				instance.buttons[i].SetEnabled(false);
			}
		}

		for (int i = PlayerStatsManager.levelStars.Count + 1; i < instance.buttons.Count; i++)
		{
			instance.buttons[i].AddToClassList("lock-overlay");
			instance.buttons[i].SetEnabled(false);
		}
	}

	void LoadLevel(string levelPath)
	{
		PlayerStatsManager.currentLevel = System.Array.FindIndex(
			GlobalData.instance.levelSheet.levels,
			x => x.SceneName == levelPath
		);
		PlayerStatsManager.ResetStats();
		Overlay.ResumeGame();
		SceneManager.LoadScene(levelPath);
	}

	public static void SetVolume(int value)
	{
		instance.volumeSlider.value = value;
		GlobalData.instance.volume = value;
		WebGLMessageHandler.SendToJavaScript(
			new WebGLMessageHandler.OutBrowserMessage
			{
				action = "setVolume",
				args = new Dictionary<string, object> { { "volume", value } },
			}
		);
	}
}
