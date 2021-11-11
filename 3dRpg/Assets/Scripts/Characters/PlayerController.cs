﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private float lastAttackTime;
    private CharacterStats characterStats;
    private bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.onEnemyClicked += EventAttack;
    }

  

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if(target != null)
        {

            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);

        //TODO:修改攻击范围
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange){
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;
        //attack
        if(lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //攻击冷却时间
            lastAttackTime = characterStats.attackData.collDown;
        }
    }


    void Hit()
    {
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        characterStats.TakeDamage(characterStats, targetStats);
    }

}
