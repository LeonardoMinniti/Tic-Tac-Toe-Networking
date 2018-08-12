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
    public int Rounds;
    public SpriteRenderer X;
    public SpriteRenderer O;
    public static bool IsX;


    public string clientName;

    private List<String> closedMarks;
    private bool socketReady;
    private string tempSendInfo;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private GameObject gameManager;
    private GameManager gameManagerScript;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();
        Rounds = 1;
        gameManagerScript.enabled = false;
        closedMarks = new List<string>();
    }


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

    private void Update()
    {
        if (Server.serverStarted)
        {
            HostButton.SetActive(false);
        }

        if(gameManagerScript.enabled == true)
        {
            if (tempSendInfo != GameManager.tempSendInfo)
            {
                tempSendInfo = GameManager.tempSendInfo;
                Send("&Mark|" + tempSendInfo);
            }
        }

        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }

        GameManager.round = Rounds;
    }

    private void OnIncomingData(string data)
    {
        if(data == "%NAME")
        {
            Send("&NAME|" + clientName);
            return;
        }

        if (data == "&X")
        {
            gameManagerScript.enabled = true;
            IsX = true;
        }
        else if (data == "&O")
        {
            gameManagerScript.enabled = false;
            IsX = false;

        }

        if (!data.Contains("&Mark") && !data.Contains("&X") && !data.Contains("&O"))
        {
            GameObject go = Instantiate(MessagePrefab, ChatContainer.transform);
            go.GetComponentInChildren<Text>().text = data;
        }
        else
        {
            data = data.Split('|')[1];
        }


        if (!closedMarks.Contains(data))
        {
            switch (data)
            {
                case "1.1":
                    ChangeSprite(Rounds, data);
                    break;
                case "1.2":
                    ChangeSprite(Rounds, data);
                    break;
                case "1.3":
                    ChangeSprite(Rounds, data);
                    break;
                case "2.1":
                    ChangeSprite(Rounds, data);
                    break;
                case "2.2":
                    ChangeSprite(Rounds, data);
                    break;
                case "2.3":
                    ChangeSprite(Rounds, data);
                    break;
                case "3.1":
                    ChangeSprite(Rounds, data);
                    break;
                case "3.2":
                    ChangeSprite(Rounds, data);
                    break;
                case "3.3":
                    ChangeSprite(Rounds, data);
                    break;
                default:
                    break;

            }
        }
    }

    private void ChangeSprite(int rounds, string data)
    {
        closedMarks.Add(data);

        if (Rounds % 2 == 0)
        {
            GameObject.Find(data).GetComponent<SpriteRenderer>().sprite = O.sprite;
            if (!IsX)
            {
                gameManagerScript.enabled = false;
            }
            if (IsX)
            {
                gameManagerScript.enabled = true;
            }
            Rounds++;
        }
        else if (Rounds % 2 != 0)
        {
            GameObject.Find(data).GetComponent<SpriteRenderer>().sprite = X.sprite;
            if (IsX)
            {
                gameManagerScript.enabled = false;
            }
            if (!IsX)
            {
                gameManagerScript.enabled = true;
            }
            Rounds++;
        }
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
