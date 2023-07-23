using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class MatchEctension
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hasBytes = provider.ComputeHash(inputBytes);
        return new Guid(hasBytes);
    }
}

public class LobbyMenu : NetworkBehaviour
{
    public static LobbyMenu instance;

    
    
    //prefab GameManager
    public GameObject _GameManager;
    public GameObject _coin;

    //name of room
    public TMP_InputField roomName;
    public string roomId;

    public GameObject Mistake;
    public TMP_Text message;
    
    
    //lists matches
   
    public readonly SyncList<string> matchIDs = new SyncList<string>();
    
    private NetworkManager _networkManager;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _networkManager = GameObject.FindObjectOfType<NetworkManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }

    public void CreateRoom()
    {
        Player.localPlayer.createRoom(roomName.text);
    }

    public bool HostGame(string matchID, GameObject player)
    {
        if (!matchIDs.Contains(matchID))
        {
            matchIDs.Add(matchID);
            return true;
        }
        else return false;
    }

    private void showMistake(string text)
    {
        Mistake.SetActive(true);
        message.text = text;
    }

    public void hideMistake()
    {
        Mistake.SetActive(false);
        message.text = "";
    }
    

    public void CreateSuccess(bool success, string ID)
    {
        if (success)
        {
            roomId = ID;
            gameObject.SetActive(false);
            Player.localPlayer.startGame();
            Debug.Log("запускаю игру");
            
        }
        else
        {
            Debug.Log("Невозможно создать такую комнату");
            showMistake("Невозможно создать такую комнату");
        }
    }

    public void startGame(string id)
    {
        GameObject obj = Instantiate(_GameManager);
        NetworkServer.Spawn(obj);
        DontDestroyOnLoad(obj);
        obj.GetComponent<NetworkMatch>().matchId = id.ToGuid();
        spawnCoins(id);
    }

    public void spawnCoins(string id)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 position = new Vector2(Random.Range(-10f, 10f), Random.Range(-4.5f, 4.5f));
            GameObject obj = Instantiate(_coin, position, Quaternion.identity);
            NetworkServer.Spawn(obj);
            obj.GetComponent<NetworkMatch>().matchId = id.ToGuid();
        }
    }
    
    public void ConnectRoom()
    {
        Player.localPlayer.connectRoom(roomName.text);
    }
    
    public bool JoinGame(string matchID, GameObject player)
    {
        if (matchIDs.Contains(matchID))
            return true;
        else 
            return false;
    }
    public void JoinSuccess(bool success, string ID)
    {
        if (success)
        {
            roomId = ID;
            gameObject.SetActive(false);
            Player.localPlayer.ConnectGame();
            Debug.Log("запускаю игру");
            
        }
        else
        {
            Mistake.SetActive(true);
            message.text= "Подключение к комнате не удалось! Возможно введен неверный код комнаты!";
        }
    }
}
