using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Win : MonoBehaviour
{
    public static Win Instance;

    public GameObject WinMsg;
    public TMP_Text message;

    public void Show(string text)
    {
        WinMsg.SetActive(true);
        message.text = text;
    }
    
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
