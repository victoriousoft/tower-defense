using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLMessageHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendMessageToJS(string message);

    [DllImport("__Internal")]
    private static extern void InitMessageListener();

    void Start()
    {
        InitMessageListener();
        SendMessageToJS("Hello from Unity");
    }

    public void SendToJavaScript(string message)
    {
        Debug.Log("UNITY - Sending message to JavaScript: " + message);
        SendMessageToJS(message);
    }

    public void ReceiveFromJavaScript(string message)
    {
        Debug.Log("UNITY - Received message from JavaScript: " + message);
    }
}