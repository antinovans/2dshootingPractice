using System;
using UnityEngine;

public interface IStatusReceiver
{
    Func<bool> IsDeadChecker { get; set; }
    event Action<float> onEnterSpeedChangeByPercentage;
    event Action onExitSpeedChangeByPercentage;
    event Action<int> onSetSpeedByNumber;
    event Action onEnterStun;
    event Action onExitStun;
    event Action<int> onDamage;
    event Action<Vector3> onInstantRepel;

    void Initialize();
    void Reset();
    void OnEnterStun();
    void OnExitStun();
    void OnEnterSpeedChangeByPercentage(float speedMultiplier);
    void OnExitSpeedChangeByPercentage();
    void OnSetSpeedByNumber(int speed);
    void OnDamage(int damageAmount);
    void OnInstantRepel(Vector3 force);  //force是既包含方向又包含力的大小
    void CreateStatusFromDamageProfile(DamageProfile profile);
    void RegisterStatus(StatusBase status);
    void UpdateStatus();
    void RemoveStatus(StatusBase status);
}