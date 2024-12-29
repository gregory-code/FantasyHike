using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.Pool;

public class character : MonoBehaviour, IPointerClickHandler
{
    [Header("Prefabs")]
    [SerializeField] Animator spellIndicatorAnim;
    [SerializeField] Animator itemIndicatorAnim;
    [SerializeField] GameObject damagePopup;
    [SerializeField] GameObject healPopup;
    [SerializeField] GameObject deathSmoke;

    private SpriteRenderer characterRenderer;
    private Animator characterAnim;
    private Animator shadowAnim;
    private BoxCollider2D myCollider;
    private Material myMat;
    
    private Transform attackPoint;
    private Transform spellCastPoint;

    public item preparedEffect;

    //public IObjectPool<Animator> pool;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;

    public int baseStrength;
    public int baseMagic;
    public int baseDefense;

    public bool isPlayer;


    /// ///////////////////////////////////////////////////////////

    public Dictionary<string, float> animLength = new Dictionary<string, float>();

    public delegate void OnEndTurn(character character);
    public event OnEndTurn onEndTurn;

    public delegate void OnHealthChanged(int change);
    public event OnHealthChanged onHealthChanged;

    public delegate void OnCharacterClicked(character target, item preparedEffect);
    public event OnCharacterClicked onCharacterClicked;

    private void Awake()
    {
        characterRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        characterAnim = transform.GetChild(0).GetComponent<Animator>();
        shadowAnim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        myCollider = characterRenderer.GetComponent<BoxCollider2D>();
        myMat = characterRenderer.material;

        attackPoint = transform.GetChild(1).GetComponent<Transform>();
        spellCastPoint = transform.GetChild(2).GetComponent<Transform>();

        GetAnimLengths();
    }

    public void SetPreparedEffect(item itemEffect)
    {
        preparedEffect = itemEffect;
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

    public IEnumerator SpellAttackAnimation(character target, item spell)
    {
        PlayAnim("spellAttack");
        yield return new WaitForSeconds(animLength["spellAttack"] / 3f);
        yield return new WaitForSeconds(animLength["spellAttack"] / 3f);
        projectile spellProjectile = Instantiate(spell.spellProjectile, spellCastPoint.transform.position, spellCastPoint.transform.rotation);
        spellProjectile.Init(target, spell, this, isPlayer);

        PlayAnim("idle");

        yield return new WaitForSeconds(1.2f);

        onEndTurn?.Invoke(this);
    }

    public IEnumerator ConsumableAnimation(character target, item consumeable)
    {
        PlayAnim("elixer");
        yield return new WaitForSeconds(animLength["elixer"] / 3f);
        yield return new WaitForSeconds(animLength["elixer"] / 3f);

        StartCoroutine(ProcessItemEffect(consumeable, this, target));

        Instantiate(consumeable.particle, characterRenderer.transform);

        PlayAnim("idle");
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
        damage -= baseDefense;

        if (damage <= -1)
        {
            onHealthChanged?.Invoke(damage);

            SpawnPopup(damagePopup, damage);

            StartCoroutine(DamageFlash());
            PlayAnim("hurt");
        }
    }

    public void Heal(int health)
    {
        if(health > 0)
        {
            SpawnPopup(healPopup, health);
            onHealthChanged?.Invoke(health);
        }
    }

    public IEnumerator ProcessItemEffect(item effect, character usingCharacter, character recivingCharacter)
    {
        effect.UseItem(effect, usingCharacter, recivingCharacter);

        yield return new WaitForSeconds(1.2f);

        onEndTurn?.Invoke(this);
    }

    public IEnumerator DeathSmoke()
    {
        yield return new WaitForSeconds(animLength["dead"]);
        yield return new WaitForSeconds(0.5f);

        GameObject smoke = Instantiate(deathSmoke, transform.position, transform.rotation);
        Destroy(smoke.gameObject, 1.5f);

        Destroy(this.gameObject);
    }

    public void SpawnPopup(GameObject toSpawn, int damage)
    {
        Vector3 randomPos = new Vector3(Random.Range(myCollider.bounds.min.x, myCollider.bounds.max.x), Random.Range(myCollider.bounds.min.y, myCollider.bounds.max.y), 4);
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

        GameObject popup = Instantiate(toSpawn, randomPos, randomRotation);
        popup.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + damage;
        Destroy(popup, 1.5f);
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
