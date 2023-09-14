using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Run,
    Shoot,
    Jump,
    Die
}
public class CharacterController : MonoBehaviour
{
    #region public fields
    public float shootingInterval = 0.2f;
    public float movementSpeed = 5f;


    #endregion

    #region private fields

    private Animator characterAC;
    private Rigidbody2D characterRb;
    private CharacterInput inputScheme = null;
    private GunController gunController;
    private AttackModel attackModel;

    private PlayerState curState = PlayerState.Idle;
    //movement
    private Vector2 movementDir = Vector2.zero;
    private Vector3 originalScale;

    private bool isNegateAnimator = false;
    //jumping 
    public float jumpForce;
    private LayerMask groundLayer;
    private bool isGrounded;
    //shooting
    private bool isShooting = false;
    private bool isShootingCd = false;
    private bool isFacingRight = true;
    private Transform gunTransform;
    #endregion

    #region mono innate methods
    private void Awake()
    {
        characterAC = GetComponent<Animator>();
        characterRb = GetComponent<Rigidbody2D>();
        inputScheme = new CharacterInput();
        gunTransform = transform.GetChild(0);
        gunController = new GunController(gunTransform);
        originalScale = transform.localScale;
        groundLayer = 1 << LayerMask.NameToLayer("Terrain");
        RegisterControllerActions();

    }

    // Start is called before the first frame update
    void Start()
    {
        attackModel = GameManager.instance.attackModel;
        GameManager.instance.player = gameObject;
        GameManager.instance.onBulletFire += Recoil;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        UpdateAnimator();
        UpdatePlayerFacingDir();
        Shoot();
        gunController.OnUpdate();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        inputScheme.Enable();
        
    }

    private void OnDisable()
    {
        inputScheme.Disable();
    }

    #endregion

    #region state related method
    private void UpdatePlayerFacingDir()
    {
        if (isShooting)
            return;
        if (movementDir.x > 0.1f) // 向右移动
        {
            transform.localScale = originalScale; // 设定原始方向
            isFacingRight = true;
        }
        else if (movementDir.x < -0.1f) // 向左移动
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // 翻转角色
            isFacingRight = false;
        }

        GameManager.instance.playerIsFacingRight = isFacingRight;
    }
    private void UpdateAnimator()
    {
        if (isGrounded)
        {
            if (isNegateAnimator)
                return;
            characterAC.SetBool("isJump", false);
            if (Mathf.Abs(characterRb.velocity.x) > 0.1f)
            {
                characterAC.SetBool("isIdle", false);
                characterAC.SetBool("isRun", true);
            }
            else
            {
                characterAC.SetBool("isRun", false);
                characterAC.SetBool("isIdle", true);
            }
        }
        else
        {
            characterAC.SetBool("isJump", true);
            characterAC.SetBool("isIdle", false);
            characterAC.SetBool("isRun", false);
        }
    }

    private void RegisterControllerActions()
    {
        var shootAction = inputScheme.Player.Shoot;
        shootAction.started += e => OnShootStarted();
        shootAction.canceled += e => OnShootCancelled();
        var jumpAction = inputScheme.Player.Jump;
        jumpAction.started += e=>Jump();
        var movementAction = inputScheme.Player.Movement;
        movementAction.performed += OnMovementPerformed;
        movementAction.canceled += OnMovementCanceled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        var newDir = value.ReadValue<Vector2>();
        if(movementDir == newDir)
            return;
        if(curState != PlayerState.Jump)
            curState = PlayerState.Run;
        movementDir = newDir;
        
    }

    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        if(curState != PlayerState.Jump)
            curState = PlayerState.Idle;
        movementDir = Vector2.zero;
        characterRb.velocity = new Vector2(0, characterRb.velocity.y);
    }

    private void Move()
    {
        if(movementDir.magnitude > 0.1f)
            characterRb.velocity = new Vector2(movementDir.x * movementSpeed, characterRb.velocity.y);
    }
    void CheckGrounded()
    {
        Vector2 rayStart = transform.position;
        Vector2 rayDirection = Vector2.down;
        float rayLength = 1f;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, groundLayer);
        Debug.DrawRay(rayStart, rayDirection * rayLength, hit.collider != null ? Color.green : Color.red);
        if (hit.collider != null)
        {
            if (!isGrounded)
            {
                isGrounded = true;
                curState = PlayerState.Idle;
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                curState = PlayerState.Jump;
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            characterRb.velocity = new Vector2(characterRb.velocity.x, jumpForce);
        }
    }
    private void OnShootStarted()
    {
        isShooting = true;
        GameManager.instance.playerIsShooting = true;
    }
    private void OnShootCancelled()
    {
        isShooting = false;
        GameManager.instance.playerIsShooting = false;
    }
    public void Shoot()
    {
        if (isShooting)
        {
            gunController.Shoot(isFacingRight);
        }
    }


    #endregion

    #region other methods

    private void Recoil(Vector2 dir)
    {
        if(curState != PlayerState.Idle)
            return;
        isNegateAnimator = true;
        characterRb.velocity = dir.normalized * (-1 * attackModel.RecoilAmount);
        StartCoroutine(ResetVelocityAfterDelay(0.1f));
    }
    IEnumerator ResetVelocityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        characterRb.velocity = new Vector2(0, characterRb.velocity.y);  // or whatever the velocity was before the recoil
        isNegateAnimator = false;
    }
    #endregion
    
}
