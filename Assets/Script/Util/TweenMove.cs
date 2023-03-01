using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMove : MonoBehaviour
{
    [SerializeField]
    AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    float m_duration = 1f;
    float m_time;
    [SerializeField]
    Vector3 m_from;
    [SerializeField]
    Vector3 m_to;
    bool m_isStart;


    public void Play()
    {
        m_isStart = true;
        m_time = 0f;
    }
    public void Play(Vector3 from, Vector3 to, float duration)
    {
        m_from = from;
        m_to = to;
        m_duration = duration;
        m_isStart = true;
        m_time = 0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isStart)
        {
            var value = m_curve.Evaluate(m_time);           
            transform.position = m_from * (1f - value) + m_to * value;
            m_time += Time.deltaTime / m_duration;
            if (m_time > 1)
            {
                m_isStart = false;
                m_time = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Play(transform.position, transform.position + Vector3.back * 3f, 0.7f);
        }
    }
}
