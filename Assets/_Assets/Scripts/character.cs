using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using static Enemy;

public class character : MonoBehaviour
{
    [SerializeField] Animator characterAnim;
    [SerializeField] Animator shadowAnim;
    [SerializeField] Animator spellIndicatorAnim;
    [SerializeField] Animator itemIndicatorAnim;
    [SerializeField] SpriteRenderer characterRenderer;
    [SerializeField] GameObject damagePopup;
    [SerializeField] GameObject deathSmoke;

    [Header("Stats")]
    public int baseStrength;
    public int baseMagic;

    public bool isPlayer;

    public Transform attackPoint;
    public Transform spellCastPoint;
    public itemEffect preparedEffect;

    public Dictionary<string, float> animLength = new Dictionary<string, float>();
    private Material myMat;
    private BoxCollider2D myCollider;

    public delegate void OnEndTurn(character character);
    public event OnEndTurn onEndTurn;

    public delegate void OnHealthChanged(int change);
    public event OnHealthChanged onHealthChanged;

    public delegate void OnCharacterClicked(character target, itemEffect preparedEffect);
    public event OnCharacterClicked onCharacterClicked;

    private void Awake()
    {
        GetAnimLengths();

        myCollider = characterRenderer.GetComponent<BoxCollider2D>();

        myMat = characterRenderer.material;
    }

    public IEnumerator BasicAttackAnimation(character target)
    {
        Vector3 startPos = transform.position;

        PlayAnim("run");
        while (Vector3.Distance(transform.position, target.attackPoint.position) >= 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target.attackPoint.position, 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        PlayAnim("basicAttack");
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        target.TakeDamage(-(1 + baseStrength));
        yield return new WaitForSeconds(animLength["basicAttack"] / 3f);
        PlayAnim("return");
        yield return new WaitForSeconds(0.3f);
        PlayAnim("idle");
        while (Vector3.Distance(transform.position, startPos) >= 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, 8 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.3f);

        onEndTurn?.Invoke(this);
    }

    public IEnumerator SpellAttackAnimation(character target, itemEffect spell)
    {
        PlayAnim("spellAttack");
        yield return new WaitForSeconds(animLength["spellAttack"] / 3f);
        yield return new WaitForSeconds(animLength["spellAttack"] / 3f);
        projectile spellProjectile = Instantiate(spell.spellProjectile, spellCastPoint.transform.position, spellCastPoint.transform.rotation);
        spellProjectile.Init(target, spell, this, isPlayer);

        PlayAnim("idle");

        yield return new WaitForSeconds(2f);

        onEndTurn?.Invoke(this);
    }

    public IEnumerator MoveTo(Transform destination)
    {
        PlayAnim("walk");
        while (Vector3.Distance(transform.position, destination.position) >= 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destination.position, 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        PlayAnim("idle");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= -1)
        {
            onHealthChanged?.Invoke(damage);

            Vector3 randomPos = new Vector3(Random.Range(myCollider.bounds.min.x, myCollider.bounds.max.x), Random.Range(myCollider.bounds.min.y, myCollider.bounds.max.y), 4);
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            GameObject popup = Instantiate(damagePopup, randomPos, randomRotation);
            popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + damage;
            Destroy(popup, 1.5f);

            StartCoroutine(DamageFlash());
            PlayAnim("hurt");
        }
    }

    public void Heal(int health)
    {
        if(health > 0)
        {
            onHealthChanged?.Invoke(health);
            //do a particle here
        }
    }

    public void ProcessItemEffect(itemEffect effect, character usingCharacter)
    {
        TakeDamage(-(effect.baseDamage + usingCharacter.baseMagic));
    }

    public IEnumerator DeathSmoke()
    {
        yield return new WaitForSeconds(animLength["dead"]);
        yield return new WaitForSeconds(0.5f);

        GameObject smoke = Instantiate(deathSmoke, transform.position, transform.rotation);
        Destroy(smoke.gameObject, 1.5f);

        Destroy(this.gameObject);
    }

    public IEnumerator DamageFlash()
    {
        int time = 3;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.red, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 3;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.white, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 3;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.red, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }

        time = 20;
        while (time > 0)
        {
            myMat.color = Color.Lerp(myMat.color, Color.white, 50 * Time.deltaTime);
            time--;
            yield return new WaitForEndOfFrame();
        }
    }

    public void PlayAnim(string name)
    {
        characterAnim.SetTrigger(name);
        shadowAnim.SetTrigger(name);
    }

    public void SetSpellIndicator(bool state)
    {
        spellIndicatorAnim.SetBool("show", state);
        spellIndicatorAnim.SetBool("finish", !state);
    }

    public void SetItemIndicator(bool state)
    {
        itemIndicatorAnim.SetBool("show", state);
        itemIndicatorAnim.SetBool("finish", !state);
    }
    private void GetAnimLengths()
    {
        animLength.Clear();
        AnimationClip[] clips = characterAnim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            animLength.Add(clip.name, clip.length);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked!");
        onCharacterClicked?.Invoke(this, preparedEffect);
    }
}
