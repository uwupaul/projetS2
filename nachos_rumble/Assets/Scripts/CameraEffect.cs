using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect Instance;
    bool AlreadyShaking;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShakeCamera(float duration)
    {
        if (AlreadyShaking)
            yield break;

        AlreadyShaking = true;
        Debug.Log("ShakeCamera Coroutine started");
        
        Vector3 startPosition =  gameObject.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            gameObject.transform.localPosition = startPosition + Random.insideUnitSphere * 0.03f;
            for (int i = 0; i < 4; i++)
                yield return null;
        }

        gameObject.transform.localPosition = startPosition;
        AlreadyShaking = false;
        
        Debug.Log("ShakeCamera Coroutine ended");
    }
}
