using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaUnitFind : MonoBehaviour
{
    public List<GameObject> m_unitList = new List<GameObject>();
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
