using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class WorldAreaTrigger : MonoBehaviour
{
    


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // ワールド移動のUIを表示させるためのボタンUI表示関数　入れる
        // ワールド移動UI表示関数　入れる
        // UIが表示されたとき、Playerの入力が入らないようにする
        // --------------------------------------------------------ここからは他のScriptになるかな？
        // UIの選択入力処理
        // ワールド移動処理etc..
        other.GetComponent<InteractionController>()
             ?.OnEnterArea();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<InteractionController>()
             ?.OnExitArea();
    }
}
