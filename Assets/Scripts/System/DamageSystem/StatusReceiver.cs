using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StatusReceiver : IStatusReceiver
{
    #region public
    public List<StatusBase>[] WorkingStatus;
    protected readonly int MaxStatusListSize = 5;
    private int activeStatusCount = 0;
    private MonoBehaviour parentMono;
    public Func<bool> IsDeadChecker { get; set; }
    public event Action<float> onEnterSpeedChangeByPercentage;
    public event Action onExitSpeedChangeByPercentage;
    public event Action<int> onSetSpeedByNumber;
    public event Action onEnterStun;
    public event Action onExitStun;
    public event Action<int> onDamage;
    public event Action<Vector3> onInstantRepel;
    public StatusReceiver(Func<bool> isDeadChecker, MonoBehaviour parentMono)
    {
        IsDeadChecker = isDeadChecker ?? throw new ArgumentNullException(nameof(isDeadChecker));
        this.parentMono = parentMono;
    }
    public virtual void CreateStatusFromDamageProfile(DamageProfile profile)
    {
        if(IsDeadChecker())
            return;
        //adding impulse status
        if (profile.ImpulseDamageAmount > 0)
            AddImpulseDamageStatusFromProfile(profile);
        //adding stun status
        if(profile.CanStun)
            AddStunStatusFromProfile(profile);
    }
    public virtual void RegisterStatus(StatusBase status)
    {
        //当前函数不能用于添加瞬间伤害的status
        activeStatusCount++;
        int idx = (int)status.StatusKind;
        if(WorkingStatus[idx].Count == 0)    //当前没有这个类型的status
            WorkingStatus[idx].Add(status);    //直接往后加
        else if (WorkingStatus[idx][0] == null)
        {
            WorkingStatus[idx][0] = status;
        }
        else    //当前有这个类型的status
        {
            WorkingStatus[idx][0].ResetDuration(status.Duration);
            return;
        }
        status.OnAdd();
    }
    

    public virtual void UpdateStatus()
    {
        //现在没有active的status
        if(activeStatusCount == 0)
            return;
        foreach (var statusList in WorkingStatus)
        {
            for (int i = 0; i < statusList.Count; i++)
            {
                if(IsDeadChecker())  //abort updating when enemy is dead
                    return;
                if(statusList[i] != null)
                    statusList[i].OnUpdate();
            }
        }
    }
    public virtual void RemoveStatus(StatusBase status)
    {
        activeStatusCount--;
        status.OnRemove();
        var statusList = WorkingStatus[(int)status.StatusKind];
        for (int i = 0; i < statusList.Count; i++)
        {
            if (statusList[i] == status)
            {
                statusList[i] = null;
                return;
            }
        }
        Debug.Log("error: Unable to remove status");
    }
    public void Initialize()
    {
        int arraySize = Enum.GetValues(typeof(StatusKind)).Length;
        WorkingStatus = new List<StatusBase>[arraySize];
        for (int i = 0; i < arraySize; i++)
        {
            WorkingStatus[i] = new List<StatusBase>();
        }
    }
    public void Reset()    //绑定物体回归对象池时重置状态池
    {
        foreach (var statusList in WorkingStatus)
        {
            for (int i = 0; i < statusList.Count; i++)
            {
                statusList[i] = null;
            }
        }
    }
    //控制状态机变化的method

    public virtual void OnEnterStun()
    {
        onEnterStun?.Invoke();
    }
    public virtual void OnExitStun()
    {
        onExitStun?.Invoke();
    }
    public virtual void OnEnterSpeedChangeByPercentage(float speedMultiplier)
    {
        onEnterSpeedChangeByPercentage?.Invoke(speedMultiplier);
    }
    public virtual void OnExitSpeedChangeByPercentage()
    {
        onExitSpeedChangeByPercentage?.Invoke();
    }
    //瞬间产生作用的方法，不影响状态机
    public virtual void OnSetSpeedByNumber(int speed)
    {
        onSetSpeedByNumber?.Invoke(speed);
    }
    public virtual void OnDamage(int damageAmount)
    {
        onDamage?.Invoke(damageAmount);
    }
    public virtual void OnInstantRepel(Vector3 force)   //force是既包含方向又包含力的大小
    {
        onInstantRepel?.Invoke(force);
    }
    #endregion

    #region protected helper functions
    protected virtual void AddImpulseDamageStatusFromProfile(DamageProfile profile)
    {
        //instant damage不进入status array，直接处理
        var instantDamageStatus = new StatusInstantDamage(this, 0, profile.ImpulseDamageAmount);
        instantDamageStatus.OnAdd();
    }
    protected virtual void AddStunStatusFromProfile(DamageProfile profile)
    {
        activeStatusCount++;
        int idxInArray = (int)StatusKind.Stun;
        int idxInList = GetIndexToAddFromStatusList(idxInArray, false);
        if (WorkingStatus[idxInArray][idxInList] != null) //index 0已经有status了
        {
            WorkingStatus[idxInArray][idxInList].ResetDuration(profile.StunDuration);
            return;
        }
        var statusStun = new StatusStun(this, profile.StunDuration,profile.RepelDirection * profile.RepelForceMagnitude);
        WorkingStatus[idxInArray][idxInList] = statusStun;
        statusStun.OnAdd();
    }
    protected int GetIndexToAddFromStatusList(int statusIndex, bool canStack)
    {
        var statusList = WorkingStatus[statusIndex]; 
        if (statusList.Count == 0)
        {
            statusList.Add(null);
            return 0;
        }

        if (!canStack)
            return 0;
        int i = 0;
        while (i < statusList.Count)
        {
            if (i >= MaxStatusListSize)
                return -1;  //status叠加达到上限
            if (statusList[i] == null)
            {
                return i;   //找到了空置的slot
            }
            i++;
        }
        //没有找到空置的slot,输出最后一个位置
        statusList.Add(null);
        return statusList.Count - 1;
    }

    #endregion
    
}