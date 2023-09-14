using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 玩家或其他要跟随的目标
    public float smoothing = 5.0f; // 平滑度

    public Vector3 fixedOffset = new Vector3(2.5f, 1f, -10f);
    public float xOffsetFacingRight = 3f; // 玩家面向右时的额外偏移量
    public float xOffsetFacingLeft = -3f; // 玩家面向左时的额外偏移量
    private float currentXOffset;
    
    //camara shake
    private const float shakeDurationConst = 0.2f; //固定持续时间
    private float shakeDuration = 0f;
    private float shakeAmount = 0f;
    private float decreaseFactor = 1.0f;
    private Vector3 originalPos;
    void Start()
    {
        // 计算摄像机和目标之间的初始偏移
        currentXOffset = 0f;
        originalPos = transform.position;
        GameManager.instance.onBulletFire += CameraShake;
    }

    void LateUpdate()
    {
        if (shakeDuration > 0)
        {
            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
            
            //衰减shakeAmount
            shakeAmount -= (0.2f / shakeDurationConst) * Time.deltaTime; 
            shakeAmount = Mathf.Max(0, shakeAmount); //保证shakeAmount不为负
            
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.position = originalPos;
        }
        
        float targetXOffset = 0f;
        if(GameManager.instance.playerIsShooting)
        {
            // 根据玩家面朝的方向调整偏移量
            targetXOffset = (GameManager.instance.playerIsFacingRight) ? xOffsetFacingRight : xOffsetFacingLeft;
        }
        currentXOffset = Mathf.Lerp(currentXOffset, targetXOffset, smoothing * Time.deltaTime);
        
        // 计算新的摄像机位置
        Vector3 targetCameraPosition = target.position + fixedOffset + new Vector3(currentXOffset, 0, 0);

        // 平滑地移动到新位置
        originalPos = Vector3.Lerp(transform.position, targetCameraPosition, smoothing * Time.deltaTime);
        transform.position = originalPos;
    }
    
    private void CameraShake(Vector2 fireDir)
    {
        var curShakeAmount = GameManager.instance.attackModel.CameraShakeAmount;
        if( curShakeAmount < 0.01f)
            return;
        shakeAmount = curShakeAmount;
        shakeDuration = shakeDurationConst;
    }
}
