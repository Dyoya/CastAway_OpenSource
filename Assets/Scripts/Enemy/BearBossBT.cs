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
    [SerializeField] float moveSpeed = 6f; // Enemy 이동 속도
    [SerializeField] float findDistance = 10f; // 플레이어 발견 범위
    [SerializeField] float attackDistance = 4f; // 플레이어 공격 범위

    [Header("Patrol")]
    [SerializeField] float idlePatrolRange = 10f; // 랜덤 순찰 최대 거리
    [SerializeField] float patrolTime = 10f; // 순찰 지점 변경 시간
    [SerializeField] float patrolMoveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true일 경우 공격형 적대 몬스터
    //[SerializeField] int attackPower = 3; // Enemy 공격력
    [SerializeField] float attackCooldown = 2f; // 공격 쿨다운 시간
    private bool canAttack = true; // 다음 공격이 가능한지 여부를 나타내는 플래그
    private float lastAttackTime; // 마지막 공격 시간


    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // 피격 사운드

    [Header("SkillRangeObject")]
    [SerializeField] GameObject rushTriggerCollision;
    [SerializeField] GameObject RushRange;
    [SerializeField] GameObject[] StampRange;
    [SerializeField] GameObject EarthPrefab;
    [SerializeField] GameObject body;
    [SerializeField] GameObject LandRange;
    [SerializeField] GameObject LandTrigger;

    NavMeshAgent _agent; //네비게이션

    Vector3 _originPos;
    Quaternion _originRot;

    //애니메이터
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

    // 데미지 임시 변수
    int _temporaryDamage = 0;

    bool isMove = false;
    bool isReturn = false;

    // 플레이어 인식 상태
    bool findPlayer = false;

    // 스킬 패턴 관련 변수
    bool isCharging = false;
    float elapsedTime = 0f;
    float waitDuration = 3f; // 차지 시간
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
            new BearBossSkill("돌진", 10f),
            new BearBossSkill("점프", 16f),
            new BearBossSkill("찍기", 12f),
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

        // agent 회전각 구하기
        if(!isSkill) 
        {
            Vector3 lookrotation = _agent.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 5f * Time.deltaTime);
        }

        // 플레이어 발견 상태에서 스킬 쿨타임 업데이트
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
                // 사망 노드
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
                // 스킬 패턴 노드
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckFind),
                        new SelectorNode
                        (
                            new List<INode>()
                            {
                                // 돌진
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Charge),
                                        new ActionNode(Rush),
                                    }
                                ),
                                // 내려찍기
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(Stamp),
                                        new ActionNode(StampWait)
                                    }
                                ),
                                // 점프
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
                // 피격 노드
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDamaged),
                        new ActionNode(Damaged),
                    }
                ),
                // 공격 노드
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckAttacking),
                        new ActionNode(CheckAttackRange),
                        new ActionNode(DoAttack),
                    }
                ),
                // 추적 노드
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckFind),
                        new ActionNode(Chase),
                    }
                ),
                // 복귀 노드
                //new ActionNode(Return),
                // 순찰, Idle 노드
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
        // 이전 공격 후 일정 시간이 지나면 공격 가능으로 설정
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
            // 플레이어가 공격 사거리 안에 들어왔으면
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
            // 공격 애니메이션 실행
            _anim.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);

            // 플레이어 방향 바라보도록 설정
            Vector3 temp = (_playerTransform.position - transform.position).normalized;
            temp.y = 0;
            transform.forward = temp;

            // 다음 공격이 가능한지 여부 설정
            canAttack = false;
            lastAttackTime = Time.time;

            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Failure;
    }
    protected void TakeDamage()
    {
        // 플레이어한테 데미지 가함
    }
    #endregion

    #region Find & Move Node
    protected INode.ENodeState CheckFind()
    {
        //Debug.Log("CheckFind");
        var overlapColliders = Physics.OverlapSphere(transform.position, findDistance, LayerMask.GetMask("Player"));

        // overlapColliders가 1개 이상 -> 플레이어가 감지 됨
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
            // 공격 범위 사거리까지 이동 완료한 경우
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                //Debug.Log("Chase : Success");
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                isMove = false; // 이동 상태 해제
                return INode.ENodeState.ENS_Success;
            }

            if (!isMove) // 이동 중이 아닌 경우에만 Move 애니메이션 호출
            {
                if (!IsAnimationRunning(_MOVE_ANIM_STATE_NAME))
                    _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

                isMove = true; // 이동 상태로 설정
                isReturn = true; // 언제든지 복귀 가능
            }

            // 공격 범위 사거리까지 이동 중일 경우
            //transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, Time.deltaTime * moveSpeed);
            _agent.speed = moveSpeed;
            _agent.isStopped = false;
            _agent.SetDestination(_playerTransform.position);

            //Debug.Log("Chase : Running");
            return INode.ENodeState.ENS_Running;
        }

        // 플레이어를 발견 못한 경우
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
            // 복귀를 완료 했을 경우
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
        // 피격 애니메이션이 실행 중이면
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

        if (!isMove && _patrolCurrentTime >= patrolTime) // 순찰 시작 전, 처음에만 호출
        {
            _agent.isStopped = false;
            _agent.speed = patrolMoveSpeed;
            isMove = true; // 이동 상태로 설정

            // 순찰 지점 1개 뽑기
            float r = Random.Range(3, idlePatrolRange);
            Vector3 sphere = Random.insideUnitSphere.normalized;
            Vector3 patrolPosition = gameObject.transform.position + r * sphere;
            _agent.SetDestination(patrolPosition);

            _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

            return INode.ENodeState.ENS_Running;
        }


        // 이동이 완료되었을 때
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
    void UseSkill() // 스킬 사이에 내부 쿨타임 추가
    {
        foreach(BearBossSkill s in skill)
        {
            s.currentCooldown += Random.Range(3f, 6f);
        }
    }
    void Rotate()
    {
        // 플레이어 방향으로 회전
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 400f * Time.deltaTime);
    }
    // 돌진 패턴
    INode.ENodeState Charge()
    {
        if (!skill[0].IsReady() || isStamp || isJump)
            return INode.ENodeState.ENS_Failure;

        // 차지
        if (!isCharging)
        {
            //Debug.Log("차지");

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
            //Debug.Log("차지 중");

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
        // 돌진
        //Debug.Log("돌진");

        _anim.SetTrigger(_RUSH_ANIM_TRIGGER_NAME);

        RushRange.SetActive(false);

        rushTriggerCollision.SetActive(true);

        _agent.enabled = false;
        transform.Translate(goal * Time.deltaTime, Space.World);

        transform.forward = goal * 10f;

        skillTime += Time.deltaTime;

        if (skillTime >= 1f)
        {
            Debug.Log("돌진 종료");

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
    // 내려찍기 패턴 - 플레이어 방향으로 찍어서 충격파 발사
    INode.ENodeState Stamp()
    {
        if (!skill[2].IsReady() || isJump)
            return INode.ENodeState.ENS_Failure;

        // 내려찍기
        //Debug.Log("내려찍기");
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
            //Debug.Log("스탬프 동작");

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

        //Debug.Log("스탬프 종료");
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
        //Debug.Log("스탬프 공격!");

        for (int i = 0; i < StampRange.Length; i++)
        {
            Transform pos = StampRange[i].transform;

            StartCoroutine(SetEarth(StampRange[i], pos, i * 0.2f));
        }
        
    }
    IEnumerator SetEarth(GameObject go, Transform tf, float time)
    {
        yield return new WaitForSeconds(time);

        //Debug.Log("생성!");

        go = Instantiate(EarthPrefab, tf);

        StartCoroutine(DestoryEarth(go));
    }
    IEnumerator DestoryEarth(GameObject go)
    {
        yield return new WaitForSeconds(2f);

        //Debug.Log("파괴!");

        Destroy(go);
    }
    // 점프 패턴 - 크게 점프 후, 일정 시간 뒤에 플레이어 위치로 착지
    // 0~3초 : 하울링 / 3~4초 : 점프 / 4~8초 : 추적 / 8~9초 : 착지 / 9~11초 : 하울링
    INode.ENodeState Howling()
    {
        if (!skill[1].IsReady())
            return INode.ENodeState.ENS_Failure;

        isJump = true;

        if (!isCharging)
        {
            Debug.Log("점프 준비");

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
        // 점프 애니메이션 실행
        if (elapsedTime < 4f && !IsAnimationRunning(_JUMP_ANIM_STATE_NAME))
        {
            _anim.SetTrigger(_JUMP_ANIM_TRIGGER_NAME);
        }

        // 착지 1초 전 까지 플레이어 위치 추적
        if(elapsedTime < 7f)
            goal = Player.transform.position;

        if (elapsedTime < 4f)
        {
            Debug.Log("점프 중");

            // 점프
            transform.Translate(new Vector3(0, 10, 0) * Time.deltaTime, Space.World);

            return INode.ENodeState.ENS_Running;
        }
        else if (elapsedTime < 8f)
        {
            Debug.Log("착지 준비 중");
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
        // 착지
        Debug.Log("착지");

        body.SetActive(true);
        LandRange.SetActive(false);
        LandTrigger.SetActive(true);

        if (elapsedTime < 9f)
        {
            transform.Translate(new Vector3(0, -10, 0) * Time.deltaTime, Space.World);

            // 한 번만 애니메이션 실행
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

        // 종료
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
    // 스킬 취소됐을 때, 스킬 진행 상황 초기화
    INode.ENodeState initSkill()
    {
        // 공통
        elapsedTime = 0f;
        isCharging = false;
        skillTime = 0f;
        _agent.enabled = true;
        isSkill = false;

        // 내려찍기 초기화
        isStamp = false;

        return INode.ENodeState.ENS_Failure;
    }
    #endregion
}
