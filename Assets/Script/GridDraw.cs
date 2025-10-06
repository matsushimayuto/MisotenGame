using UnityEngine;

// 3Dスペースに線を描画するためのコンポーネント
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class GridDraw : MonoBehaviour
{

    public int gridSize = 10;          // グリッドのマス数
    public float cellSize = 1f;        // 1マスの大きさ
    public Color lineColor = Color.black;   // グリッド線の色

    public bool IsDrawnRuntimeGrid = true;
    public bool IsOnDrawGizmos = true;

    void Start()
    {
        if (Application.isPlaying)
        {
            DrawRuntimeGrid();
        }
    }

    // シーンビューではGizmosで描画
    void OnDrawGizmos()
    {
        if (!IsOnDrawGizmos) { return; }
        Gizmos.color = lineColor;

        float halfSize = gridSize * cellSize * 0.5f;

        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = x * cellSize - halfSize;
            Vector3 start = new Vector3(xPos, 0.01f, -halfSize);
            Vector3 end = new Vector3(xPos, 0.01f, halfSize);
            start.y += 0.015f;
            end.y += 0.015f;
            Gizmos.DrawLine(start, end);
        }

        for (int z = 0; z <= gridSize; z++)
        {
            float zPos = z * cellSize - halfSize;
            Vector3 start = new Vector3(-halfSize, 0.01f, zPos);
            Vector3 end = new Vector3(halfSize, 0.01f, zPos);
            start.y += 0.015f;
            end.y += 0.015f;
            Gizmos.DrawLine(start, end);
        }
    }

    // ゲーム実行時：LineRendererを使って描画
    void DrawRuntimeGrid()
    {
        if (!IsDrawnRuntimeGrid) return;
        IsDrawnRuntimeGrid = true;

        float halfSize = gridSize * cellSize * 0.5f;

        // 縦線
        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = x * cellSize - halfSize;
            CreateLine(
                new Vector3(xPos, 0.01f, -halfSize),
                new Vector3(xPos, 0.01f, halfSize)
            );
        }

        // 横線
        for (int z = 0; z <= gridSize; z++)
        {
            float zPos = z * cellSize - halfSize;
            CreateLine(
                new Vector3(-halfSize, 0.01f, zPos),
                new Vector3(halfSize, 0.01f, zPos)
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        start.y += 0.015f;
        end.y += 0.015f;
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = this.transform;

        var lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = lineColor;
    }

    // シーン内で値変更時に自動リフレッシュ
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // エディタで再生前にオブジェクトをクリーンに
            foreach (Transform child in transform)
            {
                if (child.name.Contains("GridLine"))
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}