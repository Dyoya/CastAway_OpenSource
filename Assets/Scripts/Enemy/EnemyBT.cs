using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(Animator))]
public class EnemyBT : MonoBehaviour
{
    Transform _playerTransform;

    [Header("HP")]
    [SerializeField] int currentHp;
    [SerializeField] int maxhp = 15;

    [Header("Distance")]
    [SerializeField] float moveSpeed = 9f; // Enemy �̵� �ӵ�
    [SerializeField] float findDistance = 30f; // �÷��̾� �߰� ����
    [SerializeField] float attackDistance = 4f; // �÷��̾� ���� ����

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true�� ��� ������ ���� ����
    //[SerializeField] int attackPower = 3; // Enemy ���ݷ�


    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // �ǰ� ����

    NavMeshAgent _agent; //�׺���̼�

    Vector3 _originPos;
    Quaternion _originRot;

    //�ִϸ�����
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    const string _ATTACK_ANIM_STATE_NAME = "Attack";
    const string _ATTACK_ANIM_TRIGGER_NAME = "attack";

    const string _DAMAGED_ANIM_STATE_NAME = "Damaged";
    const string _DAMAGED_ANIM_TRIGGER_NAME = "damaged";

    // ������ �ӽ� ����
    int _temporaryDamage = 0;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        _BTRunner = new BehaviorTreeRunner(SettingBT());
        _agent = GetComponent<NavMeshAgent>();

        _originPos = transform.position;
        _originRot = transform.rotation;
    }
    private void Start()
    {
        currentHp = maxhp;
        _agent.speed = moveSpeed;
    }
    private void Update()
    {
        _BTRunner.Operate();
    }
    INode SettingBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDamaged),
                        new ActionNode(Damaged),
                    }
                ),
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckAttacking),
                        new ActionNode(CheckAttackRange),
                        new ActionNode(DoAttack),
                    }
                ),
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckFind),
                        new ActionNode(Chase),
                    }
                ),
                new ActionNode(Return)
            }
        );
    }
    #region Public Func
    public void SetDamage(int  damage)
    {
        if(_temporaryDamage != 0)
        {
            _temporaryDamage = damage;
        }
    }
    #endregion
    bool IsAnimationRunning(string stateName)
    {
        if(_anim != null)
        {
            if(_anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }

        return false;
    }

    #region Attack Node
    INode.ENodeState CheckAttacking()
    {
        // ���� �ִϸ��̼��� ���� ���̸�
        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            Debug.Log("CheckAttacking : Running");
            return INode.ENodeState.ENS_Running;
        }

        Debug.Log("CheckAttacking : Success");
        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState CheckAttackRange()
    {
        if (_playerTransform != null) 
        { 
            // �÷��̾ ���� ��Ÿ� �ȿ� ��������
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                Debug.Log("CheckAttackRange : Success");
                _agent.enabled = false;
                return INode.ENodeState.ENS_Success;
            }
        }

        Debug.Log("CheckAttackRange : Failure");
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState DoAttack()
    {
        if (_playerTransform != null)
        {
            // ���� �ִϸ��̼� ����
            //_anim.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);

            //��� �÷��̾� ���� �ٶ󺸵���
            Vector3 temp = (_playerTransform.position - transform.position).normalized;
            temp.y = 0;
            transform.forward = temp;

            Debug.Log("DoAttack : Success");
            return INode.ENodeState.ENS_Success;
        }

        Debug.Log("DoAttack : Failure");
        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Find & Move Node
    INode.ENodeState CheckFind()
    {
        Debug.Log("CheckFind");
        var overlapColliders = Physics.OverlapSphere(transform.position, findDistance, LayerMask.GetMask("Player"));

        // overlapColliders�� 1�� �̻� -> �÷��̾ ���� ��
        if (overlapColliders != null && overlapColliders.Length > 0) 
        {
            _playerTransform = overlapColliders[0].transform;

            Debug.Log("CheckFind : Success");
            return INode.ENodeState.ENS_Success;
        }

        _playerTransform = null;

        Debug.Log("CheckFind : Failure");
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState Chase()
    {
        if (_playerTransform != null)
        {   
            // ���� ���� ��Ÿ����� �̵� �Ϸ��� ���
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                Debug.Log("Chase : Success");
                _agent.enabled = false;
                return INode.ENodeState.ENS_Success;
            }

            // ���� ���� ��Ÿ����� �̵� ���� ���
            //transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, Time.deltaTime * moveSpeed);
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);

            Debug.Log("Chase : Running");
            return INode.ENodeState.ENS_Running;
        }

        // �÷��̾ �߰� ���� ���
        Debug.Log("Chase : Failure");
        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Move Origin Pos Node
    INode.ENodeState Return()
    {
        if (Vector3.SqrMagnitude(_originPos - transform.position) < 0.5f)
        {
            _agent.enabled = false;
            transform.position = _originPos;
            transform.rotation = _originRot;
            Debug.Log("Return : Success");
            return INode.ENodeState.ENS_Success;
        }
        else
        {
            //transform.position = Vector3.MoveTowards(transform.position, _originPos, Time.deltaTime * moveSpeed);
            _agent.enabled = true;
            _agent.SetDestination(_originPos);
            Debug.Log("Return : Running");
            return INode.ENodeState.ENS_Running;
        }
    }
    #endregion

    #region Damaged Node
    INode.ENodeState CheckDamaged()
    {
        // �ǰ� �ִϸ��̼��� ���� ���̸�
        if (IsAnimationRunning(_DAMAGED_ANIM_STATE_NAME))
        {
            Debug.Log("CheckDamaged : Running");
            return INode.ENodeState.ENS_Running;
        }

        Debug.Log("CheckDamaged : Success");
        return INode.ENodeState.ENS_Success;
    }
    INode.ENodeState Damaged()
    {
        if (_temporaryDamage > 0)
        {
            currentHp -= _temporaryDamage;

            if (currentHp <= 0)
            {
                Die();
            }

            PlayDamagedSound();

            //_anim.SetTrigger(_DAMAGED_ANIM_TRIGGER_NAME);

            _temporaryDamage = 0;

            Debug.Log("Damaged : Success");
            return INode.ENodeState.ENS_Success;
        }
        else
        {
            Debug.Log("Damaged : Failure");
            return INode.ENodeState.ENS_Failure;
        }
        
    }
    void PlayDamagedSound()
    {
        if(asDamagedSound != null)
        {
            asDamagedSound.Play();
        }
    }
    #endregion

    #region Die Node
    void Die()
    {
        StartCoroutine(DestoryAfterDelay(2f));
    }
    IEnumerator DestoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    #endregion
}
