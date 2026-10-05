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
    [SerializeField] private int attackIndex;

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
        if (attackIndex == 0)
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
        if (attackIndex == 1)
        {
            if (playerCombat.skill1CooldownLeft <= 0f)
            {

                cooldownOverlay.fillAmount = 0f;
                cooldownText.text = "";

                return;
            }
            cooldownOverlay.fillAmount =
            playerCombat.skill1CooldownLeft /
            playerCombat.Skill1Cooldown;

            cooldownText.text =
            playerCombat.skill1CooldownLeft
            .ToString("F1");
        }
        if (attackIndex == 2)
        {
            if (playerCombat.skill2CooldownLeft <= 0f)
            {

                cooldownOverlay.fillAmount = 0f;
                cooldownText.text = "";

                return;
            }
            cooldownOverlay.fillAmount =
            playerCombat.skill2CooldownLeft /
            playerCombat.Skill2Cooldown;

            cooldownText.text =
            playerCombat.skill2CooldownLeft
            .ToString("F1");
        }
        if (attackIndex == 3)
        {
            if (playerCombat.skill3CooldownLeft <= 0f)
            {

                cooldownOverlay.fillAmount = 0f;
                cooldownText.text = "";

                return;
            }
            cooldownOverlay.fillAmount =
            playerCombat.skill3CooldownLeft /
            playerCombat.Skill3Cooldown;

            cooldownText.text =
            playerCombat.skill3CooldownLeft
            .ToString("F1");
        }


        
    }
}