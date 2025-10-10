using System.IO;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    [System.Serializable]
    class StageData
    {
        public float playerPoint;  // 最後にクリアしたステージの場所のデータ
        public bool[] ClearFlag = new bool[4];
    }

    public void Save()
    {
        StageData stageData = new StageData();
        // 保存したいデータの値を入れる
        stageData.playerPoint = 100f;
        for(int i = 0; i < 4; i++)
            stageData.ClearFlag[i] = true;

        string json = JsonUtility.ToJson(stageData, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        Debug.Log(Application.persistentDataPath);
    }

    public void DataLoad()
    {
        string path = Application.persistentDataPath + "/save.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageData stageData = JsonUtility.FromJson<StageData>(json);

            Debug.Log(stageData.playerPoint);

            for (int i = 0; i < 4; i++)
                Debug.Log("ステージ" + i + stageData.ClearFlag[i]);
        }
        else
        {
            Debug.Log("セーブデータがありません");
        }

    }


}
