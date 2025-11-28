using UnityEngine;
using System;

public class StageLoader2D : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject WallPrefab;
    public GameObject EnemyPrefab;
    public GameObject BlockPrefab1;
    public GameObject BlockPrefab2;
    public GameObject BlockPrefab3;
    public GameObject BlockPrefab4;
    public GameObject BlockPrefab5;
    public GameObject BlockPrefab6;
    public GameObject MoveBlockPrefab1;
    public GameObject MoveBlockPrefab2;
    public GameObject MoveBlockPrefab3;
    public GameObject MoveBlockPrefab4;
    public GameObject MoveBlockPrefab5;

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

                    // 動かないブロック（3*1）縦
                    case 200: Instantiate(BlockPrefab1, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動かないブロック（3*1）横
                    case 201: Instantiate(BlockPrefab2, pos, Quaternion.Euler(0, 180, 0)); break;

                    // 動かないブロック（2*1）縦
                    case 202: Instantiate(BlockPrefab3, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動かないブロック（2*1）横
                    case 203: Instantiate(BlockPrefab4, pos, Quaternion.Euler(0, 180, 0)); break;

                    // 動かないブロック（2*2）
                    case 204: Instantiate(BlockPrefab5, pos, Quaternion.identity); break;
                    
                    // 動かないブロック（1*1）
                    case 205: Instantiate(BlockPrefab6, pos, Quaternion.identity); break;

                    // 動くブロック（3*1）縦
                    case 300: Instantiate(MoveBlockPrefab1, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動くブロック（3*1）横
                    case 301: Instantiate(MoveBlockPrefab2, pos, Quaternion.Euler(0, 180, 0)); break;

                    // 動くブロック（2*1）縦
                    case 302: Instantiate(MoveBlockPrefab3, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動くブロック（2*1）横
                    case 303: Instantiate(MoveBlockPrefab4, pos, Quaternion.Euler(0, 180, 0)); break;

                    // 動くブロック（1*1）横
                    case 304: Instantiate(MoveBlockPrefab5, pos, Quaternion.identity); break;
                }
            }
        }
    }

}
