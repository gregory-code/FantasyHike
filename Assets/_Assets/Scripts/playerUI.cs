using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerUI : MonoBehaviour
{
    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image mana;
    [SerializeField] TextMeshProUGUI manaText;

    private Player owner;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    private int currentHealth;
    private int maxHealth = 1;

    private int currentMana;
    private int maxMana = 1;

    public void Init(Player owner, int maxHealth, int maxMana)
    {
        this.owner = owner;
        owner.onHealthChanged += ChangeHealth;

        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;

        owner.currentHealth = currentHealth;

        this.maxMana = maxMana;
        this.currentMana = maxMana;

        healthText.text = $"{currentHealth} / {maxHealth}";
        manaText.text = $"{currentMana} / {maxMana}";

        health.fillAmount = 1f;
        mana.fillAmount = 1f;
    }

    public void ChangeHealth(int change)
    {
        currentHealth += change;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthText.text = $"{currentHealth} / {maxHealth}";
        owner.currentHealth = currentHealth;
        StartCoroutine(LerpValue(health, currentHealth, maxHealth));

        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
        }
    }

    private IEnumerator LerpValue(Image image, int current, int max)
    {
        float desiredHealth = Round((float)current / (float)max);

        while(Round(image.fillAmount) != desiredHealth)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, desiredHealth, 7 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private float Round(float value)
    {
        return Mathf.Round(value * 1000.0f) * 0.001f;
    }

    public int GetMana()
    {
        return currentMana;
    }

    public bool TryUseMana(int cost)
    {
        if(currentMana >= cost)
        {
            currentMana -= cost;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            manaText.text = $"{currentMana} / {maxMana}";
            StartCoroutine(LerpValue(mana, currentMana, maxMana));
            return true;
        }

        return false;
    }
}
