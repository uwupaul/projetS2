using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windowmanager : MonoBehaviour
{
    public GameObject ScoreBoard;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ScoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ScoreBoard.SetActive(false);
        }
    }
}
