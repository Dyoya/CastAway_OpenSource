using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//[RequireComponent(typeof(Animator))]
public class EnemyBT : MonoBehaviour
{
    Transform _playerTransform;
    GameObject player;
    ThirdPersonController _tpc;

    [Header("HP")]
    [SerializeField] int currentHp;
    [SerializeField] int maxhp = 15;

    [Header("Distance")]
    [SerializeField] float moveSpeed = 6f; // Enemy �̵� �ӵ�
    [SerializeField] float findDistance = 10f; // �÷��̾� �߰� ����
    [SerializeField] float attackDistance = 4f; // �÷��̾� ���� ����

    [Header("Patrol")]
    [SerializeField] float idlePatrolRange = 10f; // ���� ���� �ִ� �Ÿ�
    [SerializeField] float patrolTime = 10f; // ���� ���� ���� �ð�
    [SerializeField] float patrolMoveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true�� ��� ������ ���� ����
    [SerializeField] int attackPower = 3; // Enemy ���ݷ�
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

    EnemyManager _enemyManager;

    float _patrolCurrentTime = 0;

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
    public int temporaryDamage = 0;

    bool isMove = false;
    bool isReturn = false;

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
        player = GameObject.Find("Player");
        _tpc = player.GetComponent<ThirdPersonController>();

        currentHp = maxhp;
        _agent.updateRotation = false;

        _enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }
    private void Update()
    {
        _patrolCurrentTime += Time.deltaTime;
        _BTRunner.Operate();

        // agent ȸ���� ���ϱ�
        Vector3 lookrotation = _agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 5f * Time.deltaTime);

        //Debug.Log(temporaryDamage);
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
                        new ActionNode(CheckDieAnim),
                        new ActionNode(Die),
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
                //new ActionNode(Return),
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(Patrol),
                        new ActionNode(Idle),
                    }
                ),
            }
        );
    }
    protected bool IsAnimationRunning(string stateName)
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
        if(temporaryDamage != 0)
        {
            temporaryDamage = damage;
        }
    }
    #endregion

    #region Attack Node
    protected INode.ENodeState CheckAttacking()
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

    protected INode.ENodeState CheckAttackRange()
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

    protected INode.ENodeState DoAttack()
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
    protected void TakeDamage()
    {
        // �÷��̾����� ������ ����
        _tpc.AttackDamage(attackPower);
    }
    #endregion

    #region Find & Move Node
    protected INode.ENodeState CheckFind()
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
    protected INode.ENodeState Chase()
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
                isReturn = true; // �������� ���� ����
            }

            // ���� ���� ��Ÿ����� �̵� ���� ���
            //transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, Time.deltaTime * moveSpeed);
            _agent.speed = moveSpeed;
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
    protected INode.ENodeState Return()
    {
        if (isReturn)
        {
            // ���͸� �Ϸ� ���� ���
            if (Vector3.SqrMagnitude(_originPos - transform.position) < 0.5f)
            {
                isMove = false;
                isReturn = false;
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
                _agent.speed = moveSpeed;
                _agent.SetDestination(_originPos);
                //Debug.Log("Return : Running");
                return INode.ENodeState.ENS_Running;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Damaged Node
    protected INode.ENodeState CheckDamaged()
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
    protected INode.ENodeState Damaged()
    {
        if (temporaryDamage > 0)
        {
            currentHp -= temporaryDamage;

            PlayDamagedSound();

            _anim.SetTrigger(_DAMAGED_ANIM_TRIGGER_NAME);

            temporaryDamage = 0;

            //Debug.Log("Damaged : Success");
            return INode.ENodeState.ENS_Success;
        }
        else
        {
            //Debug.Log("Damaged : Failure");
            return INode.ENodeState.ENS_Failure;
        }
        
    }
    protected void PlayDamagedSound()
    {
        if(asDamagedSound != null)
        {
            asDamagedSound.Play();
        }
    }
    #endregion

    #region Die Node
    protected INode.ENodeState CheckDieHp()
    {
        if (currentHp <= 0)
        {
            //Debug.Log("��� ü�� Ȯ��");
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }
    protected INode.ENodeState CheckDieAnim()
    {
        if (IsAnimationRunning(_DIE_ANIM_STATE_NAME))
        {
            //Debug.Log("��� �ִϸ��̼� �����");
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }
    protected INode.ENodeState Die()
    {
        if (!IsAnimationRunning(_DIE_ANIM_STATE_NAME))
        {
            //Debug.Log("��� �ִϸ��̼� ���");
            _anim.SetTrigger(_DIE_ANIM_TRIGGER_NAME);
        }

        return INode.ENodeState.ENS_Success;
    }

    [SerializeField] GameObject ItemPrefab; // ������ ������

    protected void DestroyObject()
    {
        Debug.Log("�ı�");
        _enemyManager.BearDie();
        Destroy(gameObject);

        for (int i = 0; i < 3; i++)
        {
            _tpc.createprefabs(ItemPrefab, "���");
        }
    }
    #endregion

    #region Idle Node
    protected INode.ENodeState Patrol()
    {   
        if (!isMove && _patrolCurrentTime >= patrolTime) // ���� ���� ��, ó������ ȣ��
        {
            _agent.isStopped = false;
            _agent.speed = patrolMoveSpeed;
            isMove = true; // �̵� ���·� ����

            // ���� ���� 1�� �̱�
            float r = Random.Range(3, idlePatrolRange);
            Vector3 sphere = Random.insideUnitSphere.normalized;
            Vector3 patrolPosition =  gameObject.transform.position +  r * sphere;
            _agent.SetDestination(patrolPosition);

            _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

            return INode.ENodeState.ENS_Running;
        }


        // �̵��� �Ϸ�Ǿ��� ��
        if (!(_agent.pathPending || _agent.remainingDistance > 0.1f))
        {
            isMove = false;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;

            if (_patrolCurrentTime >= patrolTime)
            {
                _patrolCurrentTime = 0;

                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Running;
    }
    protected INode.ENodeState Idle()
    {
        //Debug.Log("Idle");
        if (!IsAnimationRunning(_IDLE_ANIM_STATE_NAME) && !isMove)
        {
            _anim.SetTrigger(_IDLE_ANIM_TRIGGER_NAME);
        }

        return INode.ENodeState.ENS_Success;
    }
    #endregion
}
