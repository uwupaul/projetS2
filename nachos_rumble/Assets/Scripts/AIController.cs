using System;
using System.Collections;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AIController : MonoBehaviourPunCallbacks, IDamageable
{
    private AIManager AIManager;
    private PhotonView PV;
    
    private Animator Animator;
    
    private int zVelHash;
    private int xVelHash;

    public NavMeshAgent agent;
    
    public Transform target;
    public bool searching;

    public int health;

    private KillFeed KillFeed;

    public bool canAttack;
    public float attackTime;
    public float attackRange;

    private void Awake()
    {
        KillFeed = GameObject.Find("Canvas/KillFeed").GetComponent<KillFeed>();
        PV = GetComponent<PhotonView>();
        AIManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<AIManager>();
        agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        zVelHash = Animator.StringToHash("Z_Velocity");
        xVelHash= Animator.StringToHash("X_Velocity");
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        SetAnimation();
        if (!searching)
            StartCoroutine(FindTarget());

        if (target != null && Vector3.Distance(transform.position, target.position) < attackRange)
            StartCoroutine(AttackPlayer());
    }

    IEnumerator FindTarget()
    {
        searching = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Single.MaxValue;
        target = enemies.Length == 0 ? gameObject.transform : enemies[0].transform;

        foreach (var enemy in enemies)
        {
            float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (enemyDistance < minDistance)
            {
                minDistance = enemyDistance;
                target = enemy.gameObject.transform;
            }
        }
        agent.SetDestination(target.position); 

        yield return new WaitForSecondsRealtime(0.03f);
        searching = false;
    }

    void SetAnimation() 
    {
        var vel = agent.velocity;
        var v = transform.InverseTransformDirection(vel);

        //Debug.Log($"X : {Animator.GetFloat("X_Velocity")}, Z : {Animator.GetFloat("Z_Velocity")}");
        
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
    }
    
    private IEnumerator AttackPlayer()
    {
        if (!canAttack)
            yield break;
        
        canAttack = false;
        agent.velocity = Vector3.Lerp(agent.velocity * 0.5f, Vector3.zero, Time.deltaTime);
        transform.LookAt(target);
        Animator.SetTrigger("Melee");
        
        yield return new WaitForSecondsRealtime(attackTime / 4);
        
        target.gameObject.GetComponent<IDamageable>().TakeDamage(25, "Angry Mexican", 3);

        yield return new WaitForSecondsRealtime(attackTime * 3/4);
        canAttack = true;
    }

    public void TakeDamage(float damage, Player opponent, int gunIndex)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.MasterClient, damage, opponent, gunIndex);
    }


    [PunRPC]
    public void RPC_TakeDamage(float damage, Player opponent, int gunIndex)
    {
        Animator.Play("Hit Reaction");
        agent.velocity = Vector3.zero;
        
        health -= (int) damage;

        if (health <= 0)
        {
            KillFeed.KillFeedEntry("Angry Mexican", opponent.NickName, gunIndex);
            ApplyKill(opponent);
            Die();
        }
    }
    
    void ApplyKill(Player player)
    {
        Debug.Log("AI Controller called Applykill on " + player.NickName);
        
        Hashtable H = new Hashtable();
        int deathOfParent = Convert.ToInt32(player.CustomProperties["K"]);
        
        H.Add("K", deathOfParent + 1);
        player.SetCustomProperties(H);
    }

    public void TakeDamage(float damage, string opponentName, int gunIndex)
    {
        throw new NotImplementedException();
        // pas utile dans cette classe
    }

    void Die()
    {
        AIManager.Die(this);
    }
}
