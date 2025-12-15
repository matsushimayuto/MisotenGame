using UnityEngine;
using System;

public class StageLoader2D : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject WallPrefab;
    public GameObject WidthWallPrefab;
    public GameObject HeightWallPrefab;
    public GameObject WarpWallAPrefab1;
    public GameObject WarpWallBPrefab1;
    public GameObject WarpWallAPrefab2;
    public GameObject WarpWallBPrefab2;
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
    [SerializeField] private bool LoadCSV;
    [SerializeField] private TextAsset StageCSV; // 読み込むcsvファイル

    private const string StageRootPath = "Stages/";
    

    // WarpA / WarpB を保持しておく変数
    private GameObject WarpA1;
    private GameObject WarpB1;
    private GameObject WarpA2;
    private GameObject WarpB2;

    void Start()
    {
        LoadStage(StageManager.Instance.GetCurrentWorld(), StageManager.Instance.GetCurrentStage());
    }

    // 外部から呼び出すステージロード
    // 例: LoadStage(2, 1); // Group2 → Level1 → ステージ2-1
    public void LoadStage(int group, int level)
    {
        TextAsset csvFile;
        if (LoadCSV)
        {
            csvFile = StageCSV;
        }
        else
        {
            string path = $"{StageRootPath}Stage{group}/Stage_G{group}_L{level}";
            csvFile = Resources.Load<TextAsset>(path);
        }

        if (csvFile == null)
        {
            Debug.LogError($"ステージファイル {csvFile}.csv が見つかりません");
            return;
        }

        LoadStageInternal(csvFile);
    }

    public void LoadStageInternal(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            Debug.LogError($"ステージファイル {csvFile}.csv が見つかりません");
            return;
        }

        // 前回の Warp をリセット
        WarpA1 = null;
        WarpB1 = null;
        WarpA2 = null;
        WarpB2 = null;

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

                Vector3 adjust = Vector3.zero;

                switch (cell)
                {
                    // プレイヤー
                    case 1: Instantiate(PlayerPrefab, pos, Quaternion.identity); break;

                    // 外壁
                    case 2: Instantiate(WallPrefab, pos, Quaternion.identity); break;

                    // 外壁(横)
                    case 3: Instantiate(WidthWallPrefab, pos, Quaternion.identity); break;

                    // 外壁(縦)
                    case 4: Instantiate(HeightWallPrefab, pos, Quaternion.identity); break;

                    // ワープ（左配置用）
                    case 5:
                        WarpA1 = Instantiate(WarpWallAPrefab1, pos, Quaternion.Euler(0, 90, 0));
                        break;

                    // ワープ（右配置用）
                    case 6:
                        WarpB1 = Instantiate(WarpWallBPrefab1, pos, Quaternion.Euler(0, 270, 0));
                        break;

                    // ワープ（上配置用）
                    case 7:
                        WarpA2 = Instantiate(WarpWallAPrefab2, pos, Quaternion.Euler(0, 180, 0));
                        break;

                    // ワープ（下配置用）
                    case 8:
                        WarpB2 = Instantiate(WarpWallBPrefab2, pos, Quaternion.Euler(0, 0, 0));
                        break;

                    // 敵
                    case 100: Instantiate(EnemyPrefab, pos, Quaternion.identity); break;

                    // 動かないブロック（3*1）縦
                    case 200: Instantiate(BlockPrefab1, pos, Quaternion.identity); break;

                    // 動かないブロック（3*1）横
                    case 201: Instantiate(BlockPrefab2, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動かないブロック（2*1）縦
                    case 202:
                        adjust = new Vector3(0, 0, -cellSize * 0.5f);
                        Instantiate(BlockPrefab3, pos + adjust, Quaternion.Euler(0, 90, 0));
                        break;

                    // 動かないブロック（2*1）横
                    case 203:
                        adjust = new Vector3(cellSize * 0.5f, 0, 0);
                        Instantiate(BlockPrefab4, pos + adjust, Quaternion.identity);
                        break;

                    // 動かないブロック（2*2）
                    case 204:
                        adjust = new Vector3(cellSize * 0.5f, 0, -cellSize * 0.5f);
                        Instantiate(BlockPrefab5, pos + adjust, Quaternion.identity);
                        break;

                    // 動かないブロック（1*1）
                    case 205: Instantiate(BlockPrefab6, pos, Quaternion.identity); break;

                    // 動くブロック（3*1）縦
                    case 300: Instantiate(MoveBlockPrefab1, pos, Quaternion.identity); break;

                    // 動くブロック（3*1）横
                    case 301: Instantiate(MoveBlockPrefab2, pos, Quaternion.Euler(0, 90, 0)); break;

                    // 動くブロック（2*1）縦
                    case 302:
                        adjust = new Vector3(0, 0, -cellSize * 0.5f);
                        Instantiate(MoveBlockPrefab3, pos + adjust, Quaternion.identity);
                        break;
                    // 動くブロック（2*1）横
                    case 303:
                        adjust = new Vector3(cellSize * 0.5f, 0, 0);
                        Instantiate(MoveBlockPrefab4, pos + adjust, Quaternion.Euler(0, 90, 0));
                        break;
                    // 動くブロック（1*1）横
                    case 304: Instantiate(MoveBlockPrefab5, pos, Quaternion.identity); break;
                }
            }
        }

        // WarpAとWarpBの接続
        if (WarpA1 != null && WarpB1 != null)
        {
            TeleportMirror tA1 = WarpA1.GetComponent<TeleportMirror>();
            TeleportMirror tB1 = WarpB1.GetComponent<TeleportMirror>();

            tA1.teleportExit = WarpB1.transform;
            tB1.teleportExit = WarpA1.transform;
        }
        else
        {
            Debug.LogWarning("WarpA1かWarpB1がどちらか不足しています。リンクされませんでした。");
        }
        // WarpAとWarpBの接続
        if (WarpA2 != null && WarpB2 != null)
        {
            TeleportMirror tA2 = WarpA2.GetComponent<TeleportMirror>();
            TeleportMirror tB2 = WarpB2.GetComponent<TeleportMirror>();

            tA2.teleportExit = WarpB2.transform;
            tB2.teleportExit = WarpA2.transform;
        }
        else
        {
            Debug.LogWarning("WarpA2かWarpB2がどちらか不足しています。リンクされませんでした。");
        }
    }

}
