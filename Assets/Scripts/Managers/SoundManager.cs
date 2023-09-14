using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class SoundClip
{
    public ClipType type;
    public AudioClip clip;
}

public enum ClipType
{
    OnFire = 0,
    OnHit = 1,
    OnExplosion = 2
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]private List<AudioSource> audioSources;
    private AudioSource hitAudioSource;
    public float defaultPitch = 1.0f;
    public float minPitch = 0.9f; 
    public float maxPitch = 1.1f;
    private IEnumerator resetPitchCor;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundClip clip)
    {
        if (clip != null)
        {
            audioSources[(int)clip.type].PlayOneShot(clip.clip);
        }
    }
    public void PlayClipWithRandomPitch(SoundClip clip)
    {
        int idx = (int)clip.type;
        audioSources[idx].pitch = Random.Range(minPitch, maxPitch);
        audioSources[idx].clip = clip.clip;
        audioSources[idx].Play();
        StartCoroutine(ResetPitchAfterPlaying(clip.clip.length, idx));
    }

    private IEnumerator ResetPitchAfterPlaying(float clipDuration, int idx)
    {
        yield return new WaitForSeconds(clipDuration);
        audioSources[idx].pitch = defaultPitch;
    }


}