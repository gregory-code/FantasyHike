using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] GameObject hitVFX;
    [SerializeField] float speed;
    private Vector3 targetPos;
    private itemEffect spell;
    private character target;

    private character usingCharacter;

    public void Init(character target, itemEffect spell, character usingCharacter, bool isEnemy)
    {
        this.target = target;
        this.spell = spell;
        this.usingCharacter = usingCharacter;
        targetPos = target.transform.position;
        targetPos.x += (isEnemy) ? 0.2f : -0.2f;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, targetPos) <= 0.2f)
        {
            GameObject hit = Instantiate(hitVFX, transform.position, transform.rotation);
            hit.transform.SetParent(null);

            target.ProcessItemEffect(spell, usingCharacter);

            Destroy(hit, 1f);
            Destroy(this.gameObject);
        }
    }
}
