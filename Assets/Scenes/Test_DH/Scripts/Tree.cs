using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField]
    private int HP;

    [SerializeField]
    private float destroyTime;

    [SerializeField]
    private BoxCollider box; // 박스콜라이더

    //필요 오브젝트
    [SerializeField]
    private GameObject go_tree; // 일반 바위
    [SerializeField]
    private GameObject go_effect_prefabs;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip effect_sound1;
    [SerializeField]
    private AudioClip effect_sound2;

    [SerializeField]
    private StarterAssets.ThirdPersonController thePlayer;
    [SerializeField]
    private GameObject ItemPrefab;

    public void Mining()
    {
        audioSource.clip = effect_sound1;
        audioSource.Play();
        var clone = Instantiate(go_effect_prefabs, box.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        HP--;
        if (HP <= 0)
        {
            Destruction();
        }
    }

    private void Destruction()
    {
        audioSource.clip = effect_sound2;
        audioSource.Play();

        box.enabled = false;
        Destroy(go_tree);

        for (int i = 0; i < 3; i++)
        {
            thePlayer.createprefabs(ItemPrefab, "나뭇가지");
        }
    }
}
