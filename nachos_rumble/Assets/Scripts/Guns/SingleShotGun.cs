using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    private PhotonView PV;
    private bool canShoot;

    public AudioSource shootingSound;
    public Animator animator;
    private GameObject crossHair;

    public float AdsFOV;
    public float HipFOV;
    

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        shootingSound.Stop();
    }

    private void Start()
    {
        crossHair = GameObject.Find("Canvas/Crosshair");
    }

    private void Update()
    {
        if (!PV.IsMine || !isEquiped)
            return;

        if (Input.GetMouseButtonDown(0))
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
        canShoot = false;

        cam.fieldOfView = HipFOV;
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
            cam.fieldOfView = HipFOV;
        }

        IEnumerator ScopeCoroutine()
        {
            animator.SetBool("Scoped", isScoped);
            yield return new WaitForSeconds(.18f);
            crossHair.SetActive(false);
            cam.fieldOfView = AdsFOV; // mettre une animation pour que ca soit plus fluide?
        }
    #endregion

    #region ShootMethods
        IEnumerator Shoot()
        {
            if (!canShoot)
                yield break;

            shootingSound.Play();
            canShoot = false;
            
            Ray ray = GetRayCast();
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage, PV.Owner, ((GunInfo) itemInfo).itemIndex);
                if (!hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("Camera"))
                    PV.RPC("RPC_Shoot",RpcTarget.All, hit.point,hit.normal);
            }

            var timeToWait = 1 / ((GunInfo) itemInfo).shotsPerSeconds;
            yield return new WaitForSecondsRealtime(timeToWait);
            canShoot = true;
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