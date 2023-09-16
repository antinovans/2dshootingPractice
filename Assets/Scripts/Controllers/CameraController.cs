using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 玩家或其他要跟随的目标
    public float smoothing = 5.0f; // 平滑度
    private Camera camera;
    private float originalOrthographicSize;   // 保存原始的摄像机大小
    
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
    
    //zoom related
    private bool isZooming = false;
    private Vector3 zoomOnTargetPosition;
    private float zoomDuration;
    private float currentZoomTime;
    private Vector3 preZoomPosition;
    void Start()
    {
        // 计算摄像机和目标之间的初始偏移
        currentXOffset = 0f;
        originalPos = transform.position;
        GameManager.instance.onBulletFire += CameraShake;
        GameManager.instance.onCameraZoomAt += ZoomOn;
        
        camera = GetComponent<Camera>();
        originalOrthographicSize = camera.orthographicSize;
    }

    void LateUpdate()
    {
        UpdateCameraDesiredPosition();
        if (isZooming)
        {
            UpdateZoomingEffect();
        }
        else
        {
            UpdateShakingEffect();
        }
    }

    private void UpdateShakingEffect()
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

        UpdateCameraDesiredPosition();
        transform.position = originalPos;
    }

    private void UpdateCameraDesiredPosition()
    {
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
    }

    private void UpdateZoomingEffect()
    {
        currentZoomTime += Time.unscaledDeltaTime;
        float progress = currentZoomTime / zoomDuration;
        // Lerp摄像机位置
        transform.position = Vector3.Lerp(preZoomPosition, zoomOnTargetPosition, progress);
        // Lerp摄像机大小
        camera.orthographicSize = Mathf.Lerp(originalOrthographicSize, originalOrthographicSize/2, progress);
        if(currentZoomTime >= zoomDuration)
        {
            isZooming = false;
            StartCoroutine(LerpCameraBackToOriginal());
        }
    }

    IEnumerator LerpCameraBackToOriginal()
    {
        float timer = 0f;
        float zoomBackDuraiton = 0.5f;
        var beginPos = transform.position;
        while (timer < zoomBackDuraiton)
        {
            timer += Time.unscaledDeltaTime;
            var progress = timer / zoomBackDuraiton;
            
            // Lerp摄像机大小
            camera.orthographicSize = Mathf.Lerp(originalOrthographicSize/2, originalOrthographicSize, progress);
            // Lerp摄像机位置
            transform.position = Vector3.Lerp(beginPos, originalPos, progress);

            yield return null;  // 等待下一帧
        }
    }
    private void CameraShake(Vector2 fireDir)
    {
        var curShakeAmount = GameManager.instance.attackModel.CameraShakeAmount;
        if( curShakeAmount < 0.01f)
            return;
        shakeAmount = curShakeAmount;
        shakeDuration = shakeDurationConst;
    }
    
    private void ZoomOn(Vector2 pos, float duration)
    {
        // 停止抖动
        shakeDuration = 0f;
        
        isZooming = true;
        zoomOnTargetPosition = new Vector3(pos.x, pos.y, transform.position.z);
        zoomDuration = duration;
        currentZoomTime = 0;
        preZoomPosition = transform.position; // 保存缩放前的位置
    }
}
