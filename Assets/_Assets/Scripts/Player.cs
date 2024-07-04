using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] playerUI playerUIPrefab;
    private playerUI myUI;
    private Material myMat;
    private BoxCollider2D collider;

    [SerializeField] GameObject damagePopup;

    [SerializeField] actionButtons actionButtonsPrefab;
    private actionButtons actionButtons;

    [SerializeField] Animator playerAnim;
    [SerializeField] Animator shadowAnim;
    private Dictionary<string, float> animLength = new Dictionary<string, float>();

    [SerializeField] Inventory inventory;

    [SerializeField] int maxHealth;
    [SerializeField] int maxMana;

    [SerializeField] int strength;

    private void Start()
    {
        myUI = Instantiate(playerUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, 1.3f);
        myUI.Init(this, maxHealth, maxMana);

        collider = GetComponent<BoxCollider2D>();

        myMat = transform.GetChild(0).GetComponent<SpriteRenderer>().material;

        GetAnimLengths();

        ShowActions();
    }

    private void GetAnimLengths()
    {
        animLength.Clear();
        AnimationClip[] clips = playerAnim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            Debug.Log(clip.name);
            animLength.Add(clip.name, clip.length);
        }
    }

    public void Attack(Enemy target)
    {
        StartCoroutine(BasicAttackAnimation(target));
    }

    private IEnumerator BasicAttackAnimation(Enemy target)
    {
        Vector3 startPos = transform.position;

        PlayAnim("run");
        while(Vector3.Distance(transform.position, target.playerAttackPoint.position) >= 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, target.playerAttackPoint.position, 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        PlayAnim("basicAttack");
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        target.TakeDamage(-(1 + strength));
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        PlayAnim("return");
        yield return new WaitForSeconds(0.3f);
        PlayAnim("idle");
        while (Vector3.Distance(transform.position, startPos) >= 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, 8 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= -1)
        {
            myUI.ChangeHealth(damage);

            Vector3 randomPos = new Vector3(Random.Range(collider.bounds.min.x, collider.bounds.max.x), Random.Range(collider.bounds.min.y, collider.bounds.max.y), 4);
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
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
        while (time > 0)
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

    private void PlayAnim(string name)
    {
        playerAnim.SetTrigger(name);
        shadowAnim.SetTrigger(name);
    }

    private void ShowActions()
    {
        actionButtons = Instantiate(actionButtonsPrefab, transform);
        actionButtons.transform.localPosition = new Vector2(0, 0.9f);
        actionButtons.Init(this, inventory);
    }
}
