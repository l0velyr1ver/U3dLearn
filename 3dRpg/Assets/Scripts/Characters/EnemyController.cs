using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;
    private GameObject attackTarget;
    public bool isGuard;
    private float speed;
    public float lookAtTime;
    private float remainLookAtTime;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    private Vector3 guardPos;

    //动画
    bool isWalk;
    bool isChase;
    bool isFollow;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            getNewWayPoint();
        }
    }

    private void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    void SwitchStates()
    {

        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("找到player");
        }

        //发现player 切换到 CHASE
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if(remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        getNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                agent.speed = speed;
                isWalk = false;
                isChase = true;
            
                if (!FoundPlayer())
                {
                    if(remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.CHASE;
                    }

                    agent.destination = transform.position;
                    isFollow = false;  
                }
                else
                {

                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }


                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false; 
    }


    void getNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + guardPos.x,transform.position.y,randomZ + guardPos.z);

        NavMeshHit hit;
        //判断是否随机到walkable
        if(NavMesh.SamplePosition(randomPoint, out hit,patrolRange, 1))
        {
            wayPoint = hit.position;
        }
        else
        {
            wayPoint = transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }


}
