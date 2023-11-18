using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] float moveSpeed = 6f; // Enemy 이동 속도
    [SerializeField] float findDistance = 10f; // 플레이어 발견 범위
    [SerializeField] float attackDistance = 4f; // 플레이어 공격 범위

    [Header("Patrol")]
    [SerializeField] float idlePatrolRange = 10f; // 랜덤 순찰 최대 거리
    [SerializeField] int idlePatrolNumber = 6; // 랜덤 순찰 지점 개수
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

    NavMeshAgent _agent; //네비게이션

    Vector3 _originPos;
    Quaternion _originRot;

    //애니메이터
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    // 순찰 지점 리스트
    List<Vector3> Patrol_Position;
    float _patrolCurrentTime = 0;
    int patrolNum = 0;

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

    // 데미지 임시 변수
    int _temporaryDamage = 0;

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
        currentHp = maxhp;
        _agent.updateRotation = false;

        SetPatrolPosition();

    }
    private void Update()
    {
        _patrolCurrentTime += Time.deltaTime;
        _BTRunner.Operate();

        // agent 회전각 구하기
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
    void SetPatrolPosition()
    {
        Patrol_Position = new List<Vector3>();

        for (int i = 0; i < idlePatrolNumber; i++)
        {
            List<int> dir = new List<int> { -1, 1 };
            int dir_x = Random.Range(0, dir.Count);
            int dir_z = Random.Range(0, dir.Count);
            float x = gameObject.transform.position.x + dir[dir_x] * Random.Range(5f, idlePatrolRange);
            float z = gameObject.transform.position.z + dir[dir_z] * Random.Range(5f, idlePatrolRange);

            Patrol_Position.Add(new Vector3(x, gameObject.transform.position.y, z));
        }
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

    INode.ENodeState CheckAttackRange()
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

    INode.ENodeState DoAttack()
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
    #endregion

    #region Find & Move Node
    INode.ENodeState CheckFind()
    {
        //Debug.Log("CheckFind");
        var overlapColliders = Physics.OverlapSphere(transform.position, findDistance, LayerMask.GetMask("Player"));

        // overlapColliders가 1개 이상 -> 플레이어가 감지 됨
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
    INode.ENodeState CheckDamaged()
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
    INode.ENodeState Patrol()
    {
        if (!isMove && _patrolCurrentTime >= patrolTime) // 순찰 시작 전, 처음에만 호출
        {
            _agent.isStopped = false;
            _agent.speed = patrolMoveSpeed;
            isMove = true; // 이동 상태로 설정

            // 순찰 지점 1개 뽑기
            patrolNum = Random.Range(0, idlePatrolNumber);
            _agent.SetDestination(Patrol_Position[patrolNum]);

            _anim.SetTrigger(_MOVE_ANIM_TRIGGER_NAME);

            return INode.ENodeState.ENS_Running;
        }

        // 이동이 완료되었을 때
        if (Vector3.SqrMagnitude(Patrol_Position[patrolNum] - transform.position) <= 0.5f)
        {
            isMove = false;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;

            if (_patrolCurrentTime >= patrolTime)
            {
                _patrolCurrentTime = 0;
                SetPatrolPosition();

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
