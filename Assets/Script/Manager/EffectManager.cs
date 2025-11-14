using UnityEngine;

public class EffectManager : MonoBehaviour
{
    /// <summary>
    /// エフェクトの種類
    /// </summary>
    public enum EffectType
    {
        Hit,
        Sparkle,
        Smoke
    }
    [Header("エフェクト一覧")]
    [SerializeField] private GameObject[] Effects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 任意の位置にエフェクトを出現
    /// </summary>
    /// <param name="type"></param>
    /// <param name="position"></param>
    public void PlayEffect(EffectType type, Vector3 position)
    {
        Instantiate(Effects[(int)type], position, Quaternion.identity);
    }
}
