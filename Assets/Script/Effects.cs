using UnityEngine;

public class Effects : MonoBehaviour
{
    // Inspectorで設定できるようにする（エフェクトのPrefabを指定）
    [SerializeField] private GameObject effectPrefab;

    // エフェクトの生成位置
    [SerializeField] private Transform spawnPoint;

    //Effectsのサイズ設定
    [SerializeField] private float effectScale;

    // Updateは毎フレーム呼ばれる
    void Update()
    {
        // 例：スペースキーを押すとエフェクトを再生
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEffect();
        }
    }

    // エフェクト生成関数
    private void SpawnEffect()
    {
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;

        // エフェクトを生成
        GameObject effect = Instantiate(effectPrefab, pos, Quaternion.identity);

        // Inspectorで設定したサイズを適用
        effect.transform.localScale = Vector3.one * effectScale;

        // 子を含むすべてのParticleSystemにスケールを適用
        ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in systems)
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Local; // スケーリングをTransform依存にする
            ps.transform.localScale = Vector3.one * effectScale; // 個別にスケールを適用
        }
    }
}
