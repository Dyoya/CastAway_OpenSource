using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelicopterController : MonoBehaviour
{
    Vector3 currentLocalPosition;

    public TMP_Text startTxt;
    public TextMeshProUGUI text;

    string dialogue;
    public string[] startDialogues;
    public string[] dialogues;

    void Start()
    {
        StartCoroutine(HelicopterMove());
    }

    IEnumerator HelicopterMove()
    {
        yield return new WaitForSeconds(1f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition - new Vector3(-200, 0, 0), 20f);
        yield return new WaitForSeconds(15f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalRotate(new Vector3(0,-10, 0), 5f).SetEase(Ease.Linear);
        transform.DOLocalMove(currentLocalPosition - new Vector3(0, 0, -30), 5f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(5f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition - new Vector3(0, 80, 0), 10f);
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene("TitleMenu");
    }

    public static void TMPDOText(TextMeshProUGUI text)
    {
        float totalCharacters = text.text.Length;
        float duration = totalCharacters / 5;

        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration).SetEase(Ease.Linear);
    }

    IEnumerator Typing(string talk)
    {
        text.text = talk;
        TMPDOText(text);

        yield return new WaitForSeconds((text.text.Length / 10) + 2f);
        NextTalk();
    }

    public int talkNum;
    public void StartTalk(string[] talks)
    {
        dialogues = talks;

        StartCoroutine(Typing(dialogues[talkNum]));
    }

    public void NextTalk()
    {
        startTxt.text = null;
        talkNum++;

        if (talkNum == dialogues.Length)
        {
            EndTalk();
            return;
        }

        StartCoroutine(Typing(dialogues[talkNum]));
    }
    public void EndTalk()
    {
        talkNum = 0;
        Debug.Log("´ë»ç ³¡");     
    }
}
