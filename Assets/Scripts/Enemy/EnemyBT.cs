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
    [SerializeField] float moveSpeed = 9f; // Enemy 이동 속도
    [SerializeField] float findDistance = 30f; // 플레이어 발견 범위
    [SerializeField] float attackDistance = 4f; // 플레이어 공격 범위

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true일 경우 공격형 적대 몬스터
    //[SerializeField] int attackPower = 3; // Enemy 공격력


    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // 피격 사운드

    NavMeshAgent _agent; //네비게이션

    Vector3 _originPos;
    Quaternion _originRot;

    //애니메이터
    Animator _anim;

    BehaviorTreeRunner _BTRunner = null;

    const string _ATTACK_ANIM_STATE_NAME = "Attack";
    const string _ATTACK_ANIM_TRIGGER_NAME = "attack";

    const string _DAMAGED_ANIM_STATE_NAME = "Damaged";
    const string _DAMAGED_ANIM_TRIGGER_NAME = "damaged";

    // 데미지 임시 변수
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
        // 공격 애니메이션이 실행 중이면
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
            // 플레이어가 공격 사거리 안에 들어왔으면
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
            // 공격 애니메이션 실행
            //_anim.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);

            //계속 플레이어 방향 바라보도록
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

        // overlapColliders가 1개 이상 -> 플레이어가 감지 됨
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
            // 공격 범위 사거리까지 이동 완료한 경우
            if (Vector3.SqrMagnitude(_playerTransform.position - transform.position) < attackDistance * attackDistance)
            {
                Debug.Log("Chase : Success");
                _agent.enabled = false;
                return INode.ENodeState.ENS_Success;
            }

            // 공격 범위 사거리까지 이동 중일 경우
            //transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, Time.deltaTime * moveSpeed);
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);

            Debug.Log("Chase : Running");
            return INode.ENodeState.ENS_Running;
        }

        // 플레이어를 발견 못한 경우
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
        // 피격 애니메이션이 실행 중이면
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
