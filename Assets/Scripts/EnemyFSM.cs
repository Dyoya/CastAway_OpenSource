using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
    - 공격형 적대 몬스터 상태
    Idle, Move, Attack, Return, Damaged, Die

    - 도망형 몬스터 상태
    Idle, Move, Damaged, Die
 */

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    CharacterController _cc;

    public EnemyState m_State;
    
    Transform _playerTF;

    [Header("HP")]
    [SerializeField] int currentHp;
    [SerializeField] int maxhp = 15;
    [SerializeField] float healDelay = 1f;

    [Header("Distance")]
    [SerializeField] float moveSpeed = 9f; // Enemy 이동 속도
    [SerializeField] float findDistance = 30f; // 플레이어 발견 범위
    [SerializeField] float maxDistanceFromPlayer = 40f; //플레이어 추격 최대 범위
    [SerializeField] float attackDistance = 4f; // 플레이어 공격 범위
    [SerializeField] float knockbackDistance = 2f;

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true일 경우 공격형 적대 몬스터
    [SerializeField] float attackDelay = 1.5f;
    //[SerializeField] int attackPower = 3; // Enemy 공격력

    float _currentTime = 0;

    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // 피격 사운드

    NavMeshAgent _agent; //네비게이션

    Vector3 _originPos;
    Quaternion _originRot;

    //애니메이터
    Animator _anim;

    void Start()
    {
        m_State = EnemyState.Idle;
        _playerTF = GameObject.Find("Player").transform;

        _cc = GetComponent<CharacterController>();

        _originPos = transform.position;
        _originRot = transform.rotation;

        currentHp = maxhp;

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        _agent.speed = moveSpeed;

        //_anim = transform.GetComponentInChildren<Animator>();

        asDamagedSound = GetComponent<AudioSource>();
    }


    void Update()
    {
        _currentTime += Time.deltaTime;

        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:

                break;
            case EnemyState.Die:

                break;
        }
    }

    void Idle()
    {
        if (Vector3.Distance(transform.position, _playerTF.position) < findDistance)
        {
            _agent.enabled = true;
            m_State = EnemyState.Move;
            print("상태 전환: Idle -> Move !!");

            //_anim.SetTrigger("IdleToMove");
        }
        if (_currentTime > healDelay)
        {
            currentHp += 2;
            _currentTime = 0;
            if (currentHp > maxhp)
                currentHp = maxhp;
        }

    }
    void Move()
    {
        // 공격형 적대 몬스터라면
        if (isAttackEnemy)
        {
            //플레이어가 추격 범위를 벗어남
            if (Vector3.Distance(transform.position, _playerTF.position) > maxDistanceFromPlayer)
            {
                m_State = EnemyState.Return;
                print("상태 전환 : Move -> Return !!");
                //_anim.SetTrigger("MoveToReturn");
            }
            //플레이어가 추격 범위 안에 있음
            else if (Vector3.Distance(transform.position, _playerTF.position) > attackDistance)
            {
                //플레이어 추격
                _agent.enabled = true;
                _agent.SetDestination(_playerTF.position);
            }
            //플레이어가 공격 범위 안에 있음
            else
            {
                _agent.enabled = false;
                m_State = EnemyState.Attack;
                print("상태 전환: Move -> Attack !!");
                //_anim.SetTrigger("MoveToAttackdelay");
            }
        }
        // 도망형 몬스터라면
        else
        {
            // 플레이어가 추격 범위 안에 들어옴
            if (Vector3.Distance(transform.position, _playerTF.position) > attackDistance)
            {
                // 도망
                
            }
        }    
    }

    public IEnumerator AttackProcess() //
    {
        yield return new WaitForSeconds(0.5f);
        //_anim.SetTrigger("StartAttack");
        Vector3 temp = (_playerTF.position - transform.position).normalized;
        temp.y = 0;
        transform.forward = temp;
        print("공격!");
    }
    public void AttackAction()
    {
        //플레이어 체력 깎기
    }
    void Attack()
    {
        //플레이어가 공격 범위 안에 있으면
        if (Vector3.Distance(transform.position, _playerTF.position) < attackDistance)
        {
            //계속 플레이어 방향 바라보도록
            Vector3 temp = (_playerTF.position - transform.position).normalized;
            temp.y = 0;
            transform.forward = temp;

            if (_currentTime >= attackDelay)
            {
                //기본 공격
                StartCoroutine(AttackProcess());
                _currentTime = 0;
            }
        }
        else
        {
            _agent.enabled = true;
            m_State = EnemyState.Move;
            print("상태 전환: Attack -> Move !!");
            //_anim.SetTrigger("AttackToMove");
            _currentTime = 0;
        }
    }
    void Return()
    {
        if (Vector3.Distance(transform.position, _playerTF.position) < maxDistanceFromPlayer)
        {
            m_State = EnemyState.Move;
            print("상태 전환: Return -> Move !!");
            //_anim.SetTrigger("ReturnToMove");
        }
        else if (Vector3.Distance(transform.position, _originPos) > 0.5f)
        {
            _agent.SetDestination(_originPos);
        }
        else
        {
            transform.position = _originPos;
            transform.rotation = _originRot;
            //hp = maxhp;
            _agent.enabled = false;
            m_State = EnemyState.Idle;
            print("상태 전환 : Return -> Idle");
            //_anim.SetTrigger("ReturnToIdle");
        }
    }
    void Damaged()
    {
        Vector3 dir = (_playerTF.position - transform.position).normalized;
        dir.y = 0;
        dir *= knockbackDistance;

        //넉백
        float elapsedTime = 0;
        float knockbackDuration = 0.5f;
        while (elapsedTime < knockbackDuration)
        {
            _cc.SimpleMove(-dir);
            elapsedTime += Time.deltaTime;
        }

        StartCoroutine(DamageProcess());
    }
    //코루틴 함수
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);
        _agent.enabled = true;
        m_State = EnemyState.Move;
    }

    public void HitEnemy(int hitPower)
    {
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        currentHp -= hitPower;
        asDamagedSound.Play();
        if (currentHp > 0)
        {
            m_State = EnemyState.Damaged;
            _agent.enabled = false;
            Damaged();
            print("상태 전환: Any State -> Damaged");
            //_anim.SetTrigger("Damaged");
        }
        else
        {
            m_State = EnemyState.Die;
            print("상태 전환: Any State -> Die");
            //_anim.SetTrigger("Die");
            Die();
        }
    }
    void Die()
    {
        StopAllCoroutines();
        _agent.enabled = false;
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        _cc.enabled = false;

        yield return new WaitForSeconds(2f);
        print("소멸!");
        Destroy(gameObject);
    }
}