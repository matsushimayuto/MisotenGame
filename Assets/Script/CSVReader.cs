using UnityEngine;
using System;

public class StageLoader2D : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject WallPrefab;
    public GameObject EnemyPrefab;
    public GameObject BlockPrefab;
    public GameObject MoveBlockPrefab1;
    public GameObject MoveBlockPrefab2;

    [SerializeField] private float cellSize = 1f; // 1マスのサイズ（任意で調整可能）

    void Start()
    {
        LoadStage("Stage1/Stage1-1"); // 読み込むCSVファイル名（拡張子なし）
    }

    void LoadStage(string stageName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(stageName);
        if (csvFile == null)
        {
            Debug.LogError($"ステージファイル {stageName}.csv が見つかりません");
            return;
        }

        // 改行コードと空行の扱いを正しくする
        string[] lines = csvFile.text
            .Replace("\r", "")
            .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int height = lines.Length;
        int width = lines[0].Split(',').Length;

        float offsetX = -(width - 1) * cellSize / 2f;
        float offsetZ = -(height - 1) * cellSize / 2f;

        for (int y = 0; y < height; y++)
        {
            string line = lines[y].Trim();  // ← 重要
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (!int.TryParse(values[x], out int cell)) continue;

                Vector3 pos = new Vector3(
                    offsetX + x * cellSize,
                    0f,
                    offsetZ + (height - 1 - y) * cellSize
                );

                switch (cell)
                {
                    // プレイヤー
                    case 1: Instantiate(PlayerPrefab, pos, Quaternion.identity); break;

                    // 外壁
                    case 2: Instantiate(WallPrefab, pos, Quaternion.identity); break;

                    // 敵
                    case 100: Instantiate(EnemyPrefab, pos, Quaternion.identity); break;

                    // 動かないブロック
                    case 200: Instantiate(BlockPrefab, pos, Quaternion.identity); break;

                    // 動くブロック（3*1）縦
                    case 201: Instantiate(MoveBlockPrefab1, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動くブロック（3*1）横
                    case 202: Instantiate(MoveBlockPrefab2, pos, Quaternion.Euler(0,180, 0)); break;
                }
            }
        }
    }

}
