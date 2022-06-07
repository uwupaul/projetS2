using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    public GameObject cameraHolder;
    public GameObject cam;
    
    PhotonView PV;
    PlayerManager playerManager;
    KillFeed KillFeed;

    #region Items
        public Item[] items;
        public Item[] secondItems;
        int itemIndex;
        int previousItemIndex = -1;

        public float sensMultiplier;
    #endregion
        
    #region Physics
        public float sprintSpeed, walkSpeed;
        
        public bool grounded;
        private bool wasGrounded;
        public Transform groundCheck;
        public float groundDistance;
        public LayerMask groundMask;
        
        public float jumpHeight;
        public float airMultiplier;
        
        public float fallSpeedThreshold;
        
        public float gravity;
        private Vector3 velocity;
        
        private float moveSpeed;
        private float xRotation;

        public CharacterController CharacterController;
        public float Speed;
        
    #endregion
    
    #region Health
        const float MaxHealth = 100f;
        public float currentHealth = MaxHealth;
        private HealthBar HealthBar;
    #endregion
    
    #region HUD
    
        private TextMeshProUGUI textHealth;
        private TextMeshProUGUI ui_username;
        private TextMeshProUGUI ui_kills;
        private TextMeshProUGUI ui_death;

        public AudioSource deathAudio;

    #endregion
    
    #region UI
        UIManager UIM;
        bool EscapeMod => UIM.EscapeMod;
        float mouseSensitivity => UIM.settingsMenu.mouseSensitivity;
    #endregion

    private Animator Animator;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        UIM = playerManager.GetComponentInChildren<UIManager>();
        Animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            (items, secondItems) = (secondItems, items);


            #region UI
                ui_username = GameObject.Find("Canvas/BottomLeft/UsernameText").GetComponent<TextMeshProUGUI>();
                ui_kills = GameObject.Find("Canvas/TopLeft/KillsText").GetComponent<TextMeshProUGUI>();
                ui_death = GameObject.Find("Canvas/TopLeft/DeathText").GetComponent<TextMeshProUGUI>();
                textHealth = GameObject.Find("Canvas/BottomLeft/TextHealth").GetComponent<TextMeshProUGUI>();
                HealthBar = GameObject.Find("Canvas/BottomLeft/HealthBar").GetComponent<HealthBar>();
                KillFeed = GameObject.Find("Canvas/KillFeed").GetComponent<KillFeed>();
                
                
                HealthBar.SetMaxHealth(MaxHealth);
                textHealth.text = MaxHealth.ToString();
                textHealth.color = Color.white;
                ui_username.text = PhotonNetwork.LocalPlayer.NickName;
            #endregion
            
            CharacterController.detectCollisions = false;

            EquipItem(0);
            
            if (EscapeMod)
                SettingsMenu.EnableMouse();
            else
                SettingsMenu.DisableMouse();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    void Update()
    {
        //Debug.Log($"PlayerData K : {PlayerData.Instance.globalKills}, D : {PlayerData.Instance.globalDeaths}");
        //Debug.Log($"CustomProperties K : {PhotonNetwork.LocalPlayer.CustomProperties["K"]}, D : {PhotonNetwork.LocalPlayer.CustomProperties["D"]}");

        if(!PV.IsMine)
            return;

        #region Movement
            wasGrounded = grounded;
            grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if (!wasGrounded && grounded)
            {
                if (velocity.y < fallSpeedThreshold)
                {
                    float fallDamage = Mathf.Round(-4f * velocity.y - 27) ;
                    RPC_TakeDamage(fallDamage, null, -1);
                }
            }

            if (grounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
        #endregion

        #region Cheats

            // partie cheats pour ne pas être bloqué lors de la soutenance

            if (Input.GetKeyDown(KeyCode.F9))
            {
                HealthBar.SetMaxHealth(MaxHealth);
                textHealth.text = MaxHealth.ToString();
                textHealth.color = Color.white;
            }

            if(Input.GetKeyDown(KeyCode.F8))
                    Die();

            Emote();

        #endregion
        
        if (!EscapeMod)
        {
            Look(); Jump(); UseItem(); Move();
        }
        else
            items[itemIndex].isEquiped = false;
        
        if (transform.position.y < -5f)
            Die();
    }

    #region Physics Method
    
    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (!grounded)
        {
            horizontalInput *= airMultiplier;
            verticalInput *= airMultiplier;
        }
        
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        
        Vector3 move = Vector3.ClampMagnitude(transform.right * horizontalInput + transform.forward * verticalInput, 1f);

        CharacterController.Move(move * moveSpeed * Time.deltaTime + velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Emote()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            items[itemIndex].Unequip();
            Animator.SetBool("Twerk", true);
        }
            
        if (Input.GetKeyUp(KeyCode.P))
        {
            //items[itemIndex].E();
            Animator.SetBool("Twerk", false);
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            items[itemIndex].Unequip();
            Animator.SetBool("Dance", true);
        }
            
        if (Input.GetKeyUp(KeyCode.M))
        {
            //items[itemIndex].E();
            Animator.SetBool("Dance", false);
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 0.5f * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.5f * sensMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    #endregion

    #region Items Method

    void EquipItemVisually(int _index)
    {
        // Mettre les armes qui sont dans le rig

        if (_index == previousItemIndex)
            return;
        
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        
        if (previousItemIndex != -1)
            items[previousItemIndex].itemGameObject.SetActive(false);

        previousItemIndex = itemIndex;
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;
        
        itemIndex = _index;

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].Unequip();
            //secondItems[previousItemIndex].itemGameObject.SetActive(false);
        }
        
        items[itemIndex].Equip();
        //secondItems[itemIndex].itemGameObject.SetActive(true);
        
        previousItemIndex = itemIndex;
    }

    void UseItem()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                if (i == previousItemIndex)
                    continue;

                itemIndex = i;
                items[itemIndex].Equip();
                secondItems[itemIndex].itemGameObject.SetActive(true);

                if (previousItemIndex != -1)
                {
                    items[previousItemIndex].Unequip();
                    secondItems[previousItemIndex].itemGameObject.SetActive(false);
                }

                previousItemIndex = itemIndex;
            }
        }

        items[itemIndex].isEquiped = true;
    }
    #endregion
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PV.IsMine && targetPlayer == PV.Owner && changedProps.ContainsKey("itemIndex"))
		{
            EquipItemVisually((int)changedProps["itemIndex"]);
            
            //items[itemIndex].Unequip();
            //items[(int)changedProps["itemIndex"]].Equip();
            
            Debug.Log($"itemIndex : {itemIndex}");
            Debug.Log($"changedProps : {(int)changedProps["itemIndex"]}");
		}

        if (!PV.IsMine)
            return;
        
        if (changedProps.ContainsKey("D"))
        {
            Debug.Log($"PC : {targetPlayer.NickName} died {(int)changedProps["D"]} times.");

            if (targetPlayer == PhotonNetwork.LocalPlayer && (int)changedProps["D"] != 0)
            {
                Debug.Log("Deaths ++");
                ui_death.text = "DEATHS : " + Convert.ToString((int)changedProps["D"]);
                PlayerData.Instance.globalDeaths++;
            }
        }

        if (changedProps.ContainsKey("K"))
        {
            Debug.Log($"PC : {targetPlayer.NickName} has {(int) changedProps["K"]} kills.");

            if (targetPlayer == PhotonNetwork.LocalPlayer && (int)changedProps["K"] != 0)
            {
                Debug.Log("Kills ++");
                ui_kills.text = "KILLS : " + Convert.ToString((int)changedProps["K"]);
                PlayerData.Instance.globalKills++;
            }
        }
    }

    /*
    public void TakeDamage(float damage, Player opponent, int gunIndex)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, opponent, gunIndex);
    }
    */
    
    public void TakeDamage(float damage, Player opponent, int gunIndex)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, opponent, gunIndex);
    }

    public void TakeDamage(float damage, string opponentName, int gunIndex)
    {
        PV.RPC("RPC_TakeDamageName", RpcTarget.All, damage, opponentName, gunIndex);
    }

    [PunRPC]
    void RPC_TakeDamageName(float damage, string opponentName, int gunIndex)
    {
        if (!PV.IsMine)
            return;

        Animator.Play("Hit Reaction");
        
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            KillFeed.KillFeedEntry(PV.Controller.NickName, opponentName, gunIndex);

            Die();
        }
        else
        {
            if (currentHealth <= 10)
                textHealth.color = Color.red;
            
            textHealth.text = currentHealth.ToString();
            HealthBar.SetHealth(currentHealth);
            
            // UIManager.Instance.HitEffect();
            StartCoroutine(cam.GetComponent<CameraEffect>().ShakeCamera(0.03f));
        }
    }
    
    [PunRPC]
    void RPC_TakeDamage(float damage, Player opponent, int gunIndex)
    {
        if (!PV.IsMine)
            return;
        
        Animator.Play("Hit Reaction");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            KillFeed.KillFeedEntry(PV.Owner, opponent, gunIndex);

            if (opponent != null)
                ApplyKill(opponent);
            
            Die();
        }
        else
        {
            if (currentHealth <= 10)
                textHealth.color = Color.red;
            
            textHealth.text = currentHealth.ToString();
            HealthBar.SetHealth(currentHealth);
            
            // UIManager.Instance.HitEffect();
            StartCoroutine(cam.GetComponent<CameraEffect>().ShakeCamera(0.03f));
        }
    }

    void ApplyKill(Player player)
    {
        Hashtable H = new Hashtable();
        int deathOfParent = Convert.ToInt32(player.CustomProperties["K"]);
        
        H.Add("K", deathOfParent + 1);
        player.SetCustomProperties(H);

        //PlayerData.Instance.globalKills++;
    }

    void Die()
    {
        deathAudio.Play();
        //items[itemIndex].Unequip();
        playerManager.Die();
    }
}