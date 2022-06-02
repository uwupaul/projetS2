using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [HideInInspector] public ProfileData playerProfile;
    
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

                HealthBar.SetMaxHealth(MaxHealth);
                textHealth.text = MaxHealth.ToString();
                textHealth.color = Color.white;
                ui_username.text = Launcher.myProfile.username; // ou Photon.Nickname ?
                playerUsername.text = Launcher.myProfile.username;
            #endregion
            
            CharacterController.detectCollisions = false;

            EquipItem(0);
            
            if (EscapeMod)
                SettingsMenu.EnableMouse();
            else
                SettingsMenu.DisableMouse();
        }
        else
            Destroy(GetComponentInChildren<Camera>().gameObject);
    }

    void Update()
    {
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
                    RPC_TakeDamage(fallDamage, null);
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
        
        if (transform.position.y < -5f)
            Die();
    }

    [PunRPC]
    private void SyncProfile(string p_username, int p_level, int p_xp, int p_globalKills, int p_globalDeaths) //profil de chq player (username in game)
    {
        playerProfile = new ProfileData(p_username,p_level,p_xp,p_globalKills,p_globalDeaths);
        playerUsername.text = playerProfile.username;
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
    
    void ChangeLayersRecursively(Transform trans, string name)
    {
        foreach (Transform child in trans)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
            ChangeLayersRecursively(child, name);
        }
    }
    
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;
        
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        
        if (previousItemIndex != -1)
            items[previousItemIndex].itemGameObject.SetActive(false);

        previousItemIndex = itemIndex;

		if (PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            ChangeLayersRecursively(items[itemIndex].itemGameObject.transform, "Weapons");
		}
    }

    void UseItem() //utiliser les items (souvent gun)
    {
        int i;
        for (i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                items[itemIndex].isScoped = false;
                if (((GunInfo) items[itemIndex].itemInfo).canScope)
                {
                    items[itemIndex].UnScoped();
                }
                if (((GunInfo) items[itemIndex].itemInfo).canScopeOthers)
                {
                    items[itemIndex].SimpleUnScoped();
                }
                EquipItem(i);
                break;
            }
        }

        if (((GunInfo) items[itemIndex].itemInfo).isAutomatic)
        {
            if (Input.GetMouseButton(0))
                items[itemIndex].Use();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                items[itemIndex].Use();
        }
        
        if (Input.GetMouseButtonDown(1) && ((GunInfo) items[itemIndex].itemInfo).canScope)
        {
            items[itemIndex].Scope();
        }
        
        if (Input.GetMouseButtonDown(1) && ((GunInfo) items[itemIndex].itemInfo).canScopeOthers)
        {
            items[itemIndex].SimpleScope();
        }
        
    }
    #endregion
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PV.IsMine && targetPlayer == PV.Owner && changedProps.ContainsKey("itemIndex"))
		{
			EquipItem((int)changedProps["itemIndex"]);
		}

        if (!PV.IsMine)
            return;
        
        if (changedProps.ContainsKey("Death"))
        {
            //Debug.Log($"{targetPlayer.NickName} died {(int)changedProps["Death"]} times.");
            
            if (targetPlayer == PV.Owner)
            {
                Launcher.myProfile.globalDeath += 1;
                //text death
                ui_death.text = "DEATHS : " + Convert.ToString((int)changedProps["Death"]);
                
            }
        }

        if (changedProps.ContainsKey("Kills"))
        {
            //Debug.Log($"{targetPlayer.NickName} has {(int) changedProps["Kills"]} kills.");

            if (targetPlayer == PV.Owner)
            {
                Launcher.myProfile.globalKill += 1;
                //text kills
                ui_kills.text = "KILLS : " + Convert.ToString((int)changedProps["Kills"]);
            }
        }
    }

    public void TakeDamage(float damage, Player opponent)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, opponent);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, Player opponent)
    {
        if (!PV.IsMine)
            return;
        
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
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
            
            // UIManager.Instance.HitEffect(); // pour l'instant ne fait rien
            StartCoroutine(cam.GetComponent<CameraEffect>().ShakeCamera(0.03f));
        }
    }

    void ApplyKill(Player player)
    {
        Hashtable H = new Hashtable();
        int deathOfParent = Convert.ToInt32(player.CustomProperties["Kills"]);
        
        H.Add("Kills", deathOfParent + 1);
        player.SetCustomProperties(H);
    }

    void Die()
    {
        deathAudio.Play();
        playerManager.Die();
        items[itemIndex].UnScoped();
        items[itemIndex].SimpleUnScoped();
    }
}