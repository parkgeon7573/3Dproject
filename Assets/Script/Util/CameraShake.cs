using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : SingletonMonoBehaviour<CameraShake>
{
    [SerializeField]
    [Range(0.01f, 0.51f)]
    float m_power = 0.1f;
    [SerializeField]
    float m_duration = 1f;
    float m_time;
    Vector3 m_orgPos;
    bool m_isStart;

    public void Shake()
    {
        m_isStart = true;
        m_time = 0f;
    }
    public void Shake(float power, float duration)
    {
        m_power = power;
        m_duration = duration;
        m_isStart = true;
        m_time = 0f;
    }
    // Start is called before the first frame update
    protected override void OnStart()
    {
        m_orgPos = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_isStart)
        {
            Vector3 dir = Random.insideUnitCircle;
            dir.z = 0f;
            transform.position = m_orgPos + dir * m_power;
            m_time += Time.deltaTime;
            if (m_time > m_duration)
            {
                transform.position = m_orgPos;
                m_isStart = false;
                m_time = 0f;
            }
        }
    }
}