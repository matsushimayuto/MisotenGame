/*
 *UIManager.cs
 *ゲームの状態に応じてUIのUI切り替えを管理するスクリプト
 */
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UIEntry
    {
        public string uiKey;
        public GameObject uiRoot;
        public bool useFade;
        public float fadeDuration = 0.3f;
    }

    public static UIManager Instance { get; private set; }

    [SerializeField] private List<UIEntry> uiEntries;
    [SerializeField] private FadeController fadeController;

    private Dictionary<string, UIEntry> uiMap;
    private UIEntry currentEntry;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        uiMap = new Dictionary<string, UIEntry>();
        foreach (var entry in uiEntries)
        {
            uiMap.Add(entry.uiKey, entry);
            entry.uiRoot.SetActive(false);
        }
    }

    public void Show(string uiKey)
    {
        if (isTransitioning)
            return;

        if (!uiMap.TryGetValue(uiKey, out var next))
        {
            Debug.LogWarning($"UI not found: {uiKey}");
            return;
        }

        if (currentEntry == next)
            return;

        isTransitioning = true;

        // 旧UIを消す
        if (currentEntry != null)
        {
            if (currentEntry.useFade)
            {
                fadeController.FadeOut(
                    currentEntry.uiRoot,
                    currentEntry.fadeDuration,
                    () => ShowNext(next)
                );
            }
            else
            {
                currentEntry.uiRoot.SetActive(false);
                ShowNext(next);
            }
        }
        else
        {
            ShowNext(next);
        }
    }

    private void ShowNext(UIEntry next)
    {
        currentEntry = next;

        if (next.useFade)
        {
            fadeController.FadeIn(
                next.uiRoot,
                next.fadeDuration,
                () => isTransitioning = false
            );
        }
        else
        {
            next.uiRoot.SetActive(true);
            isTransitioning = false;
        }
    }
}
