using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SniperGun : Gun
{
    private PhotonView PV;
    private bool canShoot;

    public AudioSource shootingSound;
    public AudioSource reloadSound;
    public Animator animator;

    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCam;
    
    public float AdsFOV;
    public float HipFOV;
    
    #region reload system variable
    
        public float reloadTime;
        public int allBullet;
        public int bullet;
        private int initBullet;
        private bool isRealoading = false;
        private TextMeshProUGUI ui_bullet;
        
    #endregion

    #region UI gameObjects
    
        private GameObject textHealth;
        private GameObject ui_username;
        private GameObject ui_kills;
        private GameObject ui_death;
        private GameObject HealthBar;
        private GameObject crossHair;
        public GameObject sniperICon;
        
    #endregion
    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PV.IsMine)
            return;
        
        initBullet = bullet;
        
        ui_username = GameObject.Find("Canvas/BottomLeft/UsernameText");
        ui_kills = GameObject.Find("Canvas/TopLeft/KillsText");
        ui_death = GameObject.Find("Canvas/TopLeft/DeathText");
        textHealth = GameObject.Find("Canvas/BottomLeft/TextHealth");
        HealthBar = GameObject.Find("Canvas/BottomLeft/HealthBar");
        crossHair = GameObject.Find("Canvas/Crosshair");
        
        
        ui_bullet = GameObject.Find("Canvas/NumberOfBullet").GetComponent<TextMeshProUGUI>();
        
        /*
        HealthBar.SetActive(true);
        ui_username.SetActive(true);
        ui_kills.SetActive(true);
        ui_death.SetActive(true);
        textHealth.SetActive(true);
        crossHair.SetActive(true);
        */
    }
    
    private void Update()
    {
        if (!PV.IsMine || !isEquiped)
            return;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRealoading && allBullet != 0 && bullet != initBullet)
            {
                isRealoading = true;
                StartCoroutine(Reload());
            }
        }

        if (isRealoading) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (bullet <= 0)
            {
                Debug.Log("is reload : " + isRealoading);
                if (isRealoading == false && allBullet != 0)
                {
                    isRealoading = true;
                    StartCoroutine(Reload());
                }
            }
            else
                StartCoroutine(Shoot());
        }

        if (Input.GetMouseButtonDown(1))
            ToggleScope();
    }

    public override void Equip()
    {
        isEquiped = true;
        itemGameObject.SetActive(true);
        canShoot = true;
        
        ui_bullet.text = bullet + " / " + allBullet;
        
        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", ((GunInfo) itemInfo).itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            ChangeLayersRecursively(itemGameObject.transform, "Weapons");
        }
    }

    public override void Unequip()
    {
        isEquiped = false;
        itemGameObject.SetActive(false);
        Unscope();
    }

    #region ShootMethods
        IEnumerator Shoot()
        {
            if (!canShoot)
                yield break;
            
            bullet -= 1;
            ui_bullet.text = bullet + " / " + allBullet;

            shootingSound.Play();
            AudioManager.Instance.SendSound(shootingSound, ((GunInfo) itemInfo).itemIndex);
            
            canShoot = false;
            
            Ray ray = GetRayCast();
            ray.origin = mainCam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit)) 
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage, PV.Owner, ((GunInfo) itemInfo).itemIndex);
                if (!hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("Camera"))
                    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }

            var timeToWait = 1 / ((GunInfo) itemInfo).shotsPerSeconds;
            yield return new WaitForSecondsRealtime(timeToWait);
            canShoot = true;
        }
        
        IEnumerator Reload()
        {
            canShoot = false;
            Unscope();
            reloadSound.Play();
            
            yield return new WaitForSeconds(reloadTime);
            
            if (!isEquiped)
            {
                canShoot = true;
                isRealoading = false;
                yield break;
            }
            
            int b = initBullet - bullet;
            if (allBullet - b < 0)
            {
                b = allBullet;
            }
            allBullet -= b;
            bullet += b;
        
            ui_bullet.text = bullet + " / " + allBullet;
            
            canShoot = true;
            isRealoading = false;
        }

        Ray GetRayCast()
        {
            float s = ((GunInfo) itemInfo).shotSpread / 1000;
            Ray ray = mainCam.ViewportPointToRay(new Vector3(Random.Range(0.5f - s, 0.5f + s), Random.Range(0.5f - s, 0.5f + s)));
            return ray;
        }

        [PunRPC]
        void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
        {
            Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
            if (colliders.Length != 0)
            {
                GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
                    Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                Destroy(bulletImpactObj, 11f);
                bulletImpactObj.transform.SetParent(colliders[0].transform);
            }
        }
    #endregion

    #region ScopeMethods

    public void ToggleScope()
    {
        isScoped = !isScoped;

        if (isScoped)
            StartCoroutine(ScopeCoroutine());
        else
            Unscope();
    }

    public void Unscope()
    {
        animator.SetBool("Scoped", false);
        
        scopeOverlay.SetActive(false);
        
        weaponCamera.SetActive(true);
        HealthBar.SetActive(true);
        ui_username.SetActive(true);
        ui_kills.SetActive(true);
        ui_death.SetActive(true);
        textHealth.SetActive(true);
        crossHair.SetActive(true);
        sniperICon.SetActive(true);
        
        mainCam.fieldOfView = HipFOV;
    }

    IEnumerator ScopeCoroutine()
    {
        animator.SetBool("Scoped", true);
        
        yield return new WaitForSeconds(.18f);

        scopeOverlay.SetActive(true);
        
        weaponCamera.SetActive(false);
        HealthBar.SetActive(false);
        ui_username.SetActive(false);
        ui_kills.SetActive(false);
        ui_death.SetActive(false);
        textHealth.SetActive(false);
        crossHair.SetActive(false);
        sniperICon.SetActive(false);
        
        mainCam.fieldOfView = AdsFOV;
    }
    #endregion
}