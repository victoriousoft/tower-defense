using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
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
		Debug.Log("UNITY - Current scene name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

		if (Application.isEditor)
			return;

		instance = this;

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
		JObject jsonObject = JObject.Parse(jsonMessage);

		InBrowserMessage message = new InBrowserMessage
		{
			action = jsonObject["action"].ToString(),
			args = jsonObject["args"].ToObject<Dictionary<string, object>>(),
		};

		// log the message nicely to the console
		Debug.Log("UNITY - Received message from JavaScript: " + message.action);
		return;
		foreach (var arg in message.args)
		{
			Debug.Log("UNITY - " + arg.Key + ": " + arg.Value);
		}
		ReceiveFromJavaScript(message);
	}

	public static void SendToJavaScript(OutBrowserMessage message)
	{
		SendMessageToJS(JsonUtility.ToJson(message));
	}

	public static void ReceiveFromJavaScript(InBrowserMessage message)
	{
		Debug.Log("UNITY - Received message from JavaScript: " + message.action);

		switch (message.action)
		{
			case "loadSave":
				Debug.Log("UNITY - Loading save data: " + message.args["levels"]);
				break;

			default:
				Debug.Log("UNITY - Unknown action: " + message);
				break;
		}
	}
}
