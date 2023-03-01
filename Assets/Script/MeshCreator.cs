using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [SerializeField]
    Texture2D m_texture;
    Vector3[] m_vectices =  {
        new Vector3(-1f, 1f, 0f), new Vector3(1f, 1f, 0f),
        new Vector3(1f, -1f, 0f), new Vector3(-1f, -1f, 0f),
    };
    int[] m_triangles = {
        0, 1, 2,
        0, 2, 3
    };
    Mesh m_mesh;
    Material m_metererial;
    Vector2[] m_uvs =
    {
        new Vector2(0f, 1f), new Vector2(1f, 1f),
        new Vector2(1, 0f), new Vector2(0, 0f)
    };
    // Start is called before the first frame update
    void Start()
    {
        m_mesh = new Mesh();
        m_mesh.vertices = m_vectices;
        m_mesh.triangles = m_triangles;
        m_mesh.name = "Rectangle";
        m_mesh.uv = m_uvs;
        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
        var mFilter = gameObject.AddComponent<MeshFilter>();
        mFilter.mesh = m_mesh;
        var renderer = gameObject.AddComponent<MeshRenderer>();
        m_metererial = new Material(Shader.Find("Standard"));
        m_metererial.SetTexture("Albedo", m_texture);
        renderer.material = m_metererial;
    }

    // Update is called once per frame
    void Update()
    {

    }
}