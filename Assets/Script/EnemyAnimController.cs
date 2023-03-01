using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimController : AnimationController
{
    public enum Motion
    {
        Idle,
        Run,
        Attack1,
        Hit,
        Max
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
