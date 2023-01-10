using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using NativeWebSocket;
using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using WebSocket = NativeWebSocket.WebSocket;

static class Globals
{
    public static string Com { get; set; }
    public static string Args { get; set; }
}
public class RequestCommand
{
    public string Command { get; set; }
    public string Arguments { get; set; }
}
public class WSServer : MonoBehaviour
{
    private WebSocketServer wssv;
    private WebSocket ws;
    private Renderer cubeRenderer;
    [SerializeField] private GameObject cube;
    private Color newColor;
    
    // Start is called before the first frame update
    void Start()
    {
        /*
         * looks like I will have to make a new server application based on NativeWebsocked instead
         * There is an issue with WebsocketSharp when building a webGL app
         * https://github.com/endel/NativeWebSocket/blob/master/NodeServer/index.js
         */
        String port = "3000";
        String url = "ws://localhost";
        ws = new WebSocket($"{url}:{port}");
        wssv = new WebSocketServer($"{url}:{port}");
        cubeRenderer = cube.GetComponent<Renderer>();
        ChangeColor();
        cubeRenderer.material.color = newColor;
        
        ws.Receive();
        
        wssv.AddWebSocketService<ProxyBridge>("/ProxyBridge");
        
        wssv.Start();
            
        Debug.Log($"Server Started on {url}:{port}. " + Environment.NewLine + " Press escape to stop the server.");
    }

    public void ChangeColor()
    {
        if (Globals.Com != null)
        {
            
        }
        else
        {
            return;
        }
    }
    private void Update()
    {
        if (wssv == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Closng the server...");
            new WaitForSeconds(3);   
            wssv.Stop();
            Debug.Log("Server closed");
        }
    }
}

public class ProxyBridge : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {   
        Debug.Log("Received message from Proxy client: " + e.Data);
        Send(e.Data);
        try
        {
            var com = JsonConvert.DeserializeObject<RequestCommand>(e.Data);
            Debug.Log($"Deserialized JSON object: {com.Command}, {com.Arguments}");
            Globals.Com = com.Command;
            Globals.Args = com.Arguments;
            // switch (com.Arguments)
            // {
            //     case "blue":
            //         break;
            //     case "red":
            //         break;
            //     case "green":
            //         break;
            //     case "yellow":
            //         break;
            // }
        }
        catch(Exception ex)
        {
            Debug.Log($"Error: {ex.Message}");
        }
    }

    protected override void OnOpen()
    {
        Debug.Log("Server Message: Client connected!");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log("Server Message: Client has been disconnected");
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Debug.Log("ERROR: " + e);
    }
}
