using UnityEngine;

[RequireComponent(typeof(MeshFilter))] 
[RequireComponent(typeof(MeshRenderer))] //두줄
public class SectorMeshCreator : MonoBehaviour
{
    //상수로 만들지 판단
    //캡슐화 변경
    private const float _viewAngle = 90f;
    private const float _detectRadius = 6f;
    private const int _segments = 20;

    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private void Awake()
    {
        if( _mesh == null)
        {
            _meshFilter = GetComponent<MeshFilter>(); //널체크
            _mesh = new Mesh { name = "SectorMesh" };
            _meshFilter.mesh = _mesh;
        }
        

        CreateSectorMesh();
    }

    //이거 안쓰고있음 참고바람
    public void UpdateMeshSettings(float angle, float radius)
    {
        CreateSectorMesh();
    }

    private void CreateSectorMesh()
    {
        if (_mesh == null) return;

        _mesh.Clear();

        int vertexCount = _segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[_segments * 3];

        vertices[0] = Vector3.zero;

        float startAngle = -_viewAngle * 0.5f;
        float angleStep = _viewAngle / _segments;

        for (int i = 0; i <= _segments; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            float rad = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * _detectRadius;
            float z = Mathf.Cos(rad) * _detectRadius;

            vertices[i + 1] = new Vector3(x, 0f, z);
        }

        //힌트만 드리자면 for문 추가면되는데 2중 for문은 권장하지 않는다 (둬도될듯?)
        for (int i = 0; i < _segments; i++)
        {
            triangles[i * 3] = 0;         
            triangles[i * 3 + 1] = i + 1; 
            triangles[i * 3 + 2] = i + 2; 
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}