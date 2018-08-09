using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System;

public class Client : MonoBehaviour {

    public GameObject ChatContainer;
    public GameObject MessagePrefab;
    public GameObject HostButton;
    public GameObject LoginPanel;
    public GameObject TicTacToe;


    public string clientName;

    private bool socketReady;
    private string tempSendInfo;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public void ConnectToServer()
    {
        if (socketReady)
            return;


        string host;
        string name;
        int port;

        host = GameObject.Find("IPInput").GetComponent<InputField>().text;
        name = GameObject.Find("NameInput").GetComponent<InputField>().text;
        clientName = name;
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out port);

        //create the socket

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
            LoginPanel.SetActive(false);
            TicTacToe.SetActive(true);
        }
        catch(Exception e)
        {
            LoginPanel.SetActive(true);
            TicTacToe.SetActive(false);
            Debug.Log("Socket error: " + e.Message);
        }
    }

    public void OnClickSend()
    {

    }
    private void Update()
    {
        if (Server.serverStarted)
        {
            HostButton.SetActive(false);
        }

        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
                //X and O
            }
        }

        if(tempSendInfo != Mark.sendInfo)
        {
            tempSendInfo = Mark.sendInfo;
            Send("&Mark|" + tempSendInfo);
            
        }
    }

    private void OnIncomingData(string data)
    {
        if(data == "%NAME")
        {
            Send("&NAME|" + clientName);
            return;
        }
        GameObject go = Instantiate(MessagePrefab, ChatContainer.transform);
        go.GetComponentInChildren<Text>().text = data;
    }

    private void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton()
    {
        //button tic tac toe 
        string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        Send(message);
    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
}
