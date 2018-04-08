using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    [ReadOnly]
    private int hp = 100;

    [SerializeField]
    [ReadOnly]
    private bool isDead = false;

    private Rigidbody rb;
    private Collider[] player;
    private Animator animator;

    [SerializeField]
    [ReadOnly]
    private bool isAttackBlocked = false;

    public Transform HpBarAnchor { set; get; }

    public bool IsAttackBlocked
    {
        get
        {
            return isAttackBlocked;
        }

        set
        {
            isAttackBlocked = value;
            if (isAttackBlocked)
            {
                animator.SetTrigger("Blocked");
            }
        }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = new Collider[1];
        HpBarAnchor = transform.Find("HpBar");
    }
	
	// Update is called once per frame
	void Update () {
        Physics.OverlapSphereNonAlloc(rb.position, 3f, player, 1 << 8);

        if (player[0] != null)
        {
            if(animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
        player[0] = null;
    }

    public void Damage(int dmg)
    {
        hp = Mathf.Max(hp - dmg, 0);
    }
}
