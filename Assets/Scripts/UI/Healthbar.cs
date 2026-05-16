using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Player _player; 
    private Slider _healthbar;


    public void Awake()
    {
        _healthbar = GetComponent<Slider>();
    }

    public void Start()
    {
        // get player using the tag
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Debug.Assert(_player != null, "Reference of Player in healthbar is missing!");

        
        float currentHealth = _player.GetCurrentHP();
        float maxHealth = _player.playerHP;

        UpdateHealthUI(currentHealth, maxHealth);
        // subscribe to broadcast
        _player.OnHealthChanged += UpdateHealthUI;
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        Debug.Log("Healthbar updated" + currentHP + maxHP);
        _healthbar.maxValue = maxHP;
        _healthbar.value = currentHP;
    }

    void OnDestroy()
    {
        if(_player != null)
        {
            _player.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
