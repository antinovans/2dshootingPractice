using UnityEngine;

public class BulletModel
{
    public float maxSpeed;
    public float minSpeed;
    public AnimationCurve speedCurve;
    public float pushBackForce;
    public float pauseTime;
    public float existingDuration;
    public DamageProfile dmgProfile;
    public Sprite bulletSprite;
    public GameObject trail;
    public SoundClip fireSoundClip;
    public SoundClip hitClip;

    public BulletModel()
    {
        
    }
    public BulletModel(BulletModel newModel)
    {
        this.maxSpeed = newModel.maxSpeed;
        this.minSpeed = newModel.minSpeed;
        this.speedCurve = newModel.speedCurve;
        this.pushBackForce = newModel.pushBackForce;
        this.pauseTime = newModel.pauseTime;
        this.existingDuration = newModel.existingDuration;
        this.dmgProfile = newModel.dmgProfile;
        this.bulletSprite = newModel.bulletSprite;
        this.trail = newModel.trail;
        fireSoundClip = newModel.fireSoundClip;
        hitClip = newModel.hitClip;
    }

    public BulletModel(BulletConfig config)
    {
        this.maxSpeed = config.maxSpeed;
        this.minSpeed = config.minSpeed;
        this.speedCurve = config.speedCurve;
        this.pushBackForce = config.pushBackForce;
        this.pauseTime = config.pauseTime;
        this.existingDuration = config.existingDuration;
        this.dmgProfile = new DamageProfile();
        dmgProfile.CopyValue(config.dmgProfile); 
        this.bulletSprite = config.bulletSprite;
        this.trail = config.trail;
        fireSoundClip = config.fireSoundClip;
    }
}