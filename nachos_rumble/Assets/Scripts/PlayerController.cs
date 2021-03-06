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
        int itemIndex;
        int previousItemIndex = -1;
    #endregion
        
    #region Physics
        public float sprintSpeed, walkSpeed;
        
        public bool grounded;
        private bool wasGrounded;
        public Transform groundCheck;
        public float groundDistance;
        public LayerMask groundMask;
        
        public float jumpHeight;
        public float airMultiplyer;
        
        public float fallSpeedThreshold;
        
        public float gravity;
        private Vector3 velocity;
        
        private float moveSpeed;
        private float xRotation;

        public CharacterController CharacterController;
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
        
        public TextMeshPro playerUsername;

        public AudioSource deathAudio;

    #endregion
    
    #region UI
        UIManager UIM;
        bool EscapeMod => UIM.EscapeMod;
        float mouseSensitivity => UIM.settingsMenu.mouseSensitivity;
    #endregion
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        UIM = playerManager.GetComponentInChildren<UIManager>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
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
                ui_username.text = PV.Owner.NickName;
                playerUsername.text = PV.Owner.NickName;
            #endregion
            
            CharacterController.detectCollisions = false;

            EquipItem(0);
            //items[0].Equip();
            
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
        //var v = transform.InverseTransformDirection(CharacterController.velocity);
        //Debug.Log($"CC Z velocity : {v.z}, X : {v.x}");
        
        if(!PV.IsMine)
            return;
        
        #region Movement
            wasGrounded = grounded;
            grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if (!wasGrounded && grounded)
            {
                if (velocity.y < fallSpeedThreshold)
                {
                    float fallDamage = Mathf.Round(-7f * velocity.y - 100f) ;
                    RPC_TakeDamage(fallDamage, null, -1);
                }
            }

            if (grounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            CharacterController.Move(velocity * Time.deltaTime);
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
            horizontalInput *= airMultiplyer;
            verticalInput *= airMultiplyer;
        }
        
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        
        Vector3 move = Vector3.ClampMagnitude((transform.right * horizontalInput + transform.forward * verticalInput), 1f);
        //Vector3 fixedMove = Vector3.Lerp(move, CharacterController.velocity, 5f);
            
        CharacterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 0.5f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.5f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    #endregion

    #region Items Method

    void EquipItemVisually(int _index)
    {
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
            items[previousItemIndex].Unequip();
        
        items[itemIndex].Equip();
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
                
                if (previousItemIndex != -1)
                    items[previousItemIndex].Unequip();

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
            
            if (targetPlayer == PV.Owner)
                ui_death.text = "DEATHS : " + Convert.ToString((int)changedProps["D"]);
        }

        if (changedProps.ContainsKey("K"))
        {
            Debug.Log($"PC : {targetPlayer.NickName} has {(int) changedProps["K"]} kills.");

            if (targetPlayer == PV.Owner)
                ui_kills.text = "KILLS : " + Convert.ToString((int)changedProps["K"]);
        }
    }

    public void TakeDamage(float damage, Player opponent, int gunIndex)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, opponent, gunIndex);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, Player opponent, int gunIndex)
    {
        if (!PV.IsMine)
            return;
        
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

        PlayerData.Instance.globalKills++;
    }

    void Die()
    {
        deathAudio.Play();
        items[itemIndex].Unequip();
        playerManager.Die();
    }
}