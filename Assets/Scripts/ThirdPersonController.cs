using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using static Item;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine.Animations;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using System.Collections.Generic;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDPickAxe; // ��̿� �ʿ��� �ִϸ��̼� �߰��� �κ�
        private int _animIDDeath; // �����̰� ���� �߰���
        private int _animIDGetItem; // �������� �ݴ� �ִϸ��̼� �߰��� �κ�
        private int _animIDAxe; // ���� �ִϸ��̼� �߰��� �κ�
        private int _animIDAttack; // ���ݿ� �ʿ��� �ִϸ��̼�
        private int _animIDFishing; // ���ÿ� �ʿ��� �ִϸ��̼�


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        //�����̰� ���� �߰��� ����(���׹̳� ���� ����)
        public HungryBar hungryBar;
        public HealthBar healthBar;
        public EnergyBar energyBar;

        //������ ��뿡 �ʿ��� ����
        [SerializeField]
        private slot Leftslot;
        [SerializeField]
        private slot Rightslot;

        private PlayableDirector pd;
        [SerializeField] TimelineAsset[] ta;
        [SerializeField] GameObject Helicopter;

        private int HandPosition;

        //������ �� �� ����� ����
        private bool isPickAxe = false;
        private bool isPickAxeDirection = false;
        private bool isJump = false;
        private bool isDead = false;
        private bool isGetItem = false;
        private bool isGetItemDirection = false;
        private bool isAxe = false;
        private bool isAxeDirection = false;
        private bool isAttack = false;
        private bool isAttackDirection = false;
        private bool isFishing = false;


        //���� ������Ʈ Ȱ��ȭ ����
        public GameObject[] objectToActivate;
        public GameObject FishingUI;

        //������ ���� ����
        private bool hasAxe = false;
        private bool hasPickAxe = false;
        private bool hasString = false;
        private bool hasFishing = false;
        private bool hasPerfactFishing = false;

        //Zone ��ġ�� �ִ��� ������
        private bool FishingZone = false;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            pd = GetComponent<PlayableDirector>();
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            if (isPickAxeDirection || isGetItemDirection || isAxeDirection)
                return;

            if (isDead)
                return;
            
            JumpAndGravity();
            GroundedCheck();
            Move();

            CheckItemdistance();
            Death();
            StaminaControll();
            InvenControll();
            PlayHealthControll();
            ItemActiveAnimation();
        }

        //Player�� ������ �޴� �Լ�
        public void AttackDamage(int damage)
        {
            healthBar.DecreaseHealth(damage);
        }

        private void ItemActiveAnimation()
        {
            Debug.Log(FishingZone);
            Debug.Log(hasPerfactFishing);
            if (hasAxe)
            {
                for(int i = 0; i < objectToActivate.Length; i++)
                {
                    if(objectToActivate[i].name == "����")
                        objectToActivate[i].SetActive(true);
                }
                Axe();
            }
            else if (hasPickAxe)
            {
                for (int i = 0; i < objectToActivate.Length; i++)
                {
                    if (objectToActivate[i].name == "���")
                        objectToActivate[i].SetActive(true);
                }
                PickAxe();
            }
            else if(hasPerfactFishing && FishingZone)
            {
                Debug.Log(hasPerfactFishing);
                for (int i = 0; i < objectToActivate.Length; i++)
                {
                    if (objectToActivate[i].name == "������ ���ô�")
                        objectToActivate[i].SetActive(true);
                }
                if (!isFishing && Input.GetKeyDown(KeyCode.F))
                    StartFishing();
                else if(isFishing)
                    Fishing();
            }
            else
            {
                for (int i = 0; i < objectToActivate.Length; i++)
                {
                       objectToActivate[i].SetActive(false);
                }
                Attack();
            }
        }

        private bool isSwing = false;
        private RaycastHit hitInfo;
        private float maxDistance = 1f;

        private bool CheckObject()
        {
            Vector3 startRay = transform.position + new Vector3(0, 1, 0);
            Vector3 direction = transform.forward;
            float rayAngle = 45f; 
            int totalRays = 10;

            for (int i = 0; i < totalRays; i++)
            {
                float angle = -rayAngle / 2 + (rayAngle / (totalRays - 1)) * i;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * direction;

                Ray ray = new Ray(startRay, dir);

                if (Physics.Raycast(ray, out hitInfo, maxDistance))
                {
                    Debug.DrawRay(startRay, dir * hitInfo.distance, Color.blue);
                    Debug.Log(hitInfo.transform.tag);
                    if (hitInfo.transform.tag == "Rock" && hasPickAxe)
                    {
                        return true;
                    }
                    if (hitInfo.transform.tag == "Tree" && hasAxe)
                    {
                        Debug.Log(hitInfo.transform.tag);
                        return true;
                    }
                }
            }
            return false;
        }

        protected IEnumerator HitCoroutine()
        {
            while (isSwing)
            {
                if(CheckObject())
                {
                    if(hitInfo.transform.tag == "Rock")
                    {
                        hitInfo.transform.GetComponent<Rock>().Mining();
                    }
                    if (hitInfo.transform.tag == "Tree")
                    {
                        Debug.Log(hitInfo.transform.tag);
                        hitInfo.transform.GetComponent<Tree>().Mining();
                    }
                    isSwing = false;
                }
                yield return null;
            }
        }

        private void StaminaControll()
        {
            //�޸� ���� �������� ���׹̳� ���ҷ� �ٸ��� ������ �κ�
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                hungryBar.DecreaseHungry(0.5f);
                energyBar.RecoverStamina();
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                hungryBar.DecreaseHungry(1.5f);
                energyBar.DecreaseStamina();
            }
        }

        private void InvenControll()
        {
            //����ġ 1, 2��Ű�� ����
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("1�� �κ�");
                HandPosition = 0;
                HandInfoAppear("�޼�");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("2�� �κ�");
                HandPosition = 1;
                HandInfoAppear("������");
            }

            //����ġ�� ���� ������ �Ա�
            if (Input.GetKeyDown(KeyCode.Q) && HandPosition == 0)
            {
                Leftslot.LeftHanduseItem(HandPosition);
            }
            else if (Input.GetKeyDown(KeyCode.Q) && HandPosition == 1)
            {
                Rightslot.RightHanduseItem(HandPosition);
            }

            //����ġ�� ���� ������
            if (Input.GetKeyDown(KeyCode.G) && HandPosition == 0)
            {
                Debug.Log("1�� �κ�");
                string leftItemName = Leftslot.GetItemName();
                if (hasPerfactFishing && FishingZone)
                {
                    dialogueQueue.Enqueue("���� ���ø� �ϰ� �־ �������� ���� �� ����");
                    StartCoroutine(DialogueUIAppear());
                }
                else
                {
                    ItemBool(leftItemName);
                    Leftslot.LeftHandThrowItem(HandPosition, leftItemName);
                }
            }
            else if (Input.GetKeyDown(KeyCode.G) && HandPosition == 1)
            {
                Debug.Log("2�� �κ�");
                string rightItemName = Leftslot.GetItemName();
                if (hasPerfactFishing && FishingZone)
                {
                    dialogueQueue.Enqueue("���� ���ø� �ϰ� �־ �������� ���� �� ����");
                    StartCoroutine(DialogueUIAppear());
                }
                else
                {
                    ItemBool(rightItemName);
                    Rightslot.RightHandThrowItem(HandPosition, rightItemName);
                }
            }
        }

        private void ItemBool(string itemName)
        {
            if (itemName == "���")
            {
                hasPickAxe = false;
            }
            else if (itemName == "����")
            {
                hasAxe = false;
            }
            else if (itemName == "������")
            {
                hasString = false;
            }
            else if (itemName == "���ô�")
            {
                hasFishing = false;
            }
            else if (itemName == "������ ���ô�")
            {
                hasPerfactFishing = false;
            }
        }

        private void PlayHealthControll()
        {
            //�÷��̾��� ������� 0�� �Ǿ��� ��
            if (hungryBar.isHungryZero)
            {
                healthBar.ZeroHungry();
            }
            else if (hungryBar.Pb.BarValue >= 80)
            {
                healthBar.IncreaseHealth();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDPickAxe = Animator.StringToHash("PickAxe");
            _animIDDeath = Animator.StringToHash("Death");
            _animIDGetItem = Animator.StringToHash("GetItem");
            _animIDAxe = Animator.StringToHash("Axe");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDFishing = Animator.StringToHash("Fishing");
        }

        private void Fishing()
        {
            _input.attack = false;
            _input.jump = false;
            Debug.Log("Fishing");
            if (_hasAnimator && Grounded && !isJump && FishingZone)
            {
                _controller.Move(Vector3.zero);
                _animator.SetBool(_animIDFishing, true);

                isFishing = true;
                if(isFishing && Input.GetKeyDown(KeyCode.F))
                    EndFishing();
                return;
            }
        }

        private void EndFishing()
        {
            if (_hasAnimator && isFishing)
            {
                Debug.Log("animation EndFishing");
                _animator.SetBool(_animIDFishing, false);
            }
            isFishing = false;
            FishingUI.SetActive(false);
        }

        private void StartFishing()
        {
            isFishing = true;
            FishingUI.SetActive(true);
        }


        private void Attack()
        {
            if (_hasAnimator && Grounded && !isJump && !isAttack && _input.attack)
            {
                _controller.Move(Vector3.zero);
                _animator.SetTrigger(_animIDAttack);
                isAttack = true;
                isSwing = true;
                isAttackDirection = true;
                //AttackDamage();
                return;
            }
        }

        private void EndAttack()
        {
            isAttack = false;
            isSwing = false;
            _input.attack = false;
        }

        private void EndAttackDirection()
        {
            isAttackDirection = false;
        }

        //���� �ִϸ��̼� ����
        private void Axe()
        {
            if (_hasAnimator && Grounded && !isJump && !isAxe && _input.attack)
            {
                _controller.Move(Vector3.zero);
                _animator.SetTrigger(_animIDAxe);
                isAxe = true;
                isSwing = true;
                isAxeDirection = true;
                return;
            }
        }

        private void EndAxe()
        {
            isAxe = false;
            isSwing = false;
            _input.attack = false;
        }

        private void EndAxeDirection()
        {
            isAxeDirection = false;
        }

        //�����Լ� �߰��� �κ�
        private void PickAxe()
        {
            if (_hasAnimator && Grounded && !isJump && !isPickAxe && _input.attack)
            {
                _controller.Move(Vector3.zero);
                _animator.SetTrigger(_animIDPickAxe);
                isPickAxe = true;
                isSwing = true;
                isPickAxeDirection = true;
                return;
            }
        }

        private void EndPickAxe()
        {
            isPickAxe = false;
            isSwing = false;
            _input.attack = false;
        }

        private void EndPickAxeDirection()
        {
            isPickAxeDirection = false;
        }

        //�÷��̾� �״� �ִϸ��̼� �߰��� �κ�    
        private void Death()
        {
            if (_hasAnimator && Grounded && !isJump && !isDead && healthBar.Pb.BarValue == 0)
            {
                _controller.Move(Vector3.zero);
                _animator.SetTrigger(_animIDDeath);
                isDead = true;
                StartDeathAnimation();
                return;
            }
        }

        //3���� �����
        private void StartDeathAnimation()
        {
            StartCoroutine(RemoveObjectAfterDelay());
        }

        private IEnumerator RemoveObjectAfterDelay()
        {
            yield return new WaitForSeconds(3F);
            Destroy(gameObject);
        }

        public void GetItem()
        {
            if (_hasAnimator && Grounded && !isJump && !isGetItem && _input.getItem)
            {
                _controller.Move(Vector3.zero);
                _animator.SetTrigger(_animIDGetItem);
                isGetItem = true;
                isGetItemDirection = true;
            }
        }

        private void EndGetItem()
        {
            isGetItem = false;
            _input.getItem = false;
        }

        private void EndGetItemDirection()
        {
            isGetItemDirection = false;
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            //������ �� ���� �Լ� Ż�� �߰��� �κ�
            if (isPickAxe || isDead || isGetItem || isAxe || isAttack || isFishing)
                return;

            //�÷��̾��� �������ٰ� 0�� ��� �� �ٵ��� �߰��� �κ� 
            float targetSpeed = MoveSpeed;
            if (_input.sprint && energyBar.Pb.BarValue > 0)
            {
                targetSpeed = SprintSpeed;
            }
            else if (!_input.sprint && energyBar.Pb.BarValue <= 0)
            {
                targetSpeed = MoveSpeed;
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                    isJump = false;
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                        isJump = true;
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private bool pickupActivated = true;

        [SerializeField]
        private TextMeshProUGUI actionText; // �ʿ� ������Ʈ

        [SerializeField]
        private TextMeshProUGUI conversationText; // �ʿ� ������Ʈ

        [SerializeField]
        private InventoryUI inventory;

        [SerializeField]
        private Image textImage;

        [SerializeField]
        private Image conversationImage;

        [SerializeField]
        private GameObject completeFishingRodPrefab;

        private List<GameObject> triggerItems = new List<GameObject>();

        private Queue<string> dialogueQueue = new Queue<string>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Helicopter")
            {
                Helicopter.SetActive(true);
                other.gameObject.SetActive(false);
                pd.Play(ta[0]);
            }
            if (other.tag == "Escape")
            {
                HelicopterController otherScript = GameObject.Find("Helicopter").GetComponent<HelicopterController>();
                pd.Play(ta[1]);
                otherScript.EscapeHelicopter();
                other.gameObject.SetActive(false);
            }
            if (other.gameObject.tag == "Item")
            {
                triggerItems.Add(other.gameObject);
                string itemName = other.gameObject.name;
                if (pickupActivated)
                    ItemInfoAppear(itemName + " ������ �ݱ� (E)");
                _input.getItem = false;
            }
            if (other.gameObject.tag == "FishingZone")
            {
                if(!hasPerfactFishing)
                {
                    conversationAppear("������ ���ô밡 ����... ���� ���鼭 �ٴٿ� �۾����� ������ ã�Ƽ� ������ ���ô븦 ����� ����!!");
                }
                else
                {
                    FishingZone = true;
                    dialogueQueue.Enqueue("���⼭ ���� �� �� �ְڴµ�? ���⼭ ���ø� ����");
                    StartCoroutine(DialogueUIAppear());
                }
            }
        }

        private void CheckItemdistance()
        {
            if (_input.getItem)
            {
                if (triggerItems.Count > 0)
                {
                    GameObject closestItem = null;
                    float closestDistance = Mathf.Infinity;

                    foreach (GameObject item in triggerItems)
                    {
                        float distance = (item.transform.position - transform.position).sqrMagnitude;
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestItem = item;
                        }
                    }

                    Collider other = closestItem.GetComponent<Collider>();

                    if (closestItem != null && closestItem == other.gameObject)
                    {
                        string itemName = other.gameObject.name;
                        bool hasAxeOrPickAxe = (itemName == "����" && hasPickAxe) || (itemName == "���" && hasAxe);
                        
                        if(hasAxeOrPickAxe)
                        {
                            disappear();
                            ItemInfoAppear("�� ������ ������ �ִ� ���� �ٸ� ������ �ֿ� �� ����!");
                        }
                        else
                        {
                            int isAcquired = inventory.AcquireItem(other.gameObject.GetComponent<ItemPickup>().item);
                            CheckInventoryForItems(other.gameObject);
                            if (isAcquired == 0)
                            {
                                GetItem();
                                Destroy(other.gameObject);
                                triggerItems.Remove(other.gameObject);
                                pickupActivated = true;
                                ItemInfoDisappear();
                                if (hasString || hasFishing)
                                {
                                    dialogueQueue.Enqueue("���⿡ �� " + itemName + "�� ����...? �ٴٿ��� ���з� �Գ�... ��ã�ƺ��� �ٸ� �͵� ������ ����!! ã�ƺ���!");
                                    StartCoroutine(DialogueUIAppear());
                                }
                                if (hasString && hasFishing)
                                {
                                    dialogueQueue.Enqueue("���ô밡 �����ϱ� ����⸦ ���� �� �ְڴ� ���̳� �غ����� ������!! ");
                                    StartCoroutine(DialogueUIAppear());
                                }
                            }
                            if (isAcquired == 1)
                            {
                                disappear();
                                ItemInfoAppear(other.gameObject.name + "���� ���� ���� �� �̻� �ֿ� �� ����!");
                            }
                            if (isAcquired == 2)
                            {
                                disappear();
                                ItemInfoAppear("��տ� �̹� ��� �ִ°� �־�!");
                            }
                        }
                        _input.getItem = false;
                    }
                }
            }
        }

        // �÷��̾� �������� Ȯ���ϰ� �������� �����ϴ� �Լ�
        private void CheckInventoryForItems(GameObject item)
        {
            if (hasString && hasString)
            {
                StartCoroutine(WaitAndCreateCompleteFishingRod("���ô�� �������� �����ϱ� ������ ���ô븦 ���� �� �ְڴ� ��?"));
                StartCoroutine(UIDisAppear());
            }

            if (item.name == "����")
            {
                hasAxe = true;
            }
            else if (item.name == "���")
            {
                hasPickAxe = true;
            }
            else if (item.name == "������")
            {
                hasString = true;
            }
            else if (item.name == "���ô�")
            {
                hasFishing = true;
            }
            else if (item.name == "������ ���ô�")
            {
                hasPerfactFishing = true;
            }
        }

        public void createprefabs(GameObject itemPrefabs, string name)
        {
            Vector3 spawnPosition = transform.position + transform.forward + new Vector3(0, 1, 0);
            completeFishingRodPrefab = (GameObject) Instantiate(itemPrefabs, spawnPosition, Quaternion.identity);
            completeFishingRodPrefab.name = name;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Item")
            {
                triggerItems.Remove(other.gameObject);
                ItemInfoDisappear();
                pickupActivated = true;
                _input.getItem = false;
            }
            if (other.gameObject.tag == "FishingZone")
            {
                FishingZone = false;
                ItemInfoDisappear();
            }
        }

        //�÷��̾� UI ���� �κ�
        void disappear()
        {
            ItemInfoDisappear();
            pickupActivated = false;
        }

        private async void HandInfoAppear(string Hand)
        {
            textImage.gameObject.SetActive(true);
            actionText.text = Hand;
            await Task.Delay(3000); // 3�� ���
            textImage.gameObject.SetActive(false);
        }

        private void ItemInfoAppear(string ItemName)
        {
            textImage.gameObject.SetActive(true);
            actionText.text = ItemName;
        }

        private void ItemInfoDisappear()
        {
            textImage.gameObject.SetActive(false);
            conversationImage.gameObject.SetActive(false);
        }

        private void conversationAppear(string conversation)
        {
            conversationImage.gameObject.SetActive(true);
            conversationText.text = conversation;
        }

        private IEnumerator DialogueUIAppear()
        {
            while (dialogueQueue.Count > 0)
            {
                string dialogue = dialogueQueue.Dequeue();

                conversationImage.gameObject.SetActive(true);
                conversationText.text = dialogue;

                yield return new WaitForSeconds(3);

                ItemInfoDisappear();
            }
        }

        private IEnumerator UIDisAppear()
        {
            yield return new WaitForSeconds(6);

            ItemInfoDisappear();
        }

        private IEnumerator WaitAndCreateCompleteFishingRod(string ItemName)
        {
            yield return new WaitForSeconds(4);
            
            conversationImage.gameObject.SetActive(true);
            conversationText.text = ItemName;

            //int fishingRodSlotIndex = inventory.FindItemSlotIndex(fishingRod);
            //int fishingLineSlotIndex = inventory.FindItemSlotIndex(fishingLine);
            //inventory.UsedItem(fishingRod, fishingRodSlotIndex);
            //inventory.UsedItem(fishingLine, fishingLineSlotIndex);

            //createprefabs();
        }
    }
}