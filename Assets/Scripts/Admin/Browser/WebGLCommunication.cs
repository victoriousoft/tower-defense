using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WebGLMessageHandler : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void SendMessageToJS(string message);

	[DllImport("__Internal")]
	private static extern bool InitMessageListener();

	[System.Serializable]
	public class OutBrowserMessage
	{
		public string action;
		public object args;
	}

	public class InBrowserMessage
	{
		public string action;
		public Dictionary<string, object> args;
	}

	private static WebGLMessageHandler instance;

	void Start()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

#if UNITY_EDITOR

		ReceiveFromJavaScript(
			new InBrowserMessage
			{
				action = "loadSave",
				args = new Dictionary<string, object> { { "levels", "[1,3]" } },
			}
		);
		return;
#endif

		bool initRes = InitMessageListener();
		if (!initRes)
		{
			Debug.LogError("Failed to initialize message listener, game is not running in a browser, terminating");
			Time.timeScale = 0;
			return;
		}

		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower() == "mainmenu")
		{
			OutBrowserMessage message = new() { action = "ready", args = null };

			string jsonMessage = JsonUtility.ToJson(message);
			Debug.Log("UNITY - Sending message to JavaScript: " + jsonMessage);
			SendToJavaScript(message);
		}
	}

	public void _ReceiveFromJavaScript(string jsonMessage)
	{
		Debug.Log("UNITY - Received raw message: " + jsonMessage);

		JObject jsonObject = JObject.Parse(jsonMessage);

		InBrowserMessage message = new InBrowserMessage
		{
			action = jsonObject["action"].ToString(),
			args = new Dictionary<string, object>(),
		};

		if (jsonObject["args"] != null)
		{
			JObject argsObject = (JObject)jsonObject["args"];
			foreach (var property in argsObject.Properties())
			{
				message.args[property.Name] = property.Value.ToString();
			}
		}

		ReceiveFromJavaScript(message);
	}

	public static void SendToJavaScript(OutBrowserMessage message)
	{
		Debug.Log("UNITY - Sending message to JavaScript: " + JsonConvert.SerializeObject(message));
		SendMessageToJS(JsonConvert.SerializeObject(message));
	}

	public static void ReceiveFromJavaScript(InBrowserMessage message)
	{
		Debug.Log("UNITY - Received message from JavaScript: " + message.action);

		switch (message.action)
		{
			case "loadSave":
				Debug.Log("UNITY - Loading save data: " + message.args["levels"]); // UNITY - Loading save data: [3,3,3,3]
				string levelsString = message.args["levels"].ToString().Trim('[', ']');
				PlayerStatsManager.levelStars = string.IsNullOrEmpty(levelsString)
					? new List<int>()
					: levelsString.Split(',').Select(int.Parse).ToList();
				MainMenuController.LockLevels();
				break;

			case "setVolume":
				Debug.Log("UNITY - Setting volume: " + message.args["volume"]);
				MainMenuController.SetVolume(int.Parse(message.args["volume"].ToString()));
				break;

			default:
				Debug.Log("UNITY - Unknown action: " + message);
				break;
		}
	}
}
