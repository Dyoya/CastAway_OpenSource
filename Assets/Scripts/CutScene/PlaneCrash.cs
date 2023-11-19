using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine. UI;
using UnityEngine.Playables;

public class PlaneCrash: MonoBehaviour
{
    public GameObject effectPrefab1; // ù ��° ����Ʈ ������
    public GameObject effectPrefab2;
    [SerializeField] Vector3 vec = new Vector3(0, 0, 0);
    [SerializeField] Vector3 vib = new Vector3 (0, 0, 0);
    [SerializeField] int vibrato = 0;
    [SerializeField] GameObject bird1;
    [SerializeField] GameObject bird2;
    [SerializeField] GameObject bird3;
    [SerializeField] PlayableDirector pd;

    private float timer = 0f;
    private float delay = 2f;

    public TMP_Text startTxt;
    public TextMeshProUGUI text;

    string dialogue;
    public string[] startDialogues;
    public string[] dialogues;
    

    void Start()
    {
        StartTalk(startDialogues);
    }

    IEnumerator Explosion()
    {
        pd.Play();
        bird1.SetActive(true);
        bird3.SetActive(true);
        bird2.SetActive(true);
        transform.DOMove(new Vector3(-690, 307, -100), 6.5f).SetSpeedBased();

        transform.DOShakeRotation(6.5f, new Vector3(0, 0, 5), 10, 10, true);
        yield return new WaitForSeconds(6.5f); 

        if (effectPrefab1 != null)
        {
            // ù ��° ����Ʈ ���
            effectPrefab1.SetActive(true);
        }

        if (effectPrefab2 != null)
        {
            effectPrefab2.SetActive(true);
        }

        transform.DOMove(vec, 20.0f);
        Vector3 direction = (vec - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.DORotateQuaternion(lookRotation, 2.0f);
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

        yield return new WaitForSeconds((text.text.Length / 5) + 1f);
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

        if(talkNum == dialogues.Length)
        {
            EndTalk();
            return;
        }

        StartCoroutine(Typing(dialogues[talkNum]));
    }
    public void EndTalk()
    {
        talkNum = 0;
        StartCoroutine(Explosion());
        Debug.Log("��� ��");
    }
}
