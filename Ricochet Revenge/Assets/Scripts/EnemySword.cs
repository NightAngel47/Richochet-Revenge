using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    public float damage;

    private float myDamage;

    public float attackDelay;
    public float collidingRecency;

    private bool collidingWPlayer;
    private bool hasCoroutine;

    public Animator meleeEnemyAnim;
   
    // Start is called before the first frame update
    void Start()     
    {
        myDamage = damage;
    }
    
    private IEnumerator Stab()
    {
        meleeEnemyAnim.SetBool("Attacking", true);
        damage = 0;
        yield return new WaitForSeconds(attackDelay);
        damage = myDamage;

        yield return new WaitForFixedUpdate();

        meleeEnemyAnim.SetBool("Attacking", false);
        if (collidingWPlayer)
        {
            StartCoroutine(Stab());
        }
        else
        {
            hasCoroutine = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            collidingWPlayer = true;
            if(!hasCoroutine)
            {
                StartCoroutine(Stab());
                hasCoroutine = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collidingWPlayer = false;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        collidingRecency = 0.01F;
    }
}
