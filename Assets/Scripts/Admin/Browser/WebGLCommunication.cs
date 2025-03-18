using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLMessageHandler : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void SendMessageToJS(string message);

	[DllImport("__Internal")]
	private static extern bool InitMessageListener();

	[System.Serializable]
	public class BrowserMessage
	{
		public string action;
		public object args;
	}

	private static WebGLMessageHandler instance;

	void Start()
	{
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

		BrowserMessage message = new BrowserMessage { action = "ready", args = null };

		string jsonMessage = JsonUtility.ToJson(message);
		Debug.Log("UNITY - Sending message to JavaScript: " + jsonMessage);
		SendToJavaScript(message);
	}

	public static void SendToJavaScript(BrowserMessage message)
	{
		SendMessageToJS(JsonUtility.ToJson(message));
	}

	public static void ReceiveFromJavaScript(object message)
	{
		Debug.Log("UNITY - Received message from JavaScript: " + message);
	}
}
