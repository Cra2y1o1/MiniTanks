using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameMode : MonoBehaviour
{

    public void startServer()
    {
        try
        {
            NetworkManager.singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.Log("Невозможно создать сервер");
        }
        
    }
    
    public void startClient()
    {
        try
        {
            NetworkManager.singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.Log("Невозможно подключиться к серверу");
        }
        
    }
}
