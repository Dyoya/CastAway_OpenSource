using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int HP;

    [SerializeField]
    private float destroyTime;

    [SerializeField]
    private SphereCollider col; // 구체콜라이더

    //필요 오브젝트
    [SerializeField]
    private GameObject go_rock; // 일반 바위
    [SerializeField]
    private GameObject go_debris; //깨진 바위
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
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        HP--;
        if(HP <= 0)
        {
            Destruction();
        }
    }

    private void Destruction()
    {
        audioSource.clip = effect_sound2;
        audioSource.Play();

        for(int i = 0; i < 3; i++)
        {
            thePlayer.createprefabs(ItemPrefab, "돌맹이");
        }
        col.enabled = false;
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }
}
