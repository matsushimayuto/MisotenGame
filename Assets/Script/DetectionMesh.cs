using UnityEngine;

public class DetectionMesh : MonoBehaviour
{
    [SerializeField, Tooltip("視野角")] private float viewAngle = 90.0f;               // 視野角
    [SerializeField, Tooltip("視認距離")] private float viewDistance = 10.0f;          // 視認距離

    [Header("索敵範囲可視化")]
    [SerializeField] private Material detectionMat;  // 半透明表示用マテリアル
    private Mesh detectionMesh;
    private GameObject detectionObj;

    public bool detected { get; set; }

    void Start()
    {
        Transform body = transform.Find("group/Armature/Body");
        // 索敵範囲の子オブジェクトを作成
        detectionObj = new GameObject("DetectionRange");
        detectionObj.transform.parent = transform;
        detectionObj.transform.localPosition = Vector3.zero;

        // マテリアル設定（指定がなければ Unlit を使用）
        MeshFilter mf = detectionObj.AddComponent<MeshFilter>();
        MeshRenderer mr = detectionObj.AddComponent<MeshRenderer>();
        mr.material = detectionMat != null
            ? detectionMat
            : new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        detectionMesh = new Mesh();
        mf.mesh = detectionMesh;

        // メッシュを初期生成
        UpdateDetectionMesh();
    }

    // Update is called once per frame
    void Update()
    {
        // 発見したら視界色を変える
        detectionObj.GetComponent<MeshRenderer>().material.color =
        detected ? new Color(1, 0, 0, 0.3f) : new Color(1, 1, 0, 0.2f);

        // 毎フレーム視界更新
        UpdateDetectionMesh();
    }

    //索敵範囲（扇形メシュ）の生成処理
    private void UpdateDetectionMesh()
    {
        Transform body = transform.Find("group/Armature/Body");

        int segments = 100;
        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        // 中心点（ローカル、y固定）
        vertices[0] = new Vector3(0f, 0.05f, 0f);

        float halfAngle = viewAngle * 0.5f;
        float startAngle = -halfAngle;
        float angleStep = viewAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i + 90f;

            // Bodyの向きを基準に扇形を回転
            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);
            Vector3 forward = body.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 dir = Quaternion.AngleAxis(currentAngle, Vector3.up) * forward;

            Vector3 rayOrigin = body.position + Vector3.up * 0.05f;

            Vector3 localPoint;

            if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, viewDistance))
            {
                localPoint = detectionObj.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                Vector3 worldPoint = rayOrigin + dir * viewDistance;
                localPoint = detectionObj.transform.InverseTransformPoint(worldPoint);
            }

            // yを固定
            localPoint.y = 0.05f;
            vertices[i + 1] = localPoint;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        detectionMesh.Clear();
        detectionMesh.vertices = vertices;
        detectionMesh.triangles = triangles;
        detectionMesh.RecalculateNormals();
    }
}
