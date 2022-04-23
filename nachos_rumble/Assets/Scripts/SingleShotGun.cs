using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }
    
    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("SingeShotGun : Shoot() : RayCast the object :" + hit.collider.gameObject);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage, PV.Owner);
            if (hit.collider.GetComponent<IDamageable>() is null)
            {
                Debug.Log("SingleShotGun : Raycast hit an object that is not damageable.");
            }
            else
            {
                Debug.Log("SingleShotGun : Raycast hit an object that is damageable.");   
            }
            PV.RPC("RPC_Shoot",RpcTarget.All, hit.point,hit.normal);
        }
    }
    
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            Debug.Log("RPC_Shoot : RayCast hit an object.");
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
                Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj,11f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        else
        {
            Debug.Log("RPC_Shoot : RayCast did not hit any object.");
        }
    }
}