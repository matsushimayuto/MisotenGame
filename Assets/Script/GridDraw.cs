using UnityEngine;

// 縦横・位置・太さを調整できるグリッド描画クラス
[ExecuteAlways]
public class GridDrawRect : MonoBehaviour
{
    [Header("グリッド設定")]
    public int gridWidth = 15;       // 横マス数（X方向）
    public int gridHeight = 11;      // 縦マス数（Z方向）
    public float cellSize = 1f;      // 1マスの大きさ
    public Vector3 gridOrigin = Vector3.zero; // グリッド中心位置
    public Color lineColor = Color.black;
    [Range(0.001f, 0.5f)] public float lineWidth = 0.05f; // グリッド線の太さ

    [Header("描画設定")]
    public bool drawInEditor = true;     // シーンビュー描画
    public bool drawAtRuntime = true;    // 実行時描画

    private Material lineMat;

    void Start()
    {
        if (Application.isPlaying && drawAtRuntime)
        {
            DrawRuntimeGrid();
        }
    }

    void OnDrawGizmos()
    {
        if (!drawInEditor) return;

        Gizmos.color = lineColor;
        float halfW = gridWidth * cellSize * 0.5f;
        float halfH = gridHeight * cellSize * 0.5f;

        // Gizmosには線の太さを直接反映できないため、近似的に描く場合はHandlesを使用する。
#if UNITY_EDITOR
        UnityEditor.Handles.color = lineColor;
        for (int x = 0; x <= gridWidth; x++)
        {
            float xPos = gridOrigin.x + (x * cellSize - halfW);
            Vector3 start = new Vector3(xPos, gridOrigin.y + 0.01f, gridOrigin.z - halfH);
            Vector3 end = new Vector3(xPos, gridOrigin.y + 0.01f, gridOrigin.z + halfH);
            UnityEditor.Handles.DrawAAPolyLine(lineWidth * 20f, start, end);
        }

        for (int z = 0; z <= gridHeight; z++)
        {
            float zPos = gridOrigin.z + (z * cellSize - halfH);
            Vector3 start = new Vector3(gridOrigin.x - halfW, gridOrigin.y + 0.01f, zPos);
            Vector3 end = new Vector3(gridOrigin.x + halfW, gridOrigin.y + 0.01f, zPos);
            UnityEditor.Handles.DrawAAPolyLine(lineWidth * 20f, start, end);
        }
#endif
    }

    void DrawRuntimeGrid()
    {
        // 既存ライン削除
        foreach (Transform child in transform)
        {
            if (child.name.Contains("GridLine"))
                Destroy(child.gameObject);
        }

        if (lineMat == null)
            lineMat = new Material(Shader.Find("Sprites/Default"));

        float halfW = gridWidth * cellSize * 0.5f;
        float halfH = gridHeight * cellSize * 0.5f;

        // 縦線
        for (int x = 0; x <= gridWidth; x++)
        {
            float xPos = gridOrigin.x + (x * cellSize - halfW);
            CreateLine(
                new Vector3(xPos, gridOrigin.y + 0.01f, gridOrigin.z - halfH),
                new Vector3(xPos, gridOrigin.y + 0.01f, gridOrigin.z + halfH)
            );
        }

        // 横線
        for (int z = 0; z <= gridHeight; z++)
        {
            float zPos = gridOrigin.z + (z * cellSize - halfH);
            CreateLine(
                new Vector3(gridOrigin.x - halfW, gridOrigin.y + 0.01f, zPos),
                new Vector3(gridOrigin.x + halfW, gridOrigin.y + 0.01f, zPos)
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = this.transform;

        var lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = lineWidth;
        lr.material = lineMat;
        lr.startColor = lr.endColor = lineColor;
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("GridLine"))
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
