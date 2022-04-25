using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    private PhotonView PV;
    private bool canShoot = true;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        StartCoroutine(Shoot());
    }
    

    IEnumerator Shoot()
    {
        if (!canShoot)
        {
            Debug.Log("canShoot == false");
            yield break;
        }

        Debug.Log("canShoot == true(?)");
        canShoot = false;
        //Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Ray ray = GetRayCast();
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("SingeShotGun : Shoot() : RayCast the object :" + hit.collider.gameObject);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage, PV.Owner);
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
            Debug.Log("RPC_Shoot : RayCast hit an object.");
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
                Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 11f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}