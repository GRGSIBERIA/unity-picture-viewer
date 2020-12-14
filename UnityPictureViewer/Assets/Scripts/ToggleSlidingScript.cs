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
    /// 隠れる方向
    /// </summary>
    public enum HideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField]
    HideDirection hiddenMode = HideDirection.Left;

    /// <summary>
    /// 移動速度 [pixel/s]
    /// </summary>
    [SerializeField, Header("Public Field")]
    float speed = 10f;

    /// <summary>
    /// 画面に対する窓幅の係数 [0.1, 1.0]
    /// </summary>
    [SerializeField]
    float windowFactor = 1f;

    [SerializeField, Header("Debbuging Field")]
    bool isSliding = false;

    [SerializeField]
    float totalDisplacement = 0f;

    /// <summary>
    /// キャンバスサイズの変更を検知する変数
    /// </summary>
    Vector2 canvasSize;

    RectTransform pts;

    RectTransform rts;

    /// <summary>
    /// 文脈上，逆の意味になっているかもしれないので取り扱い注意
    /// </summary>
    bool isHidding = true;

    float baseSign = 1f;

    bool isLeftRight = true;

    float defaultPosition;

    float movedPosition;

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
    /// <returns>StopするならTrue</returns>
    bool IsStopMoving(float displacement)
    {
        totalDisplacement += Mathf.Abs(displacement);
        float canvasValue = isLeftRight ? canvasSize.x : canvasSize.y;

        if (totalDisplacement > canvasValue * windowFactor)
        {
            return true;
        }
        return false;
    }

    float ComputeDisplacement()
    {
        var pos = rts.localPosition;
        float displacement = 0f;

        if (isLeftRight)
        {
            displacement = MovePosition(canvasSize.x);
            pos.x += displacement;
        }
        else
        {
            displacement = MovePosition(canvasSize.y);
            pos.y += displacement;
        }
        rts.localPosition = pos;
        
        return displacement;
    }

    void ResetPosition()
    {
        var pos = rts.localPosition;

        float hidePos = 0f;
        switch (hiddenMode)
        {
            case HideDirection.Left:
                hidePos = -canvasSize.x;
                break;
            case HideDirection.Right:
                hidePos = canvasSize.x;
                break;
            case HideDirection.Up:
                hidePos = canvasSize.y;
                break;
            case HideDirection.Down:
                hidePos = -canvasSize.y;
                break;
        }

        if (isLeftRight)
        {
            pos.x = isHidding ? 0f : hidePos;
        }
        else
        {
            pos.y = isHidding ? 0f : hidePos;
        }
        rts.localPosition = pos;
    }

    // Update is called once per frame
    void Update()
    {
        // キャンバスの大きさが変更されたら移動量が変更される
        canvasSize = canvasSize != pts.rect.size ? pts.rect.size : canvasSize;

        if (isSliding)
        {
            // このフレームでの変位を知りたい
            float displacement = ComputeDisplacement();

            if (IsStopMoving(displacement))
            {
                // もし，止まるなら必要な値で初期化する
                ResetPosition();

                isSliding = false;
                isHidding = !isHidding;
                totalDisplacement = 0f;
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
