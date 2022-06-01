using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ConnectionLog : MonoBehaviourPunCallbacks
{
    public GameObject connectionLogPrefab;

    public List<GameObject> itemList = new List<GameObject>();
    public Queue<GameObject> ItemsQueue = new Queue<GameObject>();
    
    public int maxItemNumber;
    public float timeAlive;
    public float fadeRate;
    
    
    /* Fonctionnement du log :
    
    - Dès qu'un joueur quitte ou rejoint la partie : on crée un prefab du texte qui s'affichera dans le log
    - On stock directement le prefab dans une queue,
    - si il y a de la place dans le log, alors on dequeue un item et on l'affiche (et on le rejoute dans la liste des
      items actuels) durant un temps donné puis on le Destroy en FadeOut (et on le retire de la liste)
    - sinon on supprime un par un les éléments du haut du layout (les premiers dans la liste des éléments)

    */

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("ConnectionLog : OnPlayerEnteredRoom");
        
        GameObject go = Instantiate(connectionLogPrefab, transform);
        go.SetActive(false);
        go.GetComponent<ConnectionLogItem>().SetUp(newPlayer, true);
        ItemsQueue.Enqueue(go);
        
        ManageQueue();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("ConnectionLog : OnPlayerLeftRoom");
        
        GameObject go = Instantiate(connectionLogPrefab, transform);
        go.SetActive(false);
        go.GetComponent<ConnectionLogItem>().SetUp(otherPlayer, false);
        ItemsQueue.Enqueue(go);
        
        ManageQueue();
    }

    void ManageQueue()
    {
        Debug.Log("ConnectionLog : ManageQueue");
        
        if (ItemsQueue.Count == 0)
            return;

        while (itemList.Count >= maxItemNumber)
        {
            // On doit retirer le premier item (celui du haut du layout)

            //Destroy(itemList[0]);
            StartCoroutine(FadeOutDestroy(itemList[0]));
            itemList.Remove(itemList[0]);
        }

        GameObject go = ItemsQueue.Dequeue();
        go.SetActive(true);
        itemList.Add(go);
        
        StartCoroutine(DisplayThenDestroy(go));
    }

    IEnumerator DisplayThenDestroy(GameObject go)
    {
        Debug.Log("ConnectionLog : DisplayThenDestroy");
        
        yield return new WaitForSecondsRealtime(timeAlive);
        
        StartCoroutine(FadeOutDestroy(go));
        itemList.Remove(go);
        
        ManageQueue();
    }
    
    IEnumerator FadeOutDestroy(GameObject go)
    {
        float targetAlpha = 0f;
        var image = go.GetComponentInChildren<TextMeshProUGUI>();
        Color curColor = image.color;
        while(Mathf.Abs(curColor.a - targetAlpha) > 0.05f)
        {
            Debug.Log(image.material.color.a);
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }
        
        Destroy(go);
    }
}