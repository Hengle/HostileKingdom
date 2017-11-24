﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableUnitBehaviour : Characters
{
    //public Animator anim;
    [SerializeField]
    CardsBehaviour cards;
    public bool isSelected = false;

    [Header("Timers")]
    [SerializeField]
    float cooldownAttack;
    [SerializeField]
    float timeCounter;

    [Header("Distances")]
    [SerializeField]
    float chaseRange;
    Vector3 newFormationPosition;
    [SerializeField]
    float newDestinationRadius;

    [Header("OnScreen")]
    public bool isOnScreen = false;
    [HideInInspector]
    public Vector2 screenPosition;
    float maxDistance = Mathf.Infinity;
    [SerializeField]
    MouseBehaviour mouse;
    RaycastHit hit;

    [Header("EnemyInteraction")]
    Characters characters;
    bool isAttacking; 
    bool canAttack;

    void Start()
    {
        base.MyStart();
        if(cards != null)
        {
            cards.targetName.text = characterName;
            cards.startingHealth = startingHitPoints;
            cards.MyStart();
        }
    }

    void Update()
    {
        base.MyUpdate();
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (mouse.UnitWithinScreenSpace(screenPosition)) //This function lets the player know if the Unit is in the screenview to do a drag selection. 
        {
            isOnScreen = true;
        }
        else
        {
            if (isOnScreen)
            {
                isOnScreen = false;
            }
        }
    }

    #region Updates
    public override void IdleUpdate()
    {
        if (isAttacking)
        {
            if (characters.hitPoints <= 0)
            {
                EnemyDies();
                return;
            }
            if (timeCounter >= cooldownAttack)
            {
                canAttack = true;
                SetAttack();
                return;
            }
            else timeCounter += Time.deltaTime;

            LookAtTarget();
        }
    }

    public override void MoveUpdate()
    {
        if (Vector3.Distance(transform.position, agent.destination) <= newDestinationRadius)
        {
            SetIdle();
            return; 
        }
    }

    public override void ChaseUpdate()
    {
        if (distanceFromTarget < scope)
        {
            if (!isAttacking)
            {
                canAttack = true;
                SetAttack();
                return;
            }
            else
            {
                SetIdle();
                return; 
            }
        }
        else agent.SetDestination(targetTransform.position);
    }

    public override void AttackUpdate()
    {
        if (canAttack)
        {
            if (!isAttacking) isAttacking = true; 
            canAttack = false;
            selectedTarget.GetComponent<EnemyBehaviour>().TakeDamage(attack);

            timeCounter = 0;
            SetIdle();
            return;
        }
        else if (distanceFromTarget >= scope)
        {
            isAttacking = false; 
            SetChase();
            return;
        }
    }
    #endregion

    #region Sets
    public override void SetMovement()
    {
        isAttacking = false;
        base.SetMovement();
    }

    public override void SetAttack()
    {
        isAttacking = true;
        base.SetAttack();
    }

    public override void SetDead()
    {
        if (isOnScreen)
        {
            mouse.selectableUnits.Remove(this);
            isOnScreen = false;
        }
        if (mouse.selectedUnit == this) mouse.selectedUnit = null;
        if (mouse.selectedUnits.Contains(this)) mouse.selectedUnits.Remove(this); 
        base.SetDead();
    }
    #endregion

    #region PublicVoids

    public void PlayableUnitTakeDamage(float damage, GameObject autoTarget)
    {
        hitPoints -= damage;
        cards.UpdateLifeBar(hitPoints);

        if (selectedTarget == null)
        {
            selectedTarget = autoTarget;
            characters = selectedTarget.GetComponent<Characters>();
            if (!isAttacking)
            {
                canAttack = true;
                SetAttack();
                return;
            }
        }
    }

    public void ClickUpdate(Vector3 formationPosition) //When I click right button. It's called from the InputManager script.  
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") || 
                hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayableUnit"))
            {
                if (selectedTarget != null)
                {
                    targetTransform = null;
                    selectedTarget = null;
                    characters = null; 
                }
                newFormationPosition = formationPosition; //If I have more than 1 unit selected it will change the value to avoid conflicts. 
                agent.SetDestination(hit.point + newFormationPosition);

                SetMovement();
                return;
            }
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.transform.gameObject != selectedTarget)
                {
                    targetTransform = hit.transform;
                    selectedTarget = targetTransform.gameObject;
                    characters = selectedTarget.GetComponent<Characters>();

                    SetChase();
                    return;
                }
            }
        }
    }
    #endregion

    void EnemyDies()
    {
        distanceFromTarget = Mathf.Infinity;
        if (characters.isDead == false) characters.SetDead();
        characters = null;
        targetTransform = null;
        selectedTarget = null;
        isAttacking = false;
        SetIdle();
        return;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Color newColor = Color.green;
        newColor.a = 0.2f;
        Gizmos.color = newColor;
        Gizmos.DrawSphere(transform.position, scope);

       if (state == UnitState.Movement)
       {
            Gizmos.color = newColor;
            Gizmos.DrawSphere(agent.destination, newDestinationRadius);
       }
    }
}