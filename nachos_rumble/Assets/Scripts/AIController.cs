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

public class AIController : MonoBehaviourPunCallbacks, IDamageable
{
    // Rôle du controller:
    //      - prendre des dégats et potentiellement mourir
    //      - donner des dégâts aux ennemis
    //      - se déplacer en cherchant l'ennemi (non-IA) le plus proche
    
    // Problèmes à régler:
    //      - l'IA se met à bugger dès qu'un joueur la touche -> pourquoi??
    //      - l'IA ne peut pas monter les pentes (on dirait il faut essayer sur un autre map) -> pourquoi ??
    //      - l'IA n'est pas détectée comme un gameobject -> on ne peut pas lui tirer dessus ? je comprends pas pourquoi


    #region Health
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
            Destroy(rb);
        }
    }

    private void FixedUpdate()
    {
        // - trouver le gameObject avec le tag 'Player' le plus proche
        // demander à la navmesh de s'y rendre
        
        // FIX: set la destination du navMeshAgent dans une coroutine toute les 0.1 ou 0.2 secondes mais pas ici
        
        if(!PV.IsMine)
            return;
        
        navMeshAgent.SetDestination(FindTarget());
    }

    Vector3 FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Single.PositiveInfinity;
        Vector3 bestTarget = transform.position;
        // on set bestTarget comme la position même du Controller, comme ça si elle ne trouve pas de cible, elle ne bouge pas
        
        foreach (GameObject p in players)
        {
            float newDistance = Vector3.Distance(transform.position, p.transform.position);
            // changer ça pour pas faire qu'une ia s'arrête sans rien faire quand un joueur est au dessus d'elle 
            
            if (newDistance < minDistance) {
                minDistance = newDistance;
                bestTarget = p.transform.position;
            }
        }

        return bestTarget;
    }

    public void TakeDamage(float damage, Player opponent, int gunIndex)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, opponent);
        Debug.Log($"AI took {damage} damage.");
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
        Debug.Log("AIController : Die()");
        aiManager.Die();
    }
}
