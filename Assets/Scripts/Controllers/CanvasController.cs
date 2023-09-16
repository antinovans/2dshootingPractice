using System;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
        [SerializeField] private GameObject[] upgradeButtons;

        private void Start()
        {
                GameManager.instance.onLevelClear += ShowUpgradeButtons;
        }

        public void ShowUpgradeButtons()
        {
                foreach (var btn in upgradeButtons)
                {
                        btn.SetActive(true);
                }
        }
        public void HideUpgradeButtons()
        {
                foreach (var btn in upgradeButtons)
                {
                        btn.SetActive(false);
                }
        }
}