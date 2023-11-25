using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class CookController : MonoBehaviour
{
    public GameObject boxNote = null;
    [SerializeField] Transform Center = null;
    [SerializeField] RectTransform[] timingRect = null;
    [SerializeField] GameObject note = null;
    Vector2[] timingBoxs = null;

    private void Start()
    {
        timingBoxs = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width / 2, Center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }

    public void CheckTiming()
    {
        float notePosX = boxNote.transform.localPosition.x;

        for (int i = 0; i < timingRect.Length;i++)
        {
            if (timingBoxs[i].x <= notePosX && notePosX <= timingBoxs[i].y)
            {
                Debug.Log("Hit" + i);   // i = 0 일때 퍼펙트 i = 1 일때 Good
                return;
            }
        }

        Debug.Log("Miss");
    }
}
