using System;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
        [SerializeField] private GameObject[] upgradeButtons;
        [SerializeField] private GameObject backToMenuButton;

        private void Start()
        {
                GameManager.instance.onLevelClear += ShowButtons;
        }

        public void ShowButtons(bool isGameFinished)
        {
                if (isGameFinished)
                        ShowBackToMenuButton();
                else
                {
                        foreach (var btn in upgradeButtons)
                        {
                                btn.SetActive(true);
                        }
                }
        }
        public void HideUpgradeButtons()
        {
                foreach (var btn in upgradeButtons)
                {
                        btn.SetActive(false);
                }
        }
        public void ShowBackToMenuButton()
        {
                backToMenuButton.SetActive(true);
        }
}