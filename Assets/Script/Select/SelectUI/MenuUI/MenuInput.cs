using UnityEngine;

public class MenuInput : MonoBehaviour
{
    public SelectArrow selector;

    float moveZ = 0f;
    bool isPressed = false;         // ゲームパッド入力がされているか
    float KeepPressingTime = 0.0f;  // 一度入力してからそのまま押し続けている時間
    const float limitTime = 0.25f;  // 一度入力してから再度入力を受け付けるまでの時間


    void Update()
    {
        moveZ = Input.GetAxis("Cross_Y");

        if (!isPressed)
        {
            // ここで矢印の選択する対象を変更している
            if (Input.GetKeyDown(KeyCode.UpArrow) || moveZ > 0)
            {
                selector.Next(-1);
                isPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || moveZ < 0)
            {
                selector.Next(+1);
                isPressed = true;
            }

            // 選択した対象の処理を実行
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
            {
                if (selector.Current.IsSelectable)
                {
                    selector.Current.Select();
                }

            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cansel();
            }
            KeepPressingTime = 0f;
        }
        else
        {
            KeepPressingTime += Time.deltaTime;
        }

        // 入力制限
        if (KeepPressingTime > limitTime)
        {
            isPressed = false;
            KeepPressingTime = 0.0f;
        }


    }

    void Cansel()
    {
        Debug.Log("Cansel");
    }

}
