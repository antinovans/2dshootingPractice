using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VFXController : MonoBehaviour
{
    public VFXType vfxType;
    public SoundClip audioClip;
    private ParticleSystem particleSystem;
    protected virtual void Init()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    
    protected virtual void OnUpdate()
    {
        if (!particleSystem.isPlaying)
        {
            VFXManager.instance.Release(this);
        }
    }

    public void Set(Vector2 startPos)
    {
        transform.position = startPos;
    }
    protected virtual void OnReset()
    {
        particleSystem.Stop();
        particleSystem.Clear();
    }
    public virtual void Play()
    {
        //sound处理
        if (audioClip.clip != null)
            SoundManager.Instance.PlayClipWithRandomPitch(audioClip);
        particleSystem.Play();
    }

    private void Awake() => Init();
    private void OnDisable() => OnReset();
    private void Update() => OnUpdate();

}