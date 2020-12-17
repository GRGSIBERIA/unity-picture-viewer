using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPanelScript : MonoBehaviour
{
    RectTransform rts;

    [SerializeField]
    bool isSliding = false;

    void Start()
    {
        rts = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSliding)
        {

        }
    }
}
