using UnityEngine;

// 3Dスペースに線を描画するためのコンポーネント
[RequireComponent(typeof(LineRenderer))]
public class GridDraw : MonoBehaviour
{

    public int gridSize = 10;          // グリッドのマス数
    public float cellSize = 1f;        // 1マスの大きさ
    public Color lineColor = Color.black;   // グリッド線の色

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        // グリッド全体の半分を原点からマイナスにずらす
        float halfSize = gridSize * cellSize * 0.5f;

        // 縦線
        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = x * cellSize - halfSize;
            CreateLine(
                new Vector3(xPos, 0, -halfSize),
                new Vector3(xPos, 0, halfSize)
            );
        }

        // 横線
        for (int z = 0; z <= gridSize; z++)
        {
            float zPos = z * cellSize - halfSize;
            CreateLine(
                new Vector3(-halfSize, 0, zPos),
                new Vector3(halfSize, 0, zPos)
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        start.y += 0.015f;
        end.y   += 0.015f;
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
}