/*
 *UIManager.cs
 *ゲームの状態に応じてUIのUI切り替えを管理するスクリプト
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static UIManager Instance { get; private set; }
    [SerializeField] private List<UISet> uiSets;

    [Tooltip("UIPanel")]
    [SerializeField] private Image fadePanel; // フェード用パネル


    private Coroutine fadeCoroutine;

    // UIManagerのSingletonを初期化
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        ;

        // GameManagerが存在すれば、イベントを登録
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;

            // 初期状態に応じて、UIをセットして表示
            HandleStateChanged(GameManager.Instance.CurrentState);
        }
    }

    public void Start()
    {
        // 保存したゲーム状態ごとのUIをイベント関数にて呼び出し、読み込み
        //GameManager.Instance.OnStateChanged += HandleStateChanged;
        //HandleStateChanged(GameManager.Instance.CurrentState);  // ゲーム開始時のUIを表示
    }

    // HandleStateChanged関数 : 引数:GameState(読み込みたいゲーム状態を入力)、戻り値:なし
    // GameManagerからCuurentStateを受け取り、設定してあるUIを表示する関数
    public void HandleStateChanged(GameState state)
    {
        Debug.Log($"[UIManager] HandleStateChanged called. State = {state}");

        // 読み込んだ状態に当てはまるUIを読み込み、設定
        foreach (var set in uiSets)
        {
            bool active = (set.state == state);

            if (set.uiRoot != null)
            {
                set.uiRoot.SetActive(active);
                Debug.Log($"[UIManager] {set.uiRoot.name} → {(active ? "ON" : "OFF")} (Match: {set.state == state})");
            }
            else
            {
                Debug.LogWarning($"[UIManager] uiRoot is NULL for state {set.state}");
            }

        }
    }

    //======================================
    //  ロードUI制御
    //======================================




    //======================================
    //  フェードUI制御
    //======================================
    public void FadeIn(float duration = 1f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, duration));
    }

    public void FadeOut(float duration = 1f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f, duration));
    }

    private IEnumerator FadeRoutine(float start, float end, float duration)
    {
        if (fadePanel == null) yield return null;

        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, end, t / duration);
            color.a = a;
            fadePanel.color = color;
            yield return null;
        }

        color.a = end;
        fadePanel.color = color;

        // 完全に透明になったら非表示
        if (end == 0f)
            fadePanel.gameObject.SetActive(false);

    }

}
