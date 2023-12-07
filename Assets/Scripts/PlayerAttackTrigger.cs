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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("트리거 내부 Enemy 인식");

            if(!_controller.isAttack && Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                Debug.Log("좌클릭 Enemy 인식");
                if (_controller.isAxe)
                {
                    other.GetComponent<EnemyBT>().temporaryDamage = AxeDamage;
                }
                else if (_controller.isPickAxe)
                {
                    other.GetComponent<EnemyBT>().temporaryDamage = PickAxeDamage;
                }
                else
                {
                    other.GetComponent<EnemyBT>().temporaryDamage = HandDamage;
                }
            }
            
        }

        if (other.gameObject.tag == "Boss")
        {
            //Debug.Log("Boss 인식");

            if (!_controller.isAttack && _input.attack)
            {
                if (_controller.isAxe)
                {
                    other.GetComponent<BearBossBT>().temporaryDamage = AxeDamage;
                }
                else if (_controller.isPickAxe)
                {
                    other.GetComponent<BearBossBT>().temporaryDamage = PickAxeDamage;
                }
                else
                {
                    other.GetComponent<BearBossBT>().temporaryDamage = HandDamage;
                }
            }

        }
    }

}
