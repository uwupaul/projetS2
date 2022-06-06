using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AutomaticGun : Gun
{
    [SerializeField] Camera cam;

    private PhotonView PV;
    private bool canShoot;

    public float pitchChange;

    public AudioSource shootingSound;
    public Animator animator;
    private GameObject crossHair;

    public AudioClip[] clips;
    
    public PlayerController PC;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        shootingSound.spatialBlend = 1f;
        shootingSound.spread = 0;
        shootingSound.spatialize = true;
    }

    private void Start()
    {
        if (!PV.IsMine)
            return;
        
        crossHair = GameObject.Find("Canvas/Crosshair");
    }
    
    private void Update()
    {
        if (!PV.IsMine || !isEquiped)
            return;

        if (Input.GetMouseButton(0))
            StartCoroutine(Shoot());

        if (Input.GetMouseButtonDown(1))
            ToggleScope();
    }

    public override void Equip()
    {
        isEquiped = true;
        itemGameObject.SetActive(true);
        canShoot = true;
        
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

        cam.fieldOfView = ((GunInfo) itemInfo).hipFOV;
        Unscope();
    }

    #region Scope
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
            crossHair.SetActive(true);
            cam.fieldOfView = ((GunInfo) itemInfo).hipFOV;;
            PC.sensMultiplier = 1;
        }

        IEnumerator ScopeCoroutine()
        {
            PC.sensMultiplier = ((GunInfo) itemInfo).scopeSensMultiplier;;
            animator.SetBool("Scoped", isScoped);
            yield return new WaitForSeconds(.18f);
            crossHair.SetActive(false);
            cam.fieldOfView = ((GunInfo) itemInfo).adsFOV;; // mettre une animation pour que ca soit plus fluide?
        }
    #endregion
    
    #region ShootMethods

        [PunRPC]
        void RPC_RemoteShoot(float pitch, int clipIndex)
        {
            shootingSound.pitch = pitch;
            shootingSound.PlayOneShot(clips[clipIndex]);
        }
    
        IEnumerator Shoot()
        {
            if (!canShoot)
                yield break;

            shootingSound.pitch = Random.Range(1 - pitchChange, 1 + pitchChange);
            int clipIndex = Random.Range(0, 1);
            shootingSound.PlayOneShot(clips[clipIndex]);
            PV.RPC("RPC_RemoteShoot", RpcTarget.Others, shootingSound.pitch, clipIndex);
            
            //AudioManager.Instance.SendSound(shootingSound, ((GunInfo) itemInfo).itemIndex);
            
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

        Ray GetRayCast()
        {
            float m = ComputePrecisionMultiplier(PC.Speed);
            float s = ((GunInfo) itemInfo).shotSpread / 1000 / m;
            Debug.Log($"m vaut {m}");
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
