using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    private PhotonView PV;
    private bool canShoot;
    
    #region reload system variable

        public float reloadTime;
        public int allBullet;
        public int bullet;
        private int initBullet;
        private bool isRealoading = false;
        private TextMeshProUGUI ui_bullet;

    #endregion

    public AudioSource shootingSound;
    public AudioSource reloadSound;
    public Animator animator;
    public GameObject forAnimation;

    private void Start()
    {
        initBullet = bullet;
        
        Debug.Log("start deagle");
        
        ui_bullet = GameObject.Find("Canvas/NumberOfBullet").GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
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

        if (isRealoading)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (bullet <= 0)
            {
                if (isRealoading == false && allBullet != 0)
                {
                    isRealoading = true;
                    StartCoroutine(Reload());
                }
            }
            else
                StartCoroutine(Shoot());
        }
    }

    public override void Equip()
    {
        ui_bullet = GameObject.Find("Canvas/NumberOfBullet").GetComponent<TextMeshProUGUI>();
        
        isEquiped = true;
        itemGameObject.SetActive(true);
        canShoot = true;
        
        Debug.Log("équipe deagle");
        ui_bullet.text = bullet + " / " + allBullet;
        
        if (PV.IsMine)
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
    }

    #region ShootMethods
        IEnumerator Shoot()
        {
            if (!canShoot)
                yield break;
            
            forAnimation.GetComponent<Animator>().Play("Shooting");
            
            bullet -= 1;
            ui_bullet.text = bullet + " / " + allBullet;

            shootingSound.Play();
            AudioManager.Instance.SendSound(shootingSound, ((GunInfo) itemInfo).itemIndex);
            
            canShoot = false;
            
            Ray ray = GetRayCast();
            ray.origin = cam.transform.position;
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
            reloadSound.Play();
            
            animator.SetBool("Reloading", true);
        
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
            
            animator.SetBool("Reloading", false);
        }

        Ray GetRayCast()
        {
            float s = ((GunInfo) itemInfo).shotSpread / 1000;
            Ray ray = cam.ViewportPointToRay(new Vector3(Random.Range(0.5f - s, 0.5f + s), Random.Range(0.5f - s, 0.5f + s)));
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
}