using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private TMP_Text keybindText;

    [Header("Skill")]
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private InputActionReference inputAction;


    private PlayerCombat playerCombat;

    private void Awake()
    {
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }

    private void Start()
    {
        if (icon != null)
        {
            icon.sprite = skillIcon;
        }

        if (inputAction != null)
        {
            keybindText.text = InputBindingUtility.GetPreferredBinding(inputAction.action);
        }

        cooldownOverlay.fillAmount = 0f;
        cooldownText.text = "";
    }

    private void Update()
    {
        HandleCooldown();
    }

    private void HandleCooldown()
    {
        if (playerCombat.attackCooldownLeft <= 0f)
        {

            cooldownOverlay.fillAmount = 0f;
            cooldownText.text = "";

            return;
        }


        cooldownOverlay.fillAmount =
            playerCombat.attackCooldownLeft /
            playerCombat.AttackCooldown;

        cooldownText.text =
        playerCombat.attackCooldownLeft
        .ToString("F1");
    }
}