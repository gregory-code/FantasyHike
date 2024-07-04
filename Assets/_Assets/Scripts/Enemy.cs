using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] enemyUI enemyUIPrefab;
    private enemyUI myUI;
    [SerializeField] Animator enemyAnim;
    [SerializeField] Animator shadowAnim;

    [SerializeField] Animator attackIndicatorAnim;

    [SerializeField] GameObject damagePopup;

    [SerializeField] int maxHealth;

    private Material myMat;
    private BoxCollider2D collider;

    public Transform playerAttackPoint;
    [SerializeField] float yMidPointAdjustment;
    [SerializeField] float yUIadjustment;

    public delegate void OnEnemyClicked(Enemy enemy);
    public event OnEnemyClicked onEnemyClicked;

    private void Start()
    {
        myUI = Instantiate(enemyUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, yUIadjustment);
        myUI.Init(this, maxHealth);

        collider = GetComponent<BoxCollider2D>();

        myMat = GetComponent<SpriteRenderer>().material;
    }

    public void TakeDamage(int damage)
    {
        if(damage <= -1)
        {
            myUI.ChangeHealth(damage);

            Vector3 randomPos = new Vector3(Random.Range(collider.bounds.min.x, collider.bounds.max.x), Random.Range(collider.bounds.min.y, collider.bounds.max.y), 4);
            Quaternion randomRotation = Quaternion.Euler(0,0, Random.Range(-10f, 10f));
            GameObject popup = Instantiate(damagePopup, randomPos, randomRotation);
            popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + damage;
            Destroy(popup, 1.5f);

            StartCoroutine(DamageFlash());
            PlayAnim("hurt");
        }
    }

    private IEnumerator DamageFlash()
    {
        int time = 10;
        while(time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.red, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 10;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.white, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 10;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.red, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 30;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.white, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetAttackIndicator(bool state)
    {
        attackIndicatorAnim.SetBool("swing", state); 
        attackIndicatorAnim.SetBool("finish", !state);
    }

    private void PlayAnim(string name)
    {
        enemyAnim.SetTrigger(name);
        shadowAnim.SetTrigger(name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onEnemyClicked?.Invoke(this);
    }
}
