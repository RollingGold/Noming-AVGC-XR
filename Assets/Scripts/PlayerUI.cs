using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    [SerializeField] private TMP_Text healthText;

    private Player player;

    private void Awake()
    {
        player =
            GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<Player>();
    }

    private void Start()
    {
        healthSlider.maxValue =
            player.MaxHealth;
    }

    private void Update()
    {
        healthSlider.value =
            player.currentHealth;

        healthText.text =
            "HP " +
            player.currentHealth +
            "/" +
            player.MaxHealth;
    }
}