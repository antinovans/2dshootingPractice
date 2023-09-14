using UnityEngine;

[System.Serializable]
public class DamageProfile
{
    public int ImpulseDamageAmount;
    public bool CanStun;
    public float StunDuration;
    public bool CanRepel;
    public float RepelForceMagnitude;
    public Vector3 RepelDirection;
    public void CopyValue(DamageProfile other)
    {
        ImpulseDamageAmount = other.ImpulseDamageAmount;
        CanStun = other.CanStun;
        StunDuration = other.StunDuration;
        CanRepel = other.CanRepel;
        RepelForceMagnitude = other.RepelForceMagnitude;
        RepelDirection = other.RepelDirection;
    }
    public DamageProfile SetImpulseDamageAmount(int impulseDamageAmount)
    {
        ImpulseDamageAmount = impulseDamageAmount;
        return this;    
    }
    public DamageProfile SetCanStun(bool canStun)
    {
        this.CanStun = canStun;
        return this;
    }
    public DamageProfile SetStunDuration(float stunDuration)
    {
        this.StunDuration = stunDuration;
        return this;
    }
    public DamageProfile SetCanRepel(bool canRepel)
    {
        this.CanRepel = canRepel;
        return this;
    }
    public DamageProfile SetRepelForceMagnitude(float repelForceMagnitude)
    {
        this.RepelForceMagnitude = repelForceMagnitude;
        return this;
    }
    public DamageProfile SetRepelDirection(Vector3 repelDirection)
    {
        this.RepelDirection = repelDirection;
        return this;
    }
}