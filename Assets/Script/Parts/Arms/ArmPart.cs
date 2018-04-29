using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPart : MonoBehaviour, IPart {

    public BasicArmScriptable armData;
    public IFightStyleSolver fightHandler;
    public Animator animator;
    public Collider blockingCollider;

    public int Damage { get { return armData.damage; } set { armData.damage = value; } }
    public int HeavyDamage { get { return armData.heavyDamage; } set { armData.heavyDamage = value; } }
    public int StaminaCost { get { return armData.normalStaminaCost; } set { armData.normalStaminaCost = value; } }
    public int HeavyStaminaCost { get { return armData.heavyStaminaCost; } set { armData.heavyStaminaCost = value; } }

    private CharacterBodyCostumization body;

    private const string BlockingAnimationState = "Blocking";


    // Use this for initialization
    void Start () {
        fightHandler = GetComponent<IFightStyleSolver>();
        animator = GetComponent<Animator>();
        fightHandler.SetAnimator(animator);
    }

    public void ArmsAddedToBody(CharacterBodyCostumization body)
    {
        this.body = body;
    }

    public void AttackAbility()
    {
        armData.AttackAbility();
    }

    public void AttackInput(CombatInput input)
    {
        fightHandler.ReceiveInput(input);
    }

    public void StartedNewAttack(ECombatInputType attack)
    {
        //Set attack type
        fightHandler.CurrentAttack = attack;
        //Set attacking state
        if(attack != ECombatInputType.NONE)
        {
            fightHandler.IsAttacking = true;
        }
        else
        {
            fightHandler.IsAttacking = false;
            return;
        }

    }

    public void SpendStamina()
    {
        int staminaSpent;
        if (fightHandler.CurrentAttack == ECombatInputType.WEAK_ATTACK)
        {
            staminaSpent = StaminaCost;
        }
        else
        {
            staminaSpent = HeavyStaminaCost;
        }
        body.CurrStamina -= staminaSpent;
        body.StartStaminaRegen();
    }

    public void OnAttackHit(Collider enemy)
    {
        int damageDone;
        if (fightHandler.CurrentAttack == ECombatInputType.WEAK_ATTACK)
        {
            damageDone = Damage;
        } else 
        {
            damageDone = HeavyDamage;
        }

        if(enemy.GetComponentInParent<Enemy>() != null)
        {
            enemy.GetComponentInParent<Enemy>().Damage(damageDone);
        }

    }

    public void Block()
    {
        animator.SetBool(BlockingAnimationState, true);
        blockingCollider.enabled = true;
    }

    public void BlockHit(Collider attacker)
    {
        attacker.GetComponentInParent<Enemy>().IsAttackBlocked = true;
    }

    public void UnBlock()
    {
        animator.SetBool(BlockingAnimationState, false);
        blockingCollider.enabled = false;
    }

    public Sprite GetSprite()
    {
        return armData.image;
    }

    public string GetName()
    {
        return armData.name;
    }
}
