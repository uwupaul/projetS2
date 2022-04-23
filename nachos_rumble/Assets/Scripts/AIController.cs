using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class AIController : MonoBehaviour
{
    // Rôle du controller:
    //      - prendre des dégats et potentiellement mourir
    //      - donner des dégâts aux ennemis
    //      - se déplacer en cherchant l'ennemi (non-IA) le plus proche
    //      - 
    //      - 
    
    
    // Attention: ne pas prendre en compte les joueurs qui sont des IA!! -> créer des tags sur les gameObject?


    #region Health
        private Text textHealth;
        const float maxHealth = 100f;
        float currentHealth = maxHealth;
    #endregion
    
    Rigidbody rb;
    PhotonView PV;
    AIManager aiManager;
    public NavMeshAgent navMeshAgent;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        aiManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<AIManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            //EquipItem(0); // équiper l'item mais il doit être équipé de base non? à voir
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject); // sert à quoi exactement?
            Destroy(rb);
        }
    }

    private void FixedUpdate()
    {
        // - trouver le gameObject avec le tag 'Player' le plus proche
        // demander à la navmesh de s'y rendre

        if(!PV.IsMine)
            return;
        
        navMeshAgent.SetDestination(FindTarget());
    }

    Vector3 FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Single.PositiveInfinity;
        Vector3 bestTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // on set bestTarget comme la position même du Controller, comme ça si elle ne trouve pas de cible, elle ne bouge pas
        
        foreach (GameObject p in players)
        {
            Vector3 playerPos = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z);
            float newDistance = EuclidianDistance(playerPos);
            
            if (newDistance < minDistance) {
                minDistance = newDistance;
                bestTarget = playerPos;
            }
        }

        return bestTarget;
    }

    float EuclidianDistance(Vector3 p)
    {
        // Retourne la distance euclidienne entre l'AIController et 'p'.
        
        Vector3 diff = new Vector3(
            transform.position.x - p.x,
            transform.position.y - p.y,
            transform.position.z - p.z);

        return (float) Math.Sqrt(Math.Pow(diff.x, 2f) + Math.Pow(diff.y, 2f) + Math.Pow(diff.z, 2f));
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
        aiManager.Die();
    }
}
