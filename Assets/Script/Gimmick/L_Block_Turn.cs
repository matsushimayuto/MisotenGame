using UnityEngine;

public class L_Block_Turn : MonoBehaviour
{
    [SerializeField] private float rotateInterval = 3.0f;   // ブロックの切り替え間隔（秒）
    [SerializeField] private GameObject[] Block;             // ブロックの配列
    [SerializeField] private Material transparentMaterial;   // 非表示状態のマテリアル
    [SerializeField] private Material indicationMaterial;    // 表示状態のマテリアル

    private int currentBlockIndex = 0;                       // 現在アクティブなブロックのインデックス

    private void Start()
    {
        // 一定間隔で SwitchBlocks() を呼び出して、ブロックの表示状態を切り替える
        InvokeRepeating(nameof(SwitchBlocks), 0.0f, rotateInterval);
    }

    /// <summary>
    /// 現在のブロックインデックスを更新し、各ブロックの表示・非表示を切り替える
    /// </summary>
    private void SwitchBlocks()
    {
        // 次のブロックインデックスに進む（最後まで行ったら先頭に戻る）
        currentBlockIndex = (currentBlockIndex + 1) % Block.Length;

        // 全ブロックを順番に処理
        for (int i = 0; i < Block.Length; i++)
        {
            // 現在とひとつ前のブロックだけを「表示状態」にする
            bool isActive = (i == currentBlockIndex || i == (currentBlockIndex - 1 + Block.Length) % Block.Length);

            // 各ブロックの状態を設定
            SetBlockState(Block[i], isActive);
        }
    }

    /// <summary>
    /// 指定ブロックの見た目・当たり判定・押し出し設定を制御する
    /// </summary>
    private void SetBlockState(GameObject block, bool isActive)
    {
        int childCount = block.transform.childCount;

        // ブロック内の全ての子オブジェクトを処理
        for (int i = 0; i < childCount; i++)
        {
            var child = block.transform.GetChild(i);
            var collider = child.GetComponent<Collider>();
            var renderer = child.GetComponent<MeshRenderer>();

            // マテリアルを切り替え（表示用 or 透明用）
            if (renderer)
                renderer.material = isActive ? indicationMaterial : transparentMaterial;

            // 当たり判定の有効/無効を切り替え
            if (collider)
                collider.enabled = isActive; // 非表示時は当たり判定をオフにする

            // 押し出しスクリプトを追加または取得
            BlockPushOut push = child.GetComponent<BlockPushOut>();
            if (push == null)
            {
                // なければ新しく追加
                push = child.gameObject.AddComponent<BlockPushOut>();
            }

            // 現在の表示状態をスクリプトに通知
            push.isActiveBlock = isActive;
        }
    }
}
