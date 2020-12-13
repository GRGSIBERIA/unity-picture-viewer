using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSlidingScript : MonoBehaviour
{
    /// <summary>
    /// 移動速度 [pixel/s]
    /// </summary>
    [SerializeField]
    float speed;

    public enum HideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [SerializeField]
    HideDirection hiddenDirection = HideDirection.Left;

    RectTransform pts;
    RectTransform rts;

    Vector3 dir;

    Vector3 size;

    [SerializeField]
    float windowFactor = 1f;

    bool isSliding = false;

    bool isHidding = true;

    // Start is called before the first frame update
    void Start()
    {
        rts = GetComponent<RectTransform>();
        pts = rts.parent.GetComponent<RectTransform>();

        // 隠れる向きをここで決める
        switch (hiddenDirection)
        {
            case HideDirection.Bottom:
                dir = Vector3.down;
                break;
            case HideDirection.Top:
                dir = Vector3.up;
                break;
            case HideDirection.Left:
                dir = Vector3.left;
                break;
            case HideDirection.Right:
                dir = Vector3.right;
                break;
            default:
                throw new UnityEngine.UnityException("Not selected hiddenDirection.");
        }

        // これをやっておかないと奥行きがなくなる
        size = pts.rect.size;
        size.z = 1f;

        // デフォルトで隠れている場合は，その座標に表示する
        if (isHidding)
        {
            rts.position = Vector3.Scale(dir, size) * windowFactor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 canvasSize = rts.rect.size;
        canvasSize.z = 1f;

        // スクリーンの大きさが変更された
        if (size != canvasSize)
            size = canvasSize;

        if (isSliding)
        {
            Vector3 hidedir = isHidding ? dir : -dir;

            rts.position += Vector3.Scale(hidedir, size) * Time.deltaTime;
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
