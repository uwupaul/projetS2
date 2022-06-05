using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable] public class Data
{
    public string username;
    public int globalKills;
    public int globalDeaths;

    public static PlayerData Instance;

    public Data() //profil par defaut
    {
        username = "";
        globalDeaths = 0;
        globalKills = 0;
    }

    public Data(string u, int k, int d)
    {
        username = u;
        globalDeaths = d;
        globalKills = k;
    }
}

public class PlayerData : MonoBehaviour
{
    public string username;
    public int globalKills;
    public int globalDeaths;

    public static PlayerData Instance;

    public PlayerData() //profil par defaut
    {
        username = "";
        globalDeaths = 0;
        globalKills = 0;
    }

    public PlayerData(string u, int k, int d)
    {
        username = u;
        globalDeaths = d;
        globalKills = k;
    }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    // TODO
    // - SaveProfile() qui enregistre son format JSON le ProfileData du joueur
    // - FindProfile() qui trouve un fichier existant au format JSON, le désérialize
    //      si ce fichier n'existe pas, elle créé un nouveau ProfileData()
    //
    // - Il faut que le username soit changé dès qu'on change l'inputField dans les menus (en même temps que le PhotonNickname)
    // - Quand une partie est finie, on update les stats du joueur

    public void SaveProfile()
    {
        string path = Application.persistentDataPath + "/profile.dt";

        if (File.Exists(path))
            File.Delete(path);


        FileStream file = File.Create(path);
        var bf = new BinaryFormatter();

        bf.Serialize(file, new Data(username, globalKills, globalDeaths));
        file.Close();

        Debug.Log("SUCCESS in SaveProfile()");

        /*
        try {
            string path = Application.persistentDataPath + "/profile.dt";

            if (File.Exists(path))
                File.Delete(path);

            FileStream file = File.Create(path);
            var bf = new BinaryFormatter();
            bf.Serialize(file, this);

            file.Close();
                
            Debug.Log("SAVED SUCCESSFULLY");
        }
        catch {
            Debug.Log("Error in 'SaveProfile()'");
        }
        */
    }
    public void LoadProfile()
    {
        try {
            string path = Application.persistentDataPath + "/profile.dt";

            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                var loadedDat = bf.Deserialize(file);
                Debug.Log("abababa");
                var loadedData = (Data) loadedDat;

                username = loadedData.username;
                globalKills = loadedData.globalKills;
                globalDeaths = loadedData.globalDeaths;
                
                Debug.Log(loadedData.username);
                Debug.Log(loadedData.globalDeaths);
                Debug.Log(loadedData.globalKills);
            }
                
            Debug.Log("SUCCESS in LoadProfile()");
        }
        catch {
            Debug.Log("Error in LoadProfile()");
        }
    }
}