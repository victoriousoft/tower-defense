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
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning("MainMenuController already exists, destroying this instance.");
			Destroy(gameObject);
		}
	}

	void Start()
	{
		Debug.Log("MainMenuController started");
		ui = GetComponent<UIDocument>().rootVisualElement;
		buttons = ui.Query<Button>().Where(x => x.ClassListContains("level-btn")).ToList();
		volumeSlider = ui.Query<SliderInt>().Where(x => x.ClassListContains("volume-slider")).First();

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
	}

	public void LockLevels()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			if (i >= GlobalData.instance.levelSheet.levels.Length)
			{
				buttons[i].SetEnabled(false);
			}
		}

		for (int i = PlayerStatsManager.levelStars.Count + 1; i < buttons.Count; i++)
		{
			buttons[i].AddToClassList("lock-overlay");
			buttons[i].SetEnabled(false);
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

	public void SetVolume(int value)
	{
		Debug.Log("Setting volume to: " + value);
		volumeSlider.value = value;
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
