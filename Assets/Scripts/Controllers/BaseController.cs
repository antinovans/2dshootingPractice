using System.Collections;
using UnityEngine;

public class BaseController : MonoBehaviour
{
        public int Hp;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        [SerializeField]private Color dmgColor;

        private void Start()
        {
                GameManager.instance.target = transform;
                GameManager.instance.onEnemyAttack += TakeDamage;
                spriteRenderer = GetComponent<SpriteRenderer>();
                originalColor = spriteRenderer.color;
        }

        private void TakeDamage(int dmg)
        {
                Hp -= dmg;
                FlashWhite(0.1f);
        }
        public void FlashWhite(float duration)
        {
                StartCoroutine(Flash(duration));
        }

        private IEnumerator Flash(float duration)
        {
                spriteRenderer.color = dmgColor;
                yield return new WaitForSeconds(duration);
                spriteRenderer.color = originalColor;
        }
}