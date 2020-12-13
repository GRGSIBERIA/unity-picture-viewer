using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenScript : MonoBehaviour
{
    RectTransform pts;

    RectTransform rts;

    // Start is called before the first frame update
    void Start()
    {
        rts = GetComponent<RectTransform>();
        pts = rts.parent.GetComponent<RectTransform>();

        rts.sizeDelta = new Vector2(pts.rect.width, 0);
    }
}
