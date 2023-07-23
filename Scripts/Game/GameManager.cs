using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<Player> players = new List<Player>();
    

    public TMP_Text myCoins;
    
    [SyncVar]
    public int remainingCoins = 20;
    // Start is called before the first frame update
    
    void Start()
    {
        GetComponent<NetworkMatch>();
        
        
        
    }

    [Command]
    public void spawnObj(GameObject obj)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeCoind(int myMoney)
    {
        
    }
}
