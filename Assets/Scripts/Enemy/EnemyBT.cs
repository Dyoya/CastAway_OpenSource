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
    [SerializeField] float moveSpeed = 6f; // Enemy �̵� �ӵ�
    [SerializeField] float findDistance = 10f; // �÷��̾� �߰� ����
    [SerializeField] float attackDistance = 4f; // �÷��̾� ���� ����

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true�� ��� ������ ���� ����
    //[SerializeField] int attackPower = 3; // Enemy ���ݷ�
    [SerializeField] float attackCooldown = 2f; // ���� ��ٿ� �ð�
    private bool canAttack = true; // ���� ������ �������� ���θ� ��Ÿ���� �÷���
    private float lastAttackTime; // ������ ���� �ð�


    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // �ǰ� ����

    NavMeshAgent _agent; //�׺���̼�

    Vector3 _originPos;
    Quaternion _originRot;

    //�ִϸ�����
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    const string _IDLE_ANIM_STATE_NAME = "Idle";
    const string _IDLE_ANIM_TRIGGER_NAME = "idle";

    const string _ATTACK_ANIM_STATE_NAME = "Attack";
    const string _ATTACK_ANIM_TRIGGER_NAME = "attack";

    const string _DAMAGED_ANIM_STATE_NAME = "Damaged";
    const string _DAMAGED_ANIM_TRIGGER_NAME = "damaged";

    const string _DIE_ANIM_STATE_NAME = "Die";
    const string _DIE_ANIM_TRIGGER_NAME = "die";

    const string _MOVE_ANIM_STATE_NAME = "Move";
    const string _MOVE_ANIM_TRIGGER_NAME = "move";

    // ������ �ӽ� ����
    int _temporaryDamage = 0;

    bool isMove = false;

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
                        new ActionNode(CheckDieHp),
                        new ActionNode(Die),
                        new ActionNode(CheckDieAnim),
                        new ActionNode(DestroyObject),
                    }
                ),
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
                new ActionNode(Return),
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(Idle),
                    }
                ),
            }
        );
    }
    bool IsAnimationRunning(string stateName)
    {
        if (_anim != null)
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

                return _anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
            }
        }

        return false;
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

    #region Attack Node
    INode.ENodeState CheckAttacking()
    {
        // ���� ���� �� ���� �ð��� ������ ���� �������� ����
        if (!canAttack && Time.time - lastAttackTime >= attackCooldown)
        {
            canAttack = true;
        }

        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState CheckAttackRange()
    {
        if (_playerTransform != null) 
        {
            // �÷��̾ ���� ��Ÿ� �ȿ� ��������
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                //Debug.Log("CheckAttackRange : Success");
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                return INode.ENodeState.ENS_Success;
            }
        }

        //Debug.Log("CheckAttackRange : Failure");
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState DoAttack()
    {
        if (_playerTransform != null && canAttack)
        {
            // ���� �ִϸ��̼� ����
            _anim.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);

            // �÷��̾� ���� �ٶ󺸵��� ����
            Vector3 temp = (_playerTransform.position - transform.position).normalized;
            temp.y = 0;
            transform.forward = temp;

            // ���� ������ �������� ���� ����
            canAttack = false;
            lastAttackTime = Time.time;

            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Find & Move Node
    INode.ENodeState CheckFind()
    {
        //Debug.Log("CheckFind");
        var overlapColliders = Physics.OverlapSphere(transform.position, findDistance, LayerMask.GetMask("Player"));

        // overlapColliders�� 1�� �̻� -> �÷��̾ ���� ��
        if (overlapColliders != null && overlapColliders.Length > 0) 
        {
            //isMove = true;
            _playerTransform = overlapColliders[0].transform;

            //Debug.Log("CheckFind : Success");
            return INode.ENodeState.ENS_Success;
        }

        _playerTransform = null;

        //Debug.Log("CheckFind : Failure");
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState Chase()
    {
        if (_playerTransform != null)
        {
            // ���� ���� ��Ÿ����� �̵� �Ϸ��� ���
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                //Debug.Log("Chase : Success");
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                isMove = false; // �̵� ���� ����
                return INode.ENodeState.ENS_Success;
            }

            if (!isMove) // �̵� ���� �ƴ� ��쿡�� Move �ִϸ��̼� ȣ��
            {
                if (!IsAnimationRunning(_MOVE_ANIM_STATE_NAME))
                    _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

                isMove = true; // �̵� ���·� ����
            }

            // ���� ���� ��Ÿ����� �̵� ���� ���
            //transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, Time.deltaTime * moveSpeed);
            _agent.isStopped = false;
            _agent.SetDestination(_playerTransform.position);

            //Debug.Log("Chase : Running");
            return INode.ENodeState.ENS_Running;
        }

        // �÷��̾ �߰� ���� ���
        //Debug.Log("Chase : Failure");
        if(isMove)
        {
            isMove = false;
        }
        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Move Origin Pos Node
    INode.ENodeState Return()
    {
        if (isMove)
        {
            if (Vector3.SqrMagnitude(_originPos - transform.position) < 0.5f)
            {
                isMove = false;
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                transform.position = _originPos;
                transform.rotation = _originRot;
                //Debug.Log("Return : Success");
                return INode.ENodeState.ENS_Success;
            }
            else
            {
                if(!IsAnimationRunning(_MOVE_ANIM_STATE_NAME))
                    _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

                //transform.position = Vector3.MoveTowards(transform.position, _originPos, Time.deltaTime * moveSpeed);
                _agent.isStopped = false;
                _agent.SetDestination(_originPos);
                //Debug.Log("Return : Running");
                return INode.ENodeState.ENS_Running;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Damaged Node
    INode.ENodeState CheckDamaged()
    {
        // �ǰ� �ִϸ��̼��� ���� ���̸�
        if (IsAnimationRunning(_DAMAGED_ANIM_STATE_NAME))
        {
            //Debug.Log("CheckDamaged : Running");
            return INode.ENodeState.ENS_Running;
        }

        //Debug.Log("CheckDamaged : Success");
        return INode.ENodeState.ENS_Success;
    }
    INode.ENodeState Damaged()
    {
        if (_temporaryDamage > 0)
        {
            currentHp -= _temporaryDamage;

            PlayDamagedSound();

            _anim.SetTrigger(_DAMAGED_ANIM_TRIGGER_NAME);

            _temporaryDamage = 0;

            //Debug.Log("Damaged : Success");
            return INode.ENodeState.ENS_Success;
        }
        else
        {
            //Debug.Log("Damaged : Failure");
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
    INode.ENodeState CheckDieHp()
    {
        if (currentHp <= 0)
        {
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState Die()
    {
        _anim.SetTrigger(_DIE_ANIM_TRIGGER_NAME);

        return INode.ENodeState.ENS_Success;
    }
    INode.ENodeState CheckDieAnim()
    {
        if (IsAnimationRunning(_DIE_ANIM_STATE_NAME))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }
    INode.ENodeState DestroyObject()
    {
        Destroy(gameObject);

        return INode.ENodeState.ENS_Success;
    }
    #endregion

    #region Idle Node
    INode.ENodeState Idle()
    {
        //Debug.Log("Idle");
        if(!IsAnimationRunning(_IDLE_ANIM_STATE_NAME))
            _anim.SetTrigger(_IDLE_ANIM_TRIGGER_NAME);

        return INode.ENodeState.ENS_Success;
    }
    #endregion
}
