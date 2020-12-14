using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 何かしらのアクションによってスライディングしながらトグルするコンポーネント
/// ストレッチされていることが前提
/// </summary>
public class ToggleSlidingScript : MonoBehaviour
{
    /// <summary>
    /// 移動速度 [pixel/s]
    /// </summary>
    [SerializeField]
    float speed = 10f;

    RectTransform pts;
    RectTransform rts;

    /// <summary>
    /// 画面に対する窓幅の係数 [0.1, 1.0]
    /// </summary>
    [SerializeField]
    float windowFactor = 1f;

    [SerializeField]
    bool isSliding = false;

    bool isHidding = true;

    /// <summary>
    /// キャンバスサイズの変更を検知する変数
    /// </summary>
    Vector2 canvasSize;

    float totalDisplacement = 0f;

    public enum HideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField]
    HideDirection hiddenMode = HideDirection.Left;

    float baseSign = 1f;

    bool isLeftRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rts = GetComponent<RectTransform>();
        pts = rts.parent.GetComponent<RectTransform>();

        if (!(hiddenMode == HideDirection.Left || hiddenMode == HideDirection.Right))
        {
            isLeftRight = false;
        }

        // 基本的な移動向きを決める
        switch (hiddenMode)
        {
            case HideDirection.Left:
                baseSign = 1f;
                break;
            case HideDirection.Right:
                baseSign = -1f;
                break;
            case HideDirection.Up:
                baseSign = 1f;
                break;
            case HideDirection.Down:
                baseSign = -1f;
                break;
        }
    }

    /// <summary>
    /// 変位の量を決める
    /// </summary>
    /// <param name="cv">キャンバスのサイズ，上下左右でx,yが異なる</param>
    /// <returns>変位 [px/s]</returns>
    float MovePosition(float cv)
    {
        // 移動する向きを決める
        float sign = (isHidding ? 1f : -1f) * baseSign;

        return sign * cv * speed * Time.deltaTime;
    }

    /// <summary>
    /// 一定の変位を超えたら自動的にストップさせる
    /// </summary>
    /// <param name="displacement">変位量</param>
    /// <param name="canvasValue">キャンバスの大きさ</param>
    void JudgeStopMove(float displacement, float canvasValue)
    {
        totalDisplacement += Mathf.Abs(displacement);
        if (totalDisplacement > canvasValue * windowFactor)
        {
            isSliding = false;
            isHidding = !isHidding;
            totalDisplacement = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // キャンバスの大きさが変更されたら移動量が変更される
        canvasSize = canvasSize != pts.rect.size ? pts.rect.size : canvasSize;

        if (isSliding)
        {
            // それぞれ上下左右で与えるパラメータが異なるからx, yをそれぞれ真逆にしないように注意する
            if (isLeftRight)
            {   // 左右に水平移動
                var pos = rts.position;
                var displacement = MovePosition(canvasSize.x);
                pos.x += displacement;
                rts.position = pos;

                JudgeStopMove(displacement, canvasSize.x);
            }
            else
            {   // 上下に水平移動
                var pos = rts.position;
                var displacement = MovePosition(canvasSize.y);
                pos.y += displacement;
                rts.position = pos;

                JudgeStopMove(displacement, canvasSize.y);  
            }
        }
    }

    /// <summary>
    /// 窓幅の係数を変える
    /// 1が全部覆う, 0にしてしまうと取り出せなくなる
    /// </summary>
    /// <param name="windowFactor">窓幅の比率</param>
    public void SetWindowFactor(float windowFactor)
    {
        this.windowFactor = windowFactor;

        // 窓係数が0.1を下回るようだったらAssertを出すようにしておく
        UnityEngine.Assertions.Assert.IsTrue(this.windowFactor > 0.1f, "WindowFactor is not over 0.1f. Window Factor completely hides from canvas under 0.1f.");
    }
}
