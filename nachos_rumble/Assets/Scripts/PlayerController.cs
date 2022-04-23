using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    #region Items
        [SerializeField] Item[] items;
        int itemIndex;
        int previousItemIndex = -1;
    #endregion

    #region Physics
        [SerializeField] GameObject cameraHolder;
        [SerializeField] float sprintSpeed, walkSpeed, jumpForce, smoothTime;
        public float mouseSensitivity;
        float verticalLookRotation;
        bool grounded;
        Vector3 smoothMoveVelocity;
        private Vector3 moveAmount;
    #endregion
    
    #region Health
        private Text textHealth;
        const float maxHealth = 100f;
        float currentHealth = maxHealth;
    #endregion

    Rigidbody rb;
    PhotonView PV;
    PlayerManager playerManager;
    GameManager _gameManager;

    private Text ui_username;

    [HideInInspector] public ProfileData playerProfile;
    public TextMeshPro playerUsername;

    public bool EscapeMod => _gameManager.EscapeMod;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            ui_username = GameObject.Find("Canvas/Username/UsernameText").GetComponent<Text>();

            ui_username.text = Launcher.myProfile.username;
            
            photonView.RPC("SyncProfile",RpcTarget.All,Launcher.myProfile.username,Launcher.myProfile.level,Launcher.myProfile.xp);

            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        
        textHealth = GameObject.Find("TextHealth").GetComponent<Text>();
        
        if (!EscapeMod)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if(!PV.IsMine)
            return;
        
        textHealth.text = currentHealth + "";
        
        if (EscapeMod)
        {
            moveAmount = Vector3.SmoothDamp(moveAmount,
                new Vector3(0,0,0) * 0, ref smoothMoveVelocity, smoothTime);
        }
        else
        {
            Look(); Move(); Jump(); UseItem();
        }
        
        if (transform.position.y < -10f)
            Die();
    }
    
    private void FixedUpdate()
    {
        if(!PV.IsMine)
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
    
    [PunRPC]
    private void SyncProfile(string p_username, int p_level,int p_xp) //profile de chq player (username in game)
    {
        playerProfile = new ProfileData(p_username,p_level,p_xp);
        playerUsername.text = playerProfile.username;
    }

    void UseItem() //utiliser les items (souvent gun)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetMouseButtonDown(0))
            items[itemIndex].Use();
    }

    #region Physics Method
    
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    #endregion
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

		if (PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
    }

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
            return;
        
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        playerManager.Die();
    }

}
