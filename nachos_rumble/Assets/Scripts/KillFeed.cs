using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class KillFeed : MonoBehaviourPunCallbacks
{
    public GameObject killFeedPrefab;

    private List<GameObject> itemList = new List<GameObject>();
    private Queue<GameObject> ItemsQueue = new Queue<GameObject>();

    public int maxItemNumber;
    public float timeAlive;

    private PhotonView PV;
    
    /* Fonctionnement du KillFeed :
     
     - dans le player lorsque quelqu'un meurt à la suite de TakeDamage(), lance une méthode photon RPC qui doit s'éxécuter ici
     (on doit rajouter à TakeDamage un paramètre pour savoir quelle arme a été utilisée par le joueur qui a donné les dégats)
     
     - cette méthode va instantier un prefab puis le set up avec le nom des joueurs, l'arme utilisée, etc
     - on stock directement le prefab dans une queue
     - si il y a de la place dans le feed, alors on dequeue un item et on l'affiche (et on le rajoute dans la liste des 
       items actuels) durant un temps donné puis on le Destroy en FadeOut (et on le retire de la liste)
     - sinon on supprime un par un les éléments du haut du layout (les premiers dans la liste des éléments)    
    */

    /* TODO
     
     - rajouter l'icon des maracas quand on les aura mises
    */

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void KillFeedEntry(Player victim, Player murderer, int gunIndex)
    {
        Debug.Log("KillFeedItem : KillFeedEntry");
        
        PV.RPC("RPC_KillFeedEntry", RpcTarget.All, victim, murderer, gunIndex);
    }

    public void KillFeedEntry(string victimName, string murdererName, int gunIndex)
    {
        PV.RPC("RPC_KillFeedEntryName", RpcTarget.All, victimName, murdererName, gunIndex);
    }

    [PunRPC]
    void RPC_KillFeedEntryName(string victimName, string murdererName, int gunIndex)
    {
        Debug.Log("KillFeedItem : RPC_KillFeedEntry");
        
        GameObject go = Instantiate(killFeedPrefab, transform);
        go.SetActive(false);
        go.GetComponent<KillFeedItem>().SetUp(victimName, murdererName, gunIndex);
        ItemsQueue.Enqueue(go);
        
        ManageQueue();
    }

    
    [PunRPC]
    void RPC_KillFeedEntry(Player victim, Player murderer, int gunIndex)
    {
        Debug.Log("KillFeedItem : RPC_KillFeedEntry");
        
        GameObject go = Instantiate(killFeedPrefab, transform);
        go.SetActive(false);
        go.GetComponent<KillFeedItem>().SetUp(victim, murderer, gunIndex);
        ItemsQueue.Enqueue(go);
        
        ManageQueue();
    }
    
    void ManageQueue()
    {
        Debug.Log("KillFeed : ManageQueue");
        
        if (ItemsQueue.Count == 0)
            return;

        while (itemList.Count >= maxItemNumber)
        {
            // On doit retirer le premier item (celui du haut du layout)

            itemList[0].GetComponent<KillFeedItem>().FadeOutDestroy();
            itemList.Remove(itemList[0]);
        }

        GameObject go = ItemsQueue.Dequeue();
        go.SetActive(true);
        itemList.Add(go);
        
        StartCoroutine(DisplayThenDestroy(go));
    }

    IEnumerator DisplayThenDestroy(GameObject go)
    {
        Debug.Log("KillFeedItem : DisplayThenDestroy");
        
        yield return new WaitForSecondsRealtime(timeAlive);

        if (go)
            go.GetComponent<KillFeedItem>().FadeOutDestroy();
            // pour pas avoir d'erreur si l'objet a déjà été destroy
        
        itemList.Remove(go);
        
        ManageQueue();
    }
}
