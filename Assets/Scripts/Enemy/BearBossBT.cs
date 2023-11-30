using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEditor.PlayerSettings;

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
    [SerializeField] GameObject RushRange;
    [SerializeField] GameObject[] StampRange;
    [SerializeField] GameObject EarthPrefab;
    [SerializeField] GameObject body;
    [SerializeField] GameObject LandRange;
    [SerializeField] GameObject LandTrigger;

    NavMeshAgent _agent; //�׺���̼�

    Vector3 _originPos;
    Quaternion _originRot;

    //�ִϸ�����
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    List<BearBossSkill> skill;

    float _patrolCurrentTime = 0;

    #region Anim Name
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

    const string _STAMP_ANIM_STATE_NAME = "Stamp";
    const string _STAMP_ANIM_TRIGGER_NAME = "stamp";

    const string _JUMP_ANIM_STATE_NAME = "Jump";
    const string _JUMP_ANIM_TRIGGER_NAME = "jump";

    const string _LANDING_ANIM_STATE_NAME = "Landing";
    const string _LANDING_ANIM_TRIGGER_NAME = "landing";

    const string _HOWLING_ANIM_STATE_NAME = "Howling";
    const string _HOWLING_ANIM_TRIGGER_NAME = "howling";
    #endregion

    // ������ �ӽ� ����
    int _temporaryDamage = 0;

    bool isMove = false;
    bool isReturn = false;

    // �÷��̾� �ν� ����
    bool findPlayer = false;

    // ��ų ���� ���� ����
    bool isCharging = false;
    float elapsedTime = 0f;
    float waitDuration = 3f; // ���� �ð�
    float skillTime = 0f;
    Vector3 goal;
    bool isSkill = false;
    bool isStamp = false;
    bool isJump = false;
    bool doLandAnim = false;
    Vector3 jumpVector;

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
            new BearBossSkill("����", 16f),
            new BearBossSkill("���", 12f),
        };

        RushRange.SetActive(false);
        foreach (GameObject stamp in StampRange)
        {
            stamp.SetActive(false);
        }
    }
    private void Update()
    {
        _patrolCurrentTime += Time.deltaTime;
        _BTRunner.Operate();

        // agent ȸ���� ���ϱ�
        if(!isSkill) 
        {
            Vector3 lookrotation = _agent.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 5f * Time.deltaTime);
        }

        // �÷��̾� �߰� ���¿��� ��ų ��Ÿ�� ������Ʈ
        if(findPlayer)
        {
            foreach (BearBossSkill s in skill)
            {
                s.UpdateCooldown(Time.deltaTime * 1f);
            }
        }
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
                                // ����
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Charge),
                                        new ActionNode(Rush),
                                    }
                                ),
                                // �������
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Stamp),
                                        new ActionNode(StampWait)
                                    }
                                ),
                                // ����
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Howling),
                                        new ActionNode(Jump),
                                        new ActionNode(Landing),
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
    protected void TakeDamage()
    {
        // �÷��̾����� ������ ����
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

            findPlayer = true;

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
        findPlayer = false;

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
            s.currentCooldown += Random.Range(3f, 6f);
        }
    }
    void Rotate()
    {
        // �÷��̾� �������� ȸ��
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 400f * Time.deltaTime);
    }
    // ���� ����
    INode.ENodeState Charge()
    {
        if (!skill[0].IsReady() || isStamp || isJump)
            return INode.ENodeState.ENS_Failure;

        // ����
        if (!isCharging)
        {
            //Debug.Log("����");

            _anim.SetTrigger(_BACKSTEP_ANIM_TRIGGER_NAME);

            isSkill = true;
            isMove = false;
            _agent.isStopped = true;

            RushRange.SetActive(true);

            isCharging = true;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime < waitDuration)
        {
            //Debug.Log("���� ��");

            Rotate();
            goal = (Player.transform.position - transform.position).normalized * 20f;

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
        //Debug.Log("����");

        _anim.SetTrigger(_RUSH_ANIM_TRIGGER_NAME);

        RushRange.SetActive(false);

        rushTriggerCollision.SetActive(true);

        _agent.enabled = false;
        transform.Translate(goal * Time.deltaTime, Space.World);

        transform.forward = goal * 10f;

        skillTime += Time.deltaTime;

        if (skillTime >= 1f)
        {
            Debug.Log("���� ����");

            _agent.enabled = true;
            _agent.speed = moveSpeed;
            isSkill = false;

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
    // ������� ���� - �÷��̾� �������� �� ����� �߻�
    INode.ENodeState Stamp()
    {
        if (!skill[2].IsReady() || isJump)
            return INode.ENodeState.ENS_Failure;

        // �������
        //Debug.Log("�������");
        isStamp = true;

        if (!isCharging)
        {
            _anim.SetTrigger(_STAMP_ANIM_TRIGGER_NAME);

            isSkill = true;
            isMove = false;
            _agent.isStopped = true;

            isCharging = true;

            for(int i=0; i<StampRange.Length; i++)
            {
                StartCoroutine(SetRange(StampRange[i], i * 0.2f, true));
            }
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime < 1.4f)
        {
            //Debug.Log("������ ����");

            Rotate();

            return INode.ENodeState.ENS_Running;
        }
        else
        {
            return INode.ENodeState.ENS_Success;
        }

    }
    IEnumerator SetRange(GameObject go, float time, bool active)
    {
        yield return new WaitForSeconds(time);

        go.SetActive(active);
    }
    INode.ENodeState StampWait()
    {
        if (elapsedTime < 5f)
            return INode.ENodeState.ENS_Running;

        //Debug.Log("������ ����");
        foreach (GameObject go in StampRange)
            StartCoroutine(SetRange(go, 0f, false));

        UseSkill();
        skill[2].SetCooldown();
        isSkill = false;
        elapsedTime = 0f;
        skillTime = 0f;
        _agent.enabled = true;

        isStamp = false;

        return INode.ENodeState.ENS_Success;
    }
    void StampAttack()
    {
        //Debug.Log("������ ����!");

        for (int i = 0; i < StampRange.Length; i++)
        {
            Transform pos = StampRange[i].transform;

            StartCoroutine(SetEarth(StampRange[i], pos, i * 0.2f));
        }
        
    }
    IEnumerator SetEarth(GameObject go, Transform tf, float time)
    {
        yield return new WaitForSeconds(time);

        //Debug.Log("����!");

        go = Instantiate(EarthPrefab, tf);

        StartCoroutine(DestoryEarth(go));
    }
    IEnumerator DestoryEarth(GameObject go)
    {
        yield return new WaitForSeconds(2f);

        //Debug.Log("�ı�!");

        Destroy(go);
    }
    // ���� ���� - ũ�� ���� ��, ���� �ð� �ڿ� �÷��̾� ��ġ�� ����
    // 0~3�� : �Ͽ︵ / 3~4�� : ���� / 4~8�� : ���� / 8~9�� : ���� / 9~11�� : �Ͽ︵
    INode.ENodeState Howling()
    {
        if (!skill[1].IsReady())
            return INode.ENodeState.ENS_Failure;

        isJump = true;

        if (!isCharging)
        {
            Debug.Log("���� �غ�");

            _anim.SetTrigger(_HOWLING_ANIM_TRIGGER_NAME);

            isSkill = true;
            isMove = false;
            _agent.enabled = false;

            isCharging = true;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime > 3f)
        {
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Running;
    }
    INode.ENodeState Jump()
    {
        // ���� �ִϸ��̼� ����
        if (elapsedTime < 4f && !IsAnimationRunning(_JUMP_ANIM_STATE_NAME))
        {
            _anim.SetTrigger(_JUMP_ANIM_TRIGGER_NAME);
        }

        // ���� 1�� �� ���� �÷��̾� ��ġ ����
        if(elapsedTime < 7f)
            goal = Player.transform.position;

        if (elapsedTime < 4f)
        {
            Debug.Log("���� ��");

            // ����
            transform.Translate(new Vector3(0, 10, 0) * Time.deltaTime, Space.World);

            return INode.ENodeState.ENS_Running;
        }
        else if (elapsedTime < 8f)
        {
            Debug.Log("���� �غ� ��");
            body.SetActive(false);
            LandRange.SetActive(true);

            LandRange.transform.position = new Vector3(goal.x, goal.y + 0.002f, goal.z);

            transform.position = new Vector3(goal.x, goal.y + 10f, goal.z);

            return INode.ENodeState.ENS_Running;
        }
        else
        {
            return INode.ENodeState.ENS_Success;
        }
    }
    INode.ENodeState Landing()
    {
        // ����
        Debug.Log("����");

        body.SetActive(true);
        LandRange.SetActive(false);
        LandTrigger.SetActive(true);

        if (elapsedTime < 9f)
        {
            transform.Translate(new Vector3(0, -10, 0) * Time.deltaTime, Space.World);

            // �� ���� �ִϸ��̼� ����
            if (!doLandAnim)
            {
                _anim.SetTrigger(_LANDING_ANIM_TRIGGER_NAME);

                doLandAnim = true;
            }

            return INode.ENodeState.ENS_Running;
        }
        else if (elapsedTime < 11.5f)
        {
            if (!IsAnimationRunning(_HOWLING_ANIM_STATE_NAME))
            {
                _anim.SetTrigger(_HOWLING_ANIM_TRIGGER_NAME);
            }

            return INode.ENodeState.ENS_Running;
        }

        // ����
        transform.position = goal;

        LandTrigger.SetActive(false);

        isSkill = false;
        _agent.enabled = true;

        isCharging = false;

        doLandAnim = false;

        UseSkill();
        skill[1].SetCooldown();
        elapsedTime = 0f;

        isStamp = false;


        return INode.ENodeState.ENS_Success;
    }
    // ��ų ��ҵ��� ��, ��ų ���� ��Ȳ �ʱ�ȭ
    INode.ENodeState initSkill()
    {
        // ����
        elapsedTime = 0f;
        isCharging = false;
        skillTime = 0f;
        _agent.enabled = true;
        isSkill = false;

        // ������� �ʱ�ȭ
        isStamp = false;

        return INode.ENodeState.ENS_Failure;
    }
    #endregion
}
