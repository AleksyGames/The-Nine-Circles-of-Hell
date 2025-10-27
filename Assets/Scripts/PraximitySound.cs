using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Transform player;

    [Header("Settings")]
    public float activationDistance = 10f;
    public float fadeSpeed = 2f;
    public float minDelay = 2f;
    public float maxDelay = 5f;

    private Coroutine soundCoroutine;
    private float targetVolume = 0f;

    void Start()
    {

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        if (audioSource != null)
        {
            audioSource.volume = 0f;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (audioSource == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float distanceRatio = Mathf.Clamp01(1f - (distance / activationDistance));
        targetVolume = distanceRatio;

        // Jeœli gracz jest w zasiêgu i coroutine nie dzia³a — uruchom
        if (distance <= activationDistance && soundCoroutine == null)
        {
            soundCoroutine = StartCoroutine(SoundLoop());
        }

        // Jeœli gracz wyszed³ z zasiêgu — zakoñcz dŸwiêk
        if (distance > activationDistance && soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            soundCoroutine = null;
            StartCoroutine(FadeOutAndStop());
        }
    }

    private IEnumerator SoundLoop()
    {
        while (Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            audioSource.Play();

            // Fade-in
            while (audioSource.volume < targetVolume && audioSource.isPlaying)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
                yield return null;
            }

            // Poczekaj, a¿ dŸwiêk siê skoñczy
            yield return new WaitWhile(() => audioSource.isPlaying);

            // Losowa przerwa zanim zagra ponownie
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }

        soundCoroutine = null;
    }

    private IEnumerator FadeOutAndStop()
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        audioSource.Stop();
    }
}
