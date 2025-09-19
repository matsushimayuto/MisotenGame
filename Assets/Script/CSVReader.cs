using UnityEngine;

public class StageLoader2D : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject BlockPrefab;

    void Start()
    {
        LoadStage("csvName"); // ここに読み込むcsvデータの名前を入れる
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

        // 上から下に読み込む（行）
        for (int y = 0; y < lines.Length; y++)
        {
            if (string.IsNullOrWhiteSpace(lines[y])) continue;

            string[] values = lines[y].Split(',');

            // 左から右に読み込む（列）
            for (int x = 0; x < values.Length; x++)
            {
                int cell;
                if (!int.TryParse(values[x], out cell)) continue;

                Vector3 pos = new Vector3(x, -y, 0);
                // -y にしてるのは、CSV上の上の行をUnity上で上に表示するため

                switch (cell)
                {
                    case 1: // プレイヤー
                        Instantiate(PlayerPrefab, pos, Quaternion.identity);
                        break;

                    case 2: // エネミー
                        Instantiate(EnemyPrefab, pos, Quaternion.identity);
                        break;

                    case 3: // ブロック
                        Instantiate(BlockPrefab, pos, Quaternion.identity);
                        break;

                        // case 3: 敵, case 4: アイテム… なども追加可能
                }
            }
        }
    }
}
