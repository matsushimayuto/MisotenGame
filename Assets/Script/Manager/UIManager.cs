/*
 *UIManager.cs
 *ゲームの状態に応じてUIのUI切り替えを管理するスクリプト
 */
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UISet
    {
        [Tooltip("UIを表示したいゲーム状態")]
        public GameState state;
        [Tooltip("表示したいCanvasやPanel等を参照")]
        public GameObject uiRoot;
    }

    public static UIManager Instance {  get; private set; }
    [SerializeField] private List<UISet> uiSets;

    // UIManagerのSingletonを初期化
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); };
    }

    public void Start()
    {
        // 保存したゲーム状態ごとのUIをイベント関数にて呼び出し、読み込み
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CuurentState);  // 初期状態を反映
    }

    // HandleStateChanged関数 : 引数:GameState(読み込みたいゲーム状態を入力)、戻り値:なし
    // GameManagerからCuurentStateを読み込むために使う関数(今のところは・・・)
    public void HandleStateChanged(GameState state)
    {
        // 読み込んだ状態に当てはまるUIを読み込み、設定
        foreach (var set in uiSets)
        {
            set.uiRoot.SetActive(set.state == state);
        }
    }
}
