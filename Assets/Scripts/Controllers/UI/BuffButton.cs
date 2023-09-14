using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BuffButton : MonoBehaviour
{
    private Buff assignedBuff;

    [SerializeField]
    private TextMeshProUGUI buffDescriptionText; 

    
    private void Start() => OnShowUp();

    private void OnShowUp()
    {
        assignedBuff = BuffManager.instance.GetRandomBuff();
        UpdateButtonText();
    }

    public void OnButtonClick()
    {
        assignedBuff?.OnApply();
        BuffManager.instance.RecalculateBuffPool();
        assignedBuff = BuffManager.instance.GetRandomBuff();
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (assignedBuff != null && buffDescriptionText != null)
        {
            buffDescriptionText.text = assignedBuff.config.description;
        }
    }

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnButtonClick);
    }
}