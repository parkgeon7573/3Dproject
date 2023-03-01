using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public WayPoint[] m_waypoints;
    [SerializeField]
    Color m_color = Color.yellow;
    private void OnDrawGizmos()
    {
        m_waypoints = GetComponentsInChildren<WayPoint>();

        for (int i = 0; i < m_waypoints.Length - 1; i++)
        {
            m_waypoints[i].m_color = m_color;
            Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
        }
        m_waypoints[m_waypoints.Length - 1].m_color = m_color;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
