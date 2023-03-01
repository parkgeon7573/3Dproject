using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour
{
    NavMeshAgent m_navAgent;
    CharacterController m_charCtr;
    PlayerAnimController m_animCtr;
    [SerializeField]
    GameObject m_attackAreaObj;
    AttackAreaUnitFind[] m_attackArea;
    [SerializeField]
    GameObject m_fxHitPrefab;
    Camera m_mainCamera;
    Vector3 m_dir;
    Vector3 m_clickDir;
    Vector3 m_clickPos;
    [SerializeField]
    float m_speed = 4f;
    float m_scale;
    public PlayerAnimController.Motion GetMotion { get { return m_animCtr.GetCurrentMotion(); } }
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    List<PlayerAnimController.Motion> m_comboList = new List<PlayerAnimController.Motion>() { PlayerAnimController.Motion.Attack1, PlayerAnimController.Motion.Attack2, PlayerAnimController.Motion.Attack3, PlayerAnimController.Motion.Attack4, };
    Dictionary<PlayerAnimController.Motion, SkillData> m_skillTable = new Dictionary<PlayerAnimController.Motion, SkillData>();
    int m_comboIndex;
    public bool IsAttack
    {
        get
        {
            if (GetMotion == PlayerAnimController.Motion.Attack1 ||
                GetMotion == PlayerAnimController.Motion.Attack2 ||
                GetMotion == PlayerAnimController.Motion.Attack3 ||
                GetMotion == PlayerAnimController.Motion.Attack4)
            {
                return true;
            }
            return false;
        }
    }

    #region Unity Animation Event Methods
    void AnimEvent_Attack()
    {
        SkillData skillData;
        if (!m_skillTable.TryGetValue(GetMotion, out skillData))
            return;
        var unitList = m_attackArea[skillData.attackArea].m_unitList;
        for (int i = 0; i < unitList.Count; i++)
        {
            var enemy = unitList[i].GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.SetDamage(skillData);
                var obj = Instantiate(m_fxHitPrefab);
                obj.transform.position = unitList[i].transform.position + Vector3.up * 0.6f;
                var dir = (transform.position - unitList[i].transform.position);
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
            }
        }
    }
    void AnimEvent_AttackFinished()
    {
        bool isCombo = false;
        if (m_keyBuffer.Count > 0)
        {
            if (m_keyBuffer.Count > 1)
            {
                ResetKeyBuffer();
                isCombo = false;
            }
            else
            {
                var key = m_keyBuffer.Dequeue();
                isCombo = true;
            }
        }
        if (isCombo)
        {
            m_comboIndex++;
            if(m_comboIndex >= m_comboList.Count)
            {
                m_comboIndex = 0;
            }
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            m_comboIndex = 0;
            m_animCtr.Play(PlayerAnimController.Motion.Idle);
        }
    }
    #endregion

    Vector3? GetClickPosition()
    {
        Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("BackGround")))
        {
            return hit.point;
        }
        return null;
    }
    
    void ResetKeyBuffer()
    {
        m_keyBuffer.Clear();
    }
    void ActionControll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_navAgent.ResetPath();
            if (GetMotion == PlayerAnimController.Motion.Idle ||
                GetMotion == PlayerAnimController.Motion.Locomotion)
            {
                m_animCtr.Play(PlayerAnimController.Motion.Attack1);
            }
            else
            {
                m_keyBuffer.Enqueue(KeyCode.Space);
                if (IsInvoking("ResetKeyBuffer"))
                {
                    CancelInvoke("ResetKeyBuffer");
                }
                Invoke("ResetKeyBuffer", 0.4f);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {

        }
        m_dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (Input.GetMouseButtonDown(0))
        {
            var result = GetClickPosition();
            if(result != null)
            {
                m_clickPos = result.Value;
                m_clickDir = result.Value - transform.position;
                m_clickDir.y = 0f;
                m_navAgent.SetDestination(result.Value);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {

        }
        if(m_dir != Vector3.zero)
        {
            m_navAgent.ResetPath();
            transform.forward = m_dir;
        }
        if(m_clickDir != Vector3.zero)
        {
            m_dir = m_clickDir.normalized;
        }
        if(m_dir != Vector3.zero && !IsAttack)
        {
            if (GetMotion != PlayerAnimController.Motion.Locomotion)
                m_animCtr.SetMotion(PlayerAnimController.Motion.Locomotion);
            if(m_navAgent.hasPath)
                transform.forward = m_dir;
            m_scale += Time.deltaTime;
            if (m_scale > 1f)
            {
                m_scale = 1f;
            }
        }
        else
        {

            if (GetMotion == PlayerAnimController.Motion.Locomotion)
                m_animCtr.SetMotion(PlayerAnimController.Motion.Idle);
            if (m_scale > 0f)
            {
                m_scale -= Time.deltaTime;
                if (m_scale < 0f) m_scale = 0f;
            }        
        }
        if(m_clickDir != Vector3.zero)
        {
            var dist = Vector3.Distance(transform.position, m_clickPos);
            //Debug.Log(dist);
            if(Mathf.Approximately(dist, 0.25f) || (dist < 0.25f))
            {
                m_clickPos = Vector3.zero;
                m_clickDir = Vector3.zero;
                m_dir = Vector3.zero;
            }
        }
        m_animCtr.SetFloat("Speed", m_scale);
        //transform.position += m_dir * m_speed * m_scale * Time.deltaTime;
        if(m_charCtr.enabled)
            m_charCtr.Move(m_dir * m_speed * m_scale * Time.deltaTime);
        else if(m_dir != Vector3.zero && m_clickDir == Vector3.zero)
            m_navAgent.Move(m_dir * m_speed * m_scale * Time.deltaTime);
    }
    void InitSkillTable()
    {
        m_skillTable.Add(PlayerAnimController.Motion.Attack1, new SkillData() { attackArea = 0, knockbackDist = 0.3f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack2, new SkillData() { attackArea = 0, knockbackDist = 0.5f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack3, new SkillData() { attackArea = 1, knockbackDist = 1f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack4, new SkillData() { attackArea = 2, knockbackDist = 2f });
    }
    // Start is called before the first frame update
    void Start()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
        m_charCtr = GetComponent<CharacterController>();
        m_mainCamera = Camera.main;
        m_animCtr = GetComponent<PlayerAnimController>();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreaUnitFind>();
        InitSkillTable();
    }

    // Update is called once per frame
    void Update()
    {
        ActionControll();       
    }
}
