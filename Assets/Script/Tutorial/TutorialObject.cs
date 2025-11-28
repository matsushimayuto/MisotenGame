// チュートリアルUIごとの詳細設定
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TutorialObject", menuName = "Scriptable Objects/TutorialObject")]
public class TutorialObject : ScriptableObject
{
    [Tooltip("UIの設定項目")]
    public string message;          // 表示メッセージ
    public Sprite illustration;      // 表示画像
    public string waitforAction;    // 次表示のアクション
    public float textSpeed;         // テキスト表示スピード
}
