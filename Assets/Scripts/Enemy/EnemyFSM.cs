using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
    - ������ ���� ���� ����
    Idle, Move, Attack, Return, Damaged, Die

    - ������ ���� ����
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
    [SerializeField] float moveSpeed = 9f; // Enemy �̵� �ӵ�
    [SerializeField] float findDistance = 30f; // �÷��̾� �߰� ����
    [SerializeField] float maxDistanceFromPlayer = 40f; //�÷��̾� �߰� �ִ� ����
    [SerializeField] float attackDistance = 4f; // �÷��̾� ���� ����
    [SerializeField] float knockbackDistance = 2f;

    [Header("Attack")]
    [SerializeField] bool isAttackEnemy; //true�� ��� ������ ���� ����
    [SerializeField] float attackDelay = 1.5f;
    //[SerializeField] int attackPower = 3; // Enemy ���ݷ�

    float _currentTime = 0;

    [Header("Sound")]
    [SerializeField] AudioSource asDamagedSound; // �ǰ� ����

    NavMeshAgent _agent; //�׺���̼�

    Vector3 _originPos;
    Quaternion _originRot;

    //�ִϸ�����
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
            print("���� ��ȯ: Idle -> Move !!");

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
        // ������ ���� ���Ͷ��
        if (isAttackEnemy)
        {
            //�÷��̾ �߰� ������ ���
            if (Vector3.Distance(transform.position, _playerTF.position) > maxDistanceFromPlayer)
            {
                m_State = EnemyState.Return;
                print("���� ��ȯ : Move -> Return !!");
                //_anim.SetTrigger("MoveToReturn");
            }
            //�÷��̾ �߰� ���� �ȿ� ����
            else if (Vector3.Distance(transform.position, _playerTF.position) > attackDistance)
            {
                //�÷��̾� �߰�
                _agent.enabled = true;
                _agent.SetDestination(_playerTF.position);
            }
            //�÷��̾ ���� ���� �ȿ� ����
            else
            {
                _agent.enabled = false;
                m_State = EnemyState.Attack;
                print("���� ��ȯ: Move -> Attack !!");
                //_anim.SetTrigger("MoveToAttackdelay");
            }
        }
        // ������ ���Ͷ��
        else
        {
            // �÷��̾ �߰� ���� �ȿ� ����
            if (Vector3.Distance(transform.position, _playerTF.position) > attackDistance)
            {
                // ����
                
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
        print("����!");
    }
    public void AttackAction()
    {
        //�÷��̾� ü�� ���
    }
    void Attack()
    {
        //�÷��̾ ���� ���� �ȿ� ������
        if (Vector3.Distance(transform.position, _playerTF.position) < attackDistance)
        {
            //��� �÷��̾� ���� �ٶ󺸵���
            Vector3 temp = (_playerTF.position - transform.position).normalized;
            temp.y = 0;
            transform.forward = temp;

            if (_currentTime >= attackDelay)
            {
                //�⺻ ����
                StartCoroutine(AttackProcess());
                _currentTime = 0;
            }
        }
        else
        {
            _agent.enabled = true;
            m_State = EnemyState.Move;
            print("���� ��ȯ: Attack -> Move !!");
            //_anim.SetTrigger("AttackToMove");
            _currentTime = 0;
        }
    }
    void Return()
    {
        if (Vector3.Distance(transform.position, _playerTF.position) < maxDistanceFromPlayer)
        {
            m_State = EnemyState.Move;
            print("���� ��ȯ: Return -> Move !!");
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
            print("���� ��ȯ : Return -> Idle");
            //_anim.SetTrigger("ReturnToIdle");
        }
    }
    void Damaged()
    {
        Vector3 dir = (_playerTF.position - transform.position).normalized;
        dir.y = 0;
        dir *= knockbackDistance;

        //�˹�
        float elapsedTime = 0;
        float knockbackDuration = 0.5f;
        while (elapsedTime < knockbackDuration)
        {
            _cc.SimpleMove(-dir);
            elapsedTime += Time.deltaTime;
        }

        StartCoroutine(DamageProcess());
    }
    //�ڷ�ƾ �Լ�
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
            print("���� ��ȯ: Any State -> Damaged");
            //_anim.SetTrigger("Damaged");
        }
        else
        {
            m_State = EnemyState.Die;
            print("���� ��ȯ: Any State -> Die");
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
        print("�Ҹ�!");
        Destroy(gameObject);
    }
}