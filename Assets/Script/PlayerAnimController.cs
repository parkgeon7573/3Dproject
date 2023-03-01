using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class PlayerAnimController : AnimationController
{
    public enum Motion
    {
        Idle,
        Locomotion,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
    }
    Motion m_motion; 
    public Motion GetCurrentMotion()
    {
        return m_motion;
    }
    public void SetMotion(Motion motion)
    {
        m_motion = motion;
    }
    StringBuilder m_sb = new StringBuilder();
    public void Play(Motion motion, bool isBlend = true)
    {
        m_motion = motion;
        m_sb.Append(motion);
        Play(m_sb.ToString(), isBlend);
        m_sb.Clear();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
}
