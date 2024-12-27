using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class enemyUI : MonoBehaviour
{
    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;

    private Enemy owner;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    private int currentHealth;
    private int maxHealth = 1;

    public void Init(Enemy owner, int maxHealth)
    {
        this.owner = owner;
        owner.onHealthChanged += ChangeHealth;

        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;

        owner.currentHealth = currentHealth;

        healthText.text = $"{currentHealth} / {maxHealth}";

        health.fillAmount = 1f;
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

        while (Round(image.fillAmount) != desiredHealth)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, desiredHealth, 7 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private float Round(float value)
    {
        return Mathf.Round(value * 1000.0f) * 0.001f;
    }

}
