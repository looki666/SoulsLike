using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {


    public float attackRange = 2.5f;
    public float aggroRange = 5f;
    public float chaseRange = 15f;

    private Vector3 startPos;

    [SerializeField]
    [ReadOnly]
    private int maxHp = 100;

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

    public Transform hpBarAnchor;
    public Transform HpBarAnchor { get { return hpBarAnchor; } set { hpBarAnchor = value; } }
    private Slider hpBar;
    public Slider HpBar { set { hpBar = value; } }

    public int CurrHp
    {
        get { return hp; }
        set { hp = Mathf.Clamp(value, 0, maxHp); if (hpBar != null) { hpBar.value = hp; } }
    }

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
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        //Has no target, try and find new target
        if (player[0] != null)
        {
            Physics.OverlapSphereNonAlloc(rb.position, aggroRange, player, 1 << 8);
        }

        //Got a target
        if (player[0] != null)
        {
            //Close enough to attack
            if (Vector3.Distance(player[0].transform.position, rb.position) <= attackRange)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }

            //Outside of range, reset target
            if (Vector3.Distance(player[0].transform.position, rb.position) > chaseRange)
            {
                player[0] = null;
            }

        }

    }

    public void Damage(int dmg)
    {
        CurrHp -= dmg;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        DebugExtension.DrawCircle(transform.position, transform.up, Color.yellow, chaseRange);
        DebugExtension.DrawCircle(transform.position, transform.up, Color.green, aggroRange);
        DebugExtension.DrawCircle(transform.position, transform.up, Color.red, attackRange);
    }
}
