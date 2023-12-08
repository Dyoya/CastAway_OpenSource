using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class PlayerAttackTrigger : MonoBehaviour
{
    GameObject _player;
    ThirdPersonController _controller;
    StarterAssetsInputs _input;

    [SerializeField] float HandDamage = 2f;
    [SerializeField] int AxeDamage = 10;
    [SerializeField] int PickAxeDamage = 12;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _controller = _player.GetComponent<ThirdPersonController>();
        _input = _player.GetComponent<PlayerInput>().GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("트리거 내부 Enemy 인식");

            if (_controller.isAxe)
            {
                other.GetComponentInParent<EnemyBT>().temporaryDamage = AxeDamage;
            }
            else if (_controller.isPickAxe)
            {
                other.GetComponentInParent<EnemyBT>().temporaryDamage = PickAxeDamage;
            }
            else
            {
                other.GetComponentInParent<EnemyBT>().temporaryDamage = HandDamage;
            }
        }

        if (other.gameObject.tag == "Boss")
        {
            //Debug.Log("Boss 인식");

            if (_controller.isAxe)
            {
                other.GetComponentInParent<BearBossBT>().temporaryDamage = AxeDamage;
            }
            else if (_controller.isPickAxe)
            {
                other.GetComponentInParent<BearBossBT>().temporaryDamage = PickAxeDamage;
            }
            else
            {
                other.GetComponentInParent<BearBossBT>().temporaryDamage = HandDamage;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

}
