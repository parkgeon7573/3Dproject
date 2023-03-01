using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator m_animator;
    public void SetFloat(string trigger, float value)
    {
        m_animator.SetFloat(trigger, value);
    }
    public void Play(string animName, bool isBlend)
    {
        if (isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_animator = GetComponent<Animator>();
    }
}
