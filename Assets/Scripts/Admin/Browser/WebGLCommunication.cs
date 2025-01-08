using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLMessageHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendMessageToJS(object message);

    [DllImport("__Internal")]
    private static extern void InitMessageListener();

    void Start()
    {
        InitMessageListener();
        SendMessageToJS(new { action = "ready", args = new { } });
    }

    public void SendToJavaScript(object message)
    {
        SendMessageToJS(message);
    }

    public void ReceiveFromJavaScript(object message)
    {
        Debug.Log("UNITY - Received message from JavaScript: " + message);
    }
}