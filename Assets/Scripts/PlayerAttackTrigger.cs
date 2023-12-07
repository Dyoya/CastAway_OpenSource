using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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
            //Debug.Log("Enemy 인식");

            if(!_controller.isAttack && _input.attack)
            {
                Debug.Log("Enemy 인식");
                if (_controller.isAxe)
                {
                    other.GetComponent<EnemyBT>().TakeDamage(AxeDamage);
                }
                else if (_controller.isPickAxe)
                {
                    other.GetComponent<EnemyBT>().temporaryDamage = PickAxeDamage;
                }
                else
                {
                    other.GetComponent<EnemyBT>().TakeDamage(HandDamage);
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
