using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect Instance;
    bool AlreadyShaking = false;

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
        Vector3 startPosition =  gameObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            gameObject.transform.position = startPosition + Random.insideUnitSphere * 0.03f;
            for (int i = 0; i < 4; i++)
                yield return null;
        }

        gameObject.transform.position = startPosition;
        AlreadyShaking = false;
    }
}
