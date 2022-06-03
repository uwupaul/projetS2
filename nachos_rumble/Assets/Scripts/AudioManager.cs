using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AudioManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public static AudioManager Instance;
    public AudioSource AudioSource;
    public AudioClip[] clips;
    
    void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre AudioManaer exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        Instance = this;

        PV = GetComponent<PhotonView>();
        
        AudioSource.spatialBlend = 1f;
        AudioSource.spread = 0;
        AudioSource.spatialize = true;
    }

    public void SendSound(AudioSource sound, int soundIndex)
    {
        PV.RPC("RPC_PlaySound", RpcTarget.Others, soundIndex,
            sound.minDistance, sound.maxDistance, sound.transform.position);
        
        Debug.Log("RPC_SendSound called");
    }

    public void SendSound(int soundIndex, float minDistance, float maxDistance, Vector3 position)
    {
        PV.RPC("RPC_PlaySound", RpcTarget.Others, soundIndex,
            minDistance, maxDistance, position);
        
        Debug.Log("RPC_SendSound called");
    }

    [PunRPC]
    void RPC_PlaySound(int soundIndex, float minDistance, float maxDistance, Vector3 position)
    {
        AudioSource.clip = clips[soundIndex];
        
        AudioSource.minDistance = minDistance;
        AudioSource.maxDistance = maxDistance;
        AudioSource.gameObject.transform.position = position;
        AudioSource.Play();
        Debug.Log("sound played" + position);
    }
}
