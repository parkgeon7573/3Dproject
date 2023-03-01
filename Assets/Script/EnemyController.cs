using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum PatrolType
    {
        Normal,
        Random,
        Pingpong,
        Max
    }
    enum MonsterState
    {
        Idle,
        Attack,
        Patrol,
        Chase,
        Hit,
        Max
    }
    NavMeshAgent m_navAgent;
    [SerializeField]
    HeroController m_player;
    EnemyAnimController m_animCtr;
    TweenMove m_tweenMove;
    [SerializeField]
    GameObject m_fxHitPrefab;
    [SerializeField]
    MonsterState m_state;
    [Header("이동경로")]
    [SerializeField]
    PatrolType m_patrolType;
    [SerializeField]
    WaypointController m_wayCtr;
    float m_idleTime;
    float m_idleDuration = 3f;
    float m_attackDist = 1.5f;
    float m_detectDist = 5f;
    bool m_isMove;
    bool m_isReserve;
    int m_curWayPoint = 0;
    int m_prevWayPoint;
    public EnemyAnimController.Motion GetMotion { get { return m_animCtr.GetCurrentMotion(); } }
    #region Animation Event Methods
    void AnimEvnet_Attack()
    {
        var dir = m_player.transform.position - transform.position;
        if (CheckDistance(dir.sqrMagnitude, Mathf.Pow(m_attackDist, 2f)))
        {
            var dot = Vector3.Dot(transform.forward, dir.normalized);
            if(dot > 0.7071f)
            {
                var obj = Instantiate(m_fxHitPrefab);
                obj.transform.position = m_player.transform.position + Vector3.up * 0.6f;
                dir = (transform.position - m_player.transform.transform.position);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
            }
        }
    }
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    #endregion


    IEnumerator Coroutine_SerchTarget(int frame)
    {
        while (m_state == MonsterState.Chase)
        {
            for (int i = 0; i < frame; i++)
                yield return null;
            m_navAgent.SetDestination(m_player.transform.position);
        }
    }

    public void BehaviourPrecess()
    {
        var dir = Vector3.zero;
        switch (m_state)
        {
            case MonsterState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime > m_idleDuration)
                {
                    if (FindTarget())
                    {
                        dir = m_player.transform.position - transform.position;
                        if (CheckDistance(dir.sqrMagnitude, Mathf.Pow(m_attackDist, 2f)))
                        {
                            var temp = GetTargetDir();
                            temp.y = 0f;
                            transform.forward = temp.normalized;

                            SetState(MonsterState.Attack);
                            m_animCtr.Play(EnemyAnimController.Motion.Attack1);
                            return;
                        }
                        SetState(MonsterState.Chase);
                        m_animCtr.Play(EnemyAnimController.Motion.Run);
                        m_navAgent.stoppingDistance = m_attackDist;
                        StartCoroutine(Coroutine_SerchTarget(10));
                        return;
                    }
                    else
                    {
                        SetState(MonsterState.Patrol);
                        m_animCtr.Play(EnemyAnimController.Motion.Run);
                        m_navAgent.stoppingDistance = m_navAgent.radius;
                        m_isMove = false;
                    }
                    m_idleTime = 0f;
                }
                break;
            case MonsterState.Attack:
                break;
            case MonsterState.Patrol:
                if (!FindTarget())
                {
                    if (!m_isMove)
                    {
                        m_isMove = true;
                        if (m_patrolType == PatrolType.Normal)
                        {
                            m_curWayPoint++;
                            if (m_curWayPoint > m_wayCtr.m_waypoints.Length - 1)
                            {
                                m_curWayPoint = 0;
                            }
                        }
                        else if (m_patrolType == PatrolType.Random)
                        {
                            m_curWayPoint = Random.Range(0, m_wayCtr.m_waypoints.Length);
                        }
                        else
                        {
                            if (!m_isReserve)
                            {
                                m_curWayPoint++;
                                if (m_curWayPoint > m_wayCtr.m_waypoints.Length - 1)
                                {
                                    m_isReserve = true;
                                    m_curWayPoint = m_wayCtr.m_waypoints.Length - 1;
                                }
                            }
                            else
                            {
                                m_curWayPoint--;
                                if (m_curWayPoint < 0)
                                {
                                    m_isReserve = false;
                                    m_curWayPoint = 0;
                                }
                            }
                        }
                        m_navAgent.SetDestination(m_wayCtr.m_waypoints[m_curWayPoint].transform.position);
                    }
                    else
                    {
                        dir = m_wayCtr.m_waypoints[m_curWayPoint].transform.position - transform.position;
                        if (CheckDistance(dir.sqrMagnitude, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                        {
                            m_isMove = false;
                            SetIdle(1f);
                            return;
                        }
                    }
                }
                else
                {
                    SetIdle(0f);
                    return;
                }
                break;
            case MonsterState.Chase:
                //m_navAgent.SetDestination(m_player.transform.position);
                dir = m_player.transform.position - transform.position;
                if (CheckDistance(dir.sqrMagnitude, Mathf.Pow(m_attackDist, 2f)))
                {
                    SetIdle(0f);
                    return;
                }
                break;
        }
    }
    public void SetDamage(SkillData skilldata)
    {
        m_animCtr.Play(EnemyAnimController.Motion.Hit);
        if (skilldata.knockbackDist > 0f)
        {
            var dir = transform.position - m_player.transform.position;

            float curMagnitude = (skilldata.knockbackDist / SkillData.MaxknockbackDist);
            float duration = SkillData.MaxKnockbackDuration * curMagnitude;
            CameraShake.Instance.Shake(0.3f * curMagnitude, duration);
            m_tweenMove.Play(transform.position, transform.position + dir.normalized * skilldata.knockbackDist, duration);
        }
    }
    void SetState(MonsterState state)
    {
        m_state = state;
    }
    void SetIdle(float duration)
    {
        SetState(MonsterState.Idle);
        m_animCtr.Play(EnemyAnimController.Motion.Idle);
        SetIdleDuration(duration);
    }
    void SetIdleDuration(float duration)
    {
        if (duration > m_idleDuration) duration = m_idleDuration;
        m_idleTime = m_idleDuration - duration;
    }
    Vector3 GetTargetDir()
    {
        var dir = m_player.transform.position - transform.position;
        return dir;
    }
    bool FindTarget()
    {
        int check = 0;
        RaycastHit hit;
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        if (Physics.Raycast(transform.position + Vector3.up * 0.6f, dir.normalized, out hit, m_detectDist, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Background")))
        {
            if (hit.transform.CompareTag("Player"))
                check++;
                return true;
        }
        Debug.DrawRay(transform.position + Vector3.up * 0.6f, dir.normalized * m_detectDist, Color.magenta);
        /*var dot = Vector3.Dot(transform.forward, dir.normalized);
        if(dot > 0)
        {
            check++;
        }
        if (check == 2)
            return true;*/
        return false;
    }
    bool CheckDistance(float targetDist, float checkDist)
    {
        if (Mathf.Approximately(targetDist, checkDist) || targetDist < checkDist)
        {
            return true;
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_animCtr = GetComponent<EnemyAnimController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_tweenMove = GetComponent<TweenMove>();
    }

    // Update is called once per frame
    void Update()
    {
        BehaviourPrecess();
    }
}