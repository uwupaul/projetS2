using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    public TMP_Text murdererText;
    public TMP_Text victimText;
    public GameObject ShortBackground;
    public GameObject LongBackground;

    public GameObject[] weaponIcons;
    private int _gunIndex;
    
    public float fadeRate;
    
    public void SetUp(Player victim, Player murderer, int gunIndex)
    {
        victimText.text = victim.NickName;
        murdererText.text = murderer is null ? "" : murderer.NickName;
        _gunIndex = gunIndex;
        
        if (gunIndex == -1)
        {
            // mettre l'icon de chute
            weaponIcons[weaponIcons.Length-1].SetActive(true);
            ShortBackground.SetActive(true);
        }
        else {
            LongBackground.SetActive(true);
            weaponIcons[gunIndex].SetActive(true);
        }
    }

    public void FadeOutDestroy() =>
        StartCoroutine(ItemDestroyCoroutine());
    

    IEnumerator ItemDestroyCoroutine()
    {
        StartCoroutine(FadeOutDestroyCoroutine(murdererText));
        StartCoroutine(FadeOutDestroyCoroutine(victimText));
        StartCoroutine(FadeOutDestroyCoroutine(ShortBackground.GetComponent<Image>()));
        StartCoroutine(FadeOutDestroyCoroutine(LongBackground.GetComponent<Image>()));
        Debug.Log(_gunIndex);
        yield return StartCoroutine(FadeOutDestroyCoroutine(weaponIcons[_gunIndex == -1 ? weaponIcons.Length-1 : _gunIndex].GetComponent<Image>()));
        Destroy(gameObject);
    }

    IEnumerator FadeOutDestroyCoroutine(TMP_Text image)
    {
        float targetAlpha = 0f;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.05f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }

        Destroy(image.gameObject);
    }
    
    IEnumerator FadeOutDestroyCoroutine(Image image)
    {
        float targetAlpha = 0f;
        Color curColor = image.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.05f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            yield return null;
        }

        Destroy(image.gameObject);
    }
}