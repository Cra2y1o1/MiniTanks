using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;
using ColorUtility = UnityEngine.ColorUtility;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    
    public static Player localPlayer;
    
    private Joystick joystick;
    //flag for flip
    private bool facingRight;
    
    public int money;
    private NetworkMatch NetworkMatch;
    public bool inGame;
    public string roomId;
    
    [SyncVar(hook = nameof(OnColorChanged))] public Color playerColor;

    public GameObject _win;
    
    [Header("Health")]
    [SyncVar(hook = nameof(OnHealthChanged))] public int health = 5;
    public TextMesh healthBar;
    
    [Header("Bullet")]
    public GameObject bullet;
    public Transform shootPoint;

    public override void OnStartClient()
    {
        if (isLocalPlayer) localPlayer = this;
    }
    
    private void Awake()
    {
        NetworkMatch = GetComponent<NetworkMatch>();
    }
    void Start()
    {
        inGame = false;
        facingRight = true;
        //localPlayer = this;
        money = 0;
        
    }

    private void Update()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (inGame && players.Length == 1)
        {
            string text =
                $"<color=#{ColorUtility.ToHtmlStringRGB(playerColor)}>Вы</color> победили, и ваш счет составляет: {money}";
            GameObject.FindWithTag("UIControl").GetComponent<Win>().Show(text);
           

            inGame = false;
        }
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        // Update the player's material color to match their assigned color
        healthBar.text = new string('-', newValue);
    }
    private void OnColorChanged(Color oldValue, Color newValue)
    {
        // Update the player's material color to match their assigned color
        GetComponent<SpriteRenderer>().color = newValue;
    }
    
    #region Moving

    //moving
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (inGame)
            {
                
                float horisontal = joystick.Horizontal;
                float vertical = joystick.Vertical;
                float speed = 6f * Time.deltaTime;
                transform.Translate(new Vector2(horisontal * speed, vertical * speed));

                if (joystick.Horizontal > 0f && !facingRight)
                {
                    flip();
                }
                else if (joystick.Horizontal < 0f && facingRight)
                {
                    flip();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TakeDamage(1);
            }
        }
        
    }
    //flip face
    private void flip()
    {
        facingRight = !facingRight;
        var Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    //activate joystick
    public void addControl()
    {
        joystick = GameObject.FindWithTag("Joystick").GetComponent<Joystick>();
        
    }

    #endregion
    
    #region Coins
    private void changeMoney(int change)
    {
        money += change;

        GameObject.FindWithTag("MoneyText").GetComponent<TMP_Text>().text = "Ваш счет: " + money;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.tag == "money")
            {
                var r = other.gameObject;
                CmdDestroyObj(r);
                changeMoney(1);
            }
        }

    }

    #endregion
    
    #region Shoot

    public void shoot()
    {

        Debug.Log("Shoot");
        float rotateZ = transform.localScale.x;
        if (rotateZ > 0f)  rotateZ = 0;
        else rotateZ = 180f;
        CmdShoot(shootPoint.position,rotateZ, roomId);
        
    }
    [Command]
    public void CmdShoot(Vector3 position ,float rotateZ, string id)
    {
        GameObject obj = Instantiate(bullet, position, Quaternion.Euler(0f, 0f, rotateZ));
        NetworkServer.Spawn(obj);
        obj.GetComponent<NetworkMatch>().matchId = id.ToGuid();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health == 0)
        {
            health = 5;
            NetworkMatch.matchId = "---".ToGuid();
            
        }
    }

    

    #endregion
    
    #region Create room

    public void createRoom(string id)
    {
        roomId = id;
        CmdCreateGame(id);
    }

    [Command]
    public void CmdCreateGame(string id)
    {
        
        if (LobbyMenu.instance.HostGame(id, gameObject))
        {
            Debug.Log("комната почти создана!");
            NetworkMatch.matchId = id.ToGuid();
            TargetHostGame(true, id);
        }
        else
        {
            Debug.Log("Ошибка в создании комнаты");
            TargetHostGame(false, id);
        }
    }
    
    [TargetRpc]
    void TargetHostGame(bool success, string ID)
    {
        LobbyMenu.instance.CreateSuccess(success, ID);
        Debug.Log($"ID == {ID}");
        roomId = ID;
    }

    public void startGame()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        CmdStartGame(roomId);
        Debug.Log("Комната создана");
    }
    
    [Command]
    public void CmdStartGame(string id)
    {
        LobbyMenu.instance.startGame(id);
    }

    #endregion

    #region Connect to room

    public void connectRoom(string id)
    {
        roomId = id;
        CmdConnectGame(id);
    }

    [Command]
    public void CmdConnectGame(string id)
    {
        
        if (LobbyMenu.instance.JoinGame(id, gameObject))
        {
            Debug.Log("Комната найдена");
            NetworkMatch.matchId = id.ToGuid();
            TargetJoinGame(true, id);
            playerColor = Color.blue;
        }
        else
        {
            Debug.Log("Ошибка в подключении");
            TargetJoinGame(false, id);
        }
    }
    
    [TargetRpc]
    void TargetJoinGame(bool success, string ID)
    {
        roomId = ID;
        LobbyMenu.instance.JoinSuccess(success, ID);
        Debug.Log($"ID == {ID}");
        
    }

    public void ConnectGame()
    {
        
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        CmdConnectStartGame(roomId);
        Debug.Log("Комната создана");
        //GetComponent<SpriteRenderer>().color = Color.blue;
        

    }
    
    [Command]
    public void CmdConnectStartGame(string id)
    {
        //LobbyMenu.instance.startGame(id);
    }

    #endregion
    
    [Command]
    public void CmdDestroyObj(GameObject obj)
    {
        Destroy(obj);
    }
}
