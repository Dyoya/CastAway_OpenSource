using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.XR;

//[RequireComponent(typeof(Animator))]
public class BearBossBT : MonoBehaviour
{
    Transform _playerTransform;
    public GameObject Player;

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
    //[SerializeField] int attackPower = 3; // Enemy ���ݷ�
    [SerializeField] float attackCooldown = 2f; // ���� ��ٿ� �ð�
    private bool canAttack = true; // ���� ������ �������� ���θ� ��Ÿ���� �÷���
    private float lastAttackTime; // ������ ���� �ð�


    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // �ǰ� ����

    [Header("SkillRangeObject")]
    [SerializeField] GameObject rushTriggerCollision;

    NavMeshAgent _agent; //�׺���̼�

    Vector3 _originPos;
    Quaternion _originRot;

    //�ִϸ�����
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    List<BearBossSkill> skill;

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

    const string _BACKSTEP_ANIM_STATE_NAME = "Backstep";
    const string _BACKSTEP_ANIM_TRIGGER_NAME = "backstep";

    const string _RUSH_ANIM_STATE_NAME = "Rush";
    const string _RUSH_ANIM_TRIGGER_NAME = "rush";

    const string _JUMP_ANIM_STATE_NAME = "Jump";
    const string _JUMP_ANIM_TRIGGER_NAME = "jump";

    const string _LANDING_ANIM_STATE_NAME = "Landing";
    const string _LANDING_ANIM_TRIGGER_NAME = "landing";

    const string _STAMP_ANIM_STATE_NAME = "Stamp";
    const string _STAMP_ANIM_TRIGGER_NAME = "stamp";

    // ������ �ӽ� ����
    int _temporaryDamage = 0;

    bool isMove = false;
    bool isReturn = false;

    // ��ų ���� ���� ����
    bool isCharging = false;
    float elapsedTime = 0f;
    float waitDuration = 3f; // �齺�� �̵��� �ɸ��� �ð�
    float skillTime = 0f;
    Vector3 directionToGoal;
    GameObject scan;
    Vector3 goal;

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

        skill = new List<BearBossSkill>
        {
            new BearBossSkill("����", 10f),
            new BearBossSkill("����", 19f),
            new BearBossSkill("���", 29f),
        };
    }
    private void Update()
    {
        _patrolCurrentTime += Time.deltaTime;
        _BTRunner.Operate();

        // agent ȸ���� ���ϱ�
        Vector3 lookrotation = _agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 5f * Time.deltaTime);

        foreach (BearBossSkill s in skill)
        {
            s.UpdateCooldown(Time.deltaTime);
        }

        RayCast();
    }
    INode SettingBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                // ��� ���
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
                // ��ų ���� ���
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckFind),
                        new SelectorNode
                        (
                            new List<INode>()
                            {
                                new ActionNode(Stamp),
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Jump),
                                        new ActionNode(Landing),
                                    }
                                ),
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Charge),
                                        new ActionNode(Rush),
                                    }
                                ),
                                new ActionNode(initSkill),
                            }
                        )
                    }
                ),
                // �ǰ� ���
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDamaged),
                        new ActionNode(Damaged),
                    }
                ),
                // ���� ���
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckAttacking),
                        new ActionNode(CheckAttackRange),
                        new ActionNode(DoAttack),
                    }
                ),
                // ���� ���
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckFind),
                        new ActionNode(Chase),
                    }
                ),
                // ���� ���
                //new ActionNode(Return),
                // ����, Idle ���
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
    public void SetDamage(int damage)
    {
        if (_temporaryDamage != 0)
        {
            _temporaryDamage = damage;
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
        if (isMove)
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
                if (!IsAnimationRunning(_MOVE_ANIM_STATE_NAME))
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
    protected void PlayDamagedSound()
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
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }
    protected INode.ENodeState Die()
    {
        _anim.SetTrigger(_DIE_ANIM_TRIGGER_NAME);

        return INode.ENodeState.ENS_Success;
    }
    protected INode.ENodeState CheckDieAnim()
    {
        if (IsAnimationRunning(_DIE_ANIM_STATE_NAME))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }
    protected INode.ENodeState DestroyObject()
    {
        Destroy(gameObject);

        return INode.ENodeState.ENS_Success;
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

    #region Skill Node
    void UseSkill() // ��ų ���̿� ���� ��Ÿ�� �߰�
    {
        foreach(BearBossSkill s in skill)
        {
            s.currentCooldown += 5f;
        }
    }
    void RayCast()
    {
        Debug.DrawRay(transform.position, gameObject.transform.forward * 15f, new Color(0, 1, 0));
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gameObject.transform.forward, out hit, 15f))
        {
            scan = hit.collider.gameObject;
        }
        else
        {
            scan = null;
        }

    }
    // ���� ����
    INode.ENodeState Charge()
    {
        if (!skill[0].IsReady())
            return INode.ENodeState.ENS_Failure;

        // �齺��
        Debug.Log("����");

        if (!isCharging)
        {
            _anim.SetTrigger(_BACKSTEP_ANIM_TRIGGER_NAME);

            _agent.isStopped = true;

            isCharging = true;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime < waitDuration)
        {
            Debug.Log("���� ��");
            // �÷��̾� �������� ȸ��
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 200f * Time.deltaTime);

            goal = (Player.transform.position - transform.position).normalized * 15f;

            return INode.ENodeState.ENS_Running;
        }
        else
        {
            return INode.ENodeState.ENS_Success;
        }
    }

    INode.ENodeState Rush()
    {
        // ����
        Debug.Log("����");

        _anim.SetTrigger(_RUSH_ANIM_TRIGGER_NAME);

        rushTriggerCollision.SetActive(true);

        _agent.enabled = false;
        transform.Translate(goal * Time.deltaTime, Space.World);

        transform.forward = goal * 10f;

        skillTime += Time.deltaTime;

        if (skillTime >= 1f)
        {
            _agent.enabled = true;
            _agent.speed = moveSpeed;

            UseSkill();
            skill[0].SetCooldown();
            isCharging = false;
            elapsedTime = 0f;
            skillTime = 0f;
            rushTriggerCollision.SetActive(false);

            return INode.ENodeState.ENS_Success;
        }
        
        return INode.ENodeState.ENS_Running;
    }
    // ���� ���� - ũ�� ���� ��, ���� �ð� �ڿ� �÷��̾� ��ġ�� ����
    INode.ENodeState Jump()
    {
        if (!skill[1].IsReady())
            return INode.ENodeState.ENS_Failure;

        // ����
        Debug.Log("����");

        return INode.ENodeState.ENS_Success;
    }
    INode.ENodeState Landing()
    {
        // ����
        Debug.Log("����");

        UseSkill();
        skill[1].SetCooldown();

        return INode.ENodeState.ENS_Success;
    }
    // ������� ���� - �÷��̾� �������� �� ����� �߻�
    INode.ENodeState Stamp()
    {
        if (!skill[2].IsReady())
            return INode.ENodeState.ENS_Failure;

        // �������
        Debug.Log("�������");

        UseSkill();
        skill[2].SetCooldown();

        return INode.ENodeState.ENS_Success;
    }
    // ��ų ��ҵ��� ��, ��ų ���� ��Ȳ �ʱ�ȭ
    INode.ENodeState initSkill()
    {
        elapsedTime = 0f;
        isCharging = false;
        skillTime = 0f;
        _agent.enabled = true;

        return INode.ENodeState.ENS_Failure;
    }
    #endregion
}
