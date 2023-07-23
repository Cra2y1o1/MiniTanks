using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control : MonoBehaviour
{
    public GameObject block;
    
    // Start is called before the first frame update
    void Start()
    {
        Player.localPlayer.addControl();
    }

    // Update is called once per frame
    void Update()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
        {
            Player.localPlayer.inGame = true;
            gameObject.SetActive(false);
        } 
    }
}
