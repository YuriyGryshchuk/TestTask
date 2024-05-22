using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class RandomMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private  float _randomRange = 0.1f; 

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private bool _isCashed;
    private Mesh _mesh;


    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        _isCashed = true;
    }

    private async void OnEnable()
    {
        await UniTask.WaitWhile(() => !_isCashed);

        _mesh = new Mesh();

        GenerateRandomCube(_mesh);

        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }

    void GenerateRandomCube(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f), // 0
            new Vector3( 0.5f, -0.5f, -0.5f), // 1
            new Vector3( 0.5f,  0.5f, -0.5f), // 2
            new Vector3(-0.5f,  0.5f, -0.5f), // 3
            new Vector3(-0.5f, -0.5f,  0.5f), // 4
            new Vector3( 0.5f, -0.5f,  0.5f), // 5
            new Vector3( 0.5f,  0.5f,  0.5f), // 6
            new Vector3(-0.5f,  0.5f,  0.5f)  // 7
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(
                Random.Range(-_randomRange, _randomRange),
                Random.Range(-_randomRange, _randomRange),
                Random.Range(-_randomRange, _randomRange)
            );
        }

        // Определение треугольников
        int[] triangles = new int[]
        {
            // Front face
            0, 2, 1,
            0, 3, 2,
            // Back face
            4, 5, 6,
            4, 6, 7,
            // Top face
            3, 7, 6,
            3, 6, 2,
            // Bottom face
            0, 1, 5,
            0, 5, 4,
            // Left face
            0, 4, 7,
            0, 7, 3,
            // Right face
            1, 2, 6,
            1, 6, 5
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}    
