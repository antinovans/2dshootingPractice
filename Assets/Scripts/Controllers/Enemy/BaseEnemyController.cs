using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Idle = 0,
    Move = 1,
    Stun = 2,
    Die = 3,
    Attack = 4
}
public abstract class BaseEnemyController : MonoBehaviour
{
    #region references
    [SerializeField] protected EnemyConfig config;
    protected Transform target;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator ac;

    protected AudioSource audioSource;
    /***    status handler    ***/
    protected IStatusReceiver statusReceiver;
    protected string[] animatorStates = { "IsHit", "IsDead", "IsIdle", "IsAttack", "IsMove" };
    #endregion

    #region variables
    protected bool isDisabled = false;
    protected bool isFacingRight;
    protected Vector3 originalScale;
    private LayerMask groundLayer;
    public EnemyState LastState { get; set; }
    public EnemyState CurrentState { get; set; }
    public float MoveSpeed { get; set; }
    public int curHp { get; set; }
    #endregion

    public EnemyType GetEnemyType()
    {
        return config.type;
    }

    #region unity life cycle related
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ac = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        originalScale = transform.localScale;
        curHp = config.hp;
        MoveSpeed = config.defaultSpeed;
        groundLayer = 1 << LayerMask.NameToLayer("Terrain");
        OnAwake();
    }
    protected virtual void OnAwake()
    {
        statusReceiver = new StatusReceiver(() => curHp <= 0, this);
        statusReceiver.Initialize();
        RegisterEventsToStatusReceiver();
    }
    private void OnEnable()=> OnReset();
    private void Start() => OnStart();

    protected virtual void OnStart()
    {
        LastState = EnemyState.Idle;
        CurrentState = EnemyState.Move;
        //temp
        target = GameManager.instance.target;
    }

    private void Update() => OnUpdate();

    protected virtual void OnUpdate()
    {
        CheckGrounded();
        if(isDisabled) 
            return;
        statusReceiver.UpdateStatus();
        UpdateAnimator();
    }
    protected bool isGrounded;
    protected float checkGroundRayLength;
    protected void CheckGrounded()
    {
        Vector2 rayStart = transform.position;
        Vector2 rayDirection = Vector2.down;
        float rayLength = checkGroundRayLength;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, groundLayer);
        Debug.DrawRay(rayStart, rayDirection * rayLength, hit.collider != null ? Color.green : Color.red);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void FixedUpdate() => OnFixedUpdate();
    protected virtual void OnFixedUpdate()
    {
        Navigate();
        if(isFacingRight)
            transform.localScale = originalScale; // 设定原始方向
        else
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // 翻转角色
    }

    #endregion
    
    protected virtual void Navigate()
    {
        if (CurrentState == EnemyState.Move)
        {
            var movingDir = new Vector2((target.position - transform.position).x, 0f).normalized;
            rb.velocity = movingDir * config.defaultSpeed;
            if (rb.velocity.x > 0.1f) // 向右移动
            {
                isFacingRight = true;
            }
            else if (rb.velocity.x < -0.1f) // 向左移动
            {
                isFacingRight = false;
            }
        }
    }
    protected virtual void UpdateAnimator()
    {
        // 先将所有状态设为false
        foreach (string state in animatorStates)
        {
            ac.SetBool(state, false);
        }

        // 然后根据当前状态设为true
        switch (CurrentState)
        {
            case EnemyState.Move:
                ac.SetBool("IsMove", true);
                break;
            case EnemyState.Stun:
                ac.SetBool("IsHit", true);
                break;
            case EnemyState.Idle:
                ac.SetBool("IsIdle", true);
                break;
            case EnemyState.Die:
                ac.SetBool("IsDead", true);
                break;
            case EnemyState.Attack:
                ac.SetBool("IsAttack", true);
                break;
            // 其他状态
        }
    }
    private void SetAnimatorToDie()
    {
        // 先将所有状态设为false
        foreach (string state in animatorStates)
        {
            ac.SetBool(state, false);
        }
        ac.SetBool("IsDead", true);
    }
    public virtual void OnReceiveDamageProfile(DamageProfile profile)
    {
        if(isDisabled)
            return;
        statusReceiver.CreateStatusFromDamageProfile(profile);
    }
    protected virtual void RegisterEventsToStatusReceiver()
    {
        // statusReceiver.onEnterSpeedChangeByPercentage += OnEnterSpeedChange;
        // statusReceiver.onExitSpeedChangeByPercentage += OnExitSlow;
        statusReceiver.onEnterStun += OnEnterStun;
        statusReceiver.onExitStun += OnExitStun;
        statusReceiver.onDamage += OnDamage;
        statusReceiver.onInstantRepel += OnRepel;
    }
    protected virtual void OnEnterStun()   //影响state
    {
        LastState = CurrentState;
        CurrentState = EnemyState.Stun;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    protected virtual void OnExitStun()    //影响state
    {
        appliedForce = Vector3.zero;    //重置受到的力为空
        LastState = CurrentState;
        CurrentState = EnemyState.Move;
        MoveSpeed = config.defaultSpeed;
    }

    protected virtual void OnAttack()
    {
        GameManager.instance.onEnemyAttack?.Invoke(config.attackDmg);
        audioSource.clip = config.onAttackClip;
        audioSource.Play();
        CurrentState = EnemyState.Move;
    }
    protected virtual void OnEnterSpeedChange(float speedMultiplier)
    {
        MoveSpeed = config.defaultSpeed * speedMultiplier;
    }
    protected virtual void OnExitSlow()
    {
        MoveSpeed = config.defaultSpeed;
    }
    protected virtual void OnDamage(int damageAmount)  //不影响state
    {
        curHp -= damageAmount;
        if (curHp <= 0)
        {
            OnDied();
        }
        else
        {
            //播放音效
            audioSource.clip = config.onHitclip;
            audioSource.Play();
        }
    }

    protected virtual void OnDied()
    {
        //disable collision
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        //invoker相关action
        GameManager.instance.onEnemyDie?.Invoke(config);
        //animator
        SetAnimatorToDie();
        //play dead sound
        audioSource.clip = config.onDieclip;
        audioSource.Play();
        //disable velocity
        rb.velocity = new Vector2(0, rb.velocity.y);
        //disable status update
        isDisabled = true;
        CurrentState = EnemyState.Die;
        //给一个击飞效果
        Vector2 dir = appliedForce.normalized;
        float force = appliedForce.magnitude > 5f ? appliedForce.magnitude : 5f;
        rb.AddForce(dir * force,ForceMode2D.Impulse);
    }

    protected Vector3 appliedForce; //用来记录受到的力
    public virtual void OnRepel(Vector3 force) //不影响state
    {
        appliedForce = force;
        this.rb.AddForce(force, ForceMode2D.Impulse);
    }

    #region object pool related
    public void OnSet(Vector2 startPos, Quaternion rotation)
    {
        transform.position = startPos;
        transform.rotation = rotation;
    }
    protected virtual void OnReset()
    {
        //清空状态池
        statusReceiver.Reset();
        isDisabled = false;
        appliedForce = Vector3.zero;
        LastState = EnemyState.Idle;
        CurrentState = EnemyState.Move;
        curHp = config.hp;
        MoveSpeed = config.defaultSpeed;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    #endregion
    


    //怪物死亡落地产生尸体相关
    protected virtual void OnDiedAnimationComplete()
    {
        StartCoroutine(OnDiedComplete());
    }
    protected IEnumerator OnDiedComplete()
    {
        while (!isGrounded)
        {
            yield return null;  //wait for the next frame
        }
        rb.velocity =Vector2.zero;
        gameObject.SetActive(false);
        var corpse = Instantiate(config.deadEnemyObj,  transform.position, transform.rotation, GameManager.instance.corpseTransform);
        corpse.transform.localScale = transform.localScale;
        EnemyManager.instance.Release(this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Base") && 
            (CurrentState == EnemyState.Move || CurrentState == EnemyState.Idle))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            LastState = CurrentState;
            CurrentState = EnemyState.Attack;
        }
            
    }
}