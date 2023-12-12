using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//[RequireComponent(typeof(Animator))]
public class EscapeEnemyBT : MonoBehaviour
{
    Transform _playerTransform;
    GameObject player;
    ThirdPersonController _tpc;

    [Header("HP")]
    [SerializeField] int currentHp;
    [SerializeField] int maxhp = 15;

    [Header("Distance")]
    [SerializeField] float moveSpeed = 10f; // Enemy �̵� �ӵ�
    [SerializeField] float findDistance = 5f; // �÷��̾� �߰� ����
    [SerializeField] float runDistance = 10f;

    [Header("Patrol")]
    [SerializeField] float idlePatrolRange = 10f; // ���� ���� �ִ� �Ÿ�
    [SerializeField] float patrolTime = 10f; // ���� ���� ���� �ð�
    [SerializeField] float patrolMoveSpeed = 6f;


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

    const string _DAMAGED_ANIM_STATE_NAME = "Damaged";
    const string _DAMAGED_ANIM_TRIGGER_NAME = "damaged";

    const string _DIE_ANIM_STATE_NAME = "Die";
    const string _DIE_ANIM_TRIGGER_NAME = "die";

    const string _MOVE_ANIM_STATE_NAME = "Move";
    const string _MOVE_ANIM_TRIGGER_NAME = "move";

    // ������ �ӽ� ����
    public int temporaryDamage = 0;

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
        _agent.updateRotation = false;

        player = GameObject.Find("Player");
        _tpc = player.GetComponent<ThirdPersonController>();

        _enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }
    private void Update()
    {
        _patrolCurrentTime += Time.deltaTime;
        _BTRunner.Operate();

        // agent ȸ���� ���ϱ�
        Vector3 lookrotation = _agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 5f * Time.deltaTime);
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
                        new ActionNode(CheckFind),
                        new ActionNode(Run),
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
    public void SetDamage(int damage)
    {
        if (temporaryDamage != 0)
        {
            temporaryDamage = damage;
        }
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
    // TODO : Run���� �����ϱ� //
    INode.ENodeState Run()
    {
        if (_playerTransform != null)
        {
            if (!isMove) // �̵� ���� �ƴ� ��쿡�� Move �ִϸ��̼� ȣ��
            {
                if (!IsAnimationRunning(_MOVE_ANIM_STATE_NAME))
                    _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

                isMove = true; // �̵� ���·� ����
                _agent.speed = moveSpeed;
                _agent.isStopped = false;

                float r = Random.Range(3, runDistance);
                Vector3 sphere = Random.insideUnitSphere.normalized;
                Vector3 patrolPosition = gameObject.transform.position + r * sphere;
                _agent.SetDestination(patrolPosition);
            }

            if (!(_agent.pathPending || _agent.remainingDistance > moveSpeed * 0.3))
            {
                isMove = false;
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;

                return INode.ENodeState.ENS_Success;
            }

            //Debug.Log("Run : Running");
            return INode.ENodeState.ENS_Running;
        }

        // �÷��̾ �߰� ���� ���
        //Debug.Log("Run : Failure");
        if (isMove)
        {
            isMove = false;
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
    void PlayDamagedSound()
    {
        if (asDamagedSound != null)
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
        _enemyManager.RabbitDie();

        for (int i = 0; i < 2; i++)
        {
            _tpc.createprefabs(ItemPrefab, "���");
        }

        Debug.Log("�ı�");
        Destroy(gameObject);
    }
    #endregion

    #region Idle Node
    INode.ENodeState Patrol()
    {
        if (!isMove && _patrolCurrentTime >= patrolTime) // ���� ���� ��, ó������ ȣ��
        {
            _agent.isStopped = false;
            _agent.speed = patrolMoveSpeed;
            isMove = true; // �̵� ���·� ����

            // ���� ���� 1�� �̱�
            float r = Random.Range(3, idlePatrolRange);
            Vector3 sphere = Random.insideUnitSphere.normalized;
            Vector3 patrolPosition = gameObject.transform.position + r * sphere;
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
    INode.ENodeState Idle()
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
