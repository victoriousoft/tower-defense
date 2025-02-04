using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLMessageHandler : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void SendMessageToJS(string message);

	[DllImport("__Internal")]
	private static extern void InitMessageListener();

	[System.Serializable]
	public class BrowserMessage
	{
		public string action;
		public object args;
	}

	void Start()
	{
		InitMessageListener();
		BrowserMessage message = new BrowserMessage { action = "ready", args = null };

		string jsonMessage = JsonUtility.ToJson(message);
		Debug.Log("UNITY - Sending message to JavaScript: " + jsonMessage);
		SendToJavaScript(message);
	}

	public void SendToJavaScript(BrowserMessage message)
	{
		SendMessageToJS(JsonUtility.ToJson(message));
	}

	public void ReceiveFromJavaScript(object message)
	{
		Debug.Log("UNITY - Received message from JavaScript: " + message);
	}
}
