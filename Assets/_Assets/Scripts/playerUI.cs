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
    public Transform statusEffectTransform;

    private Player owner;

    float desiredMana;
    float desiredHealth;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    private int currentHealth;
    private int maxHealth = 1;

    private int currentMana;
    private int maxMana = 1;

    public void Init(Player owner, int maxHealth, int maxMana, int currentHealth, int currentMana)
    {
        this.owner = owner;
        owner.onHealthChanged += ChangeHealth;

        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;

        owner.currentHealth = currentHealth;

        this.maxMana = maxMana;
        this.currentMana = currentMana;

        healthText.text = $"{currentHealth} / {maxHealth}";
        manaText.text = $"{currentMana} / {maxMana}";

        float desiredHealth = Round((float)currentHealth / (float)maxHealth);
        float desiredMana = Round((float)currentMana / (float)maxMana);
        health.fillAmount = desiredHealth;
        mana.fillAmount = desiredMana;
    }

    public void AddedMaxHealth(int addedMaxHealth)
    {
        maxHealth += addedMaxHealth;
        ChangeHealth(addedMaxHealth);
    }

    public void AddedMaxMana(int addedMaxMana)
    {
        maxMana += addedMaxMana;
        TryUseMana(-addedMaxMana);
    }

    public void ChangeHealth(int change)
    {
        currentHealth += change;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthText.text = $"{currentHealth} / {maxHealth}";
        owner.currentHealth = currentHealth;
        desiredHealth = Round((float)currentHealth / (float)maxHealth);
        StartCoroutine(LerpValue(health, true));
        owner.GetSaveData().maxHealth = maxHealth;
        owner.GetSaveData().currentHealth = currentHealth;

        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
        }
    }

    private IEnumerator LerpValue(Image image, bool health)
    {
        while(Round(image.fillAmount) != desiredHealth)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, (health) ? desiredHealth : desiredMana, 7 * Time.deltaTime);
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
            desiredMana = Round((float)currentMana / (float)maxMana);
            StartCoroutine(LerpValue(mana, false));
            owner.GetSaveData().maxMana = maxMana;
            owner.GetSaveData().currentMana = currentMana;
            return true;
        }

        return false;
    }
}
