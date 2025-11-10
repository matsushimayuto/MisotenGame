using UnityEngine;

public class StageLoader2D : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject MoveBlockPrefab;
    public GameObject BlockPrefab;
    public GameObject WallPrefab;


    [SerializeField] private float cellSize = 1f; // 1マスのサイズ（任意で調整可能）

    void Start()
    {
        LoadStage("test"); // 読み込むCSVファイル名（拡張子なし）
    }

    void LoadStage(string stageName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(stageName);
        if (csvFile == null)
        {
            Debug.LogError($"ステージファイル {stageName}.csv が見つかりません");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        int height = lines.Length;
        int width = lines[0].Split(',').Length;

        // ステージを中央に配置するためのオフセット
        float offsetX = -(width - 1) * cellSize / 2f;
        float offsetZ = -(height - 1) * cellSize / 2f;

        // 上から下、左から右に配置
        for (int y = 0; y < height; y++)
        {
            if (string.IsNullOrWhiteSpace(lines[y])) continue;

            string[] values = lines[y].Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (!int.TryParse(values[x], out int cell)) continue;

                // Z軸を使って「上から下」方向に並べる
                Vector3 pos = new Vector3(
                    offsetX + x * cellSize,
                    0f, // 高さ（固定）
                    offsetZ + (height - 1 - y) * cellSize // CSVの上の行を奥(Z+)に配置
                );

                GameObject prefab = null;

                switch (cell)
                {
                    case 1: // プレイヤー
                        Instantiate(PlayerPrefab, pos, Quaternion.identity);
                        break;

                    case 2: // 敵
                        Instantiate(EnemyPrefab, pos, Quaternion.identity);
                        break;

                    case 3: // 外壁
                        Instantiate(WallPrefab, pos, Quaternion.identity);
                        break;

                    case 4: // 動かないブロック
                        Instantiate(BlockPrefab, pos, Quaternion.identity);
                        break;

                    case 5: // 動くブロック(3×1)
                        Instantiate(MoveBlockPrefab, pos, Quaternion.identity);
                        break;
                }

                if (prefab != null)
                {
                    Instantiate(prefab, pos, Quaternion.identity);
                }
            }
        }
    }
}
