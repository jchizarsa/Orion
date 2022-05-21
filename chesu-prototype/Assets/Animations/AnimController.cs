using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{

    public Animator anim;
    public int attackHash;
    public int isDeadHash;
    public bool canAttack;
    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        attackHash = Animator.StringToHash("attack");
        isDeadHash = Animator.StringToHash("isDead");
        canAttack = false;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack)
        {
            anim.SetBool(attackHash, true);
        }
        else
        {
            anim.SetBool(attackHash, false);
        }

        if(isDead)
        {
            anim.SetBool(isDeadHash, true);
        }
    }
}
