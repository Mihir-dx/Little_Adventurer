using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private CharacterController C;
    public float speed = 7f;
    public GameObject speedReduced;
    public int Coin;
    private Vector3 movementVelocity;
    private PlayerInput playerInput;
    private float verticalVelocity;
    public float gravity = -9.81f;
    private Animator _animator;
    private float attackStartTime;
    public float attackSlideDuration = 0.4f; // Duration of the attack animation
    public float attackSlideSpeed = 0.06f;
    private Vector3 attackSlideDirection;
    private DamageCaster _damageCaster; // Reference to the DamageCaster script
    private Health _health; // Reference to the Health script
    public GameObject ItemToDrop;
    private Vector3 ImpactOnCharacter;
    public bool isInvincible;
    public float invincibleDuration = 1.5f;
    private float attackAnimationDuration;
    public float SlideSpeed = 9f;
    public float SpawnDuration = 2f;
    private float currentSpawnTime;
    public AudioSource P_Attack;
    public AudioSource E_Attack;
    public AudioSource P_BeingHit;
    public AudioSource E_BeingHit;
    public AudioSource CoinSFX;
    public AudioSource HealSFX;
    public AudioSource Spawn;
    public AudioSource PoisonHit;
    public AudioSource DeathSFX;
    public ParticleSystem Poison;
    private int poisonMinDamage = 3;
    private int poisonMaxDamage = 7;
    private float poisonTickInterval = 1f;
    private Coroutine _poisonRoutine;

    // Enemy
    public bool isPlayer = true;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform TargetPlayer;

    //Material Animation
    private MaterialPropertyBlock PropBlock;
    private SkinnedMeshRenderer _skinMeshRenderer;

    // State Machine
    public CharacterState CurrentState;
    public enum CharacterState
    {
        Normal,
        Attacking,
        Dead,
        BeingHit,
        Slide,
        Spawn
    }

    private void Awake()
    {
        C = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        PropBlock = new MaterialPropertyBlock();
        _skinMeshRenderer.GetPropertyBlock(PropBlock);

        if (!isPlayer)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                TargetPlayer = playerObj.transform;
                agent.speed = speed;
            }
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayer) return;
        if (other.CompareTag("Slow"))
        {
            speed -= 2f;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Poison"))
        {
            if (_poisonRoutine == null)
                _poisonRoutine = StartCoroutine(DoPoisonDamage());
                speed -= 4f;
            if (PoisonHit != null)
                PoisonHit.Play();

            // Start continuous Camera Shake while inside the poison
            var cam = Camera.main ? Camera.main.GetComponent<Camere>() : null;
            if (cam != null)
                cam.StartContinuousShake(); //Value can be adjusted.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayer) return;

        if (other.CompareTag("Poison"))
        {
            if (_poisonRoutine != null)
            {
                StopCoroutine(_poisonRoutine);
                _poisonRoutine = null;
                speed += 4f;
                PoisonHit.Stop();
            }

            // Stop continuous Camera Shake when exiting the poison
            var cam = Camera.main ? Camera.main.GetComponent<Camere>() : null;
            if (cam != null)
                cam.StopShake();
        }
    }

    private IEnumerator DoPoisonDamage()
    {
        while (true)
        {
            if (_health != null)
            {
                int dmg = Random.Range(poisonMinDamage, poisonMaxDamage + 1);
                ApplyDamage(dmg, transform.position);
            }
            yield return new WaitForSeconds(poisonTickInterval);
        }
    }

    private void CalcPMovement()
    {
        if (playerInput.MouseButtonDown && C.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        else if (playerInput.SpaceKeyDown && C.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        movementVelocity.Set(playerInput.HorizontalInput, 0f, playerInput.VerticalInput);
        movementVelocity.Normalize();
        movementVelocity = Quaternion.Euler(0f, -45f, 0f) * movementVelocity;
        _animator.SetFloat("Speed", movementVelocity.magnitude);

        movementVelocity *= speed * Time.deltaTime;

        if (movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(movementVelocity);

        _animator.SetBool("AirBorne", !C.isGrounded);
        gravity = C.isGrounded ? -9.81f : -21f;
    }

    private void CalcEMovement()
    {
        if (TargetPlayer == null) return;

        if (Vector3.Distance(TargetPlayer.position, transform.position) >= agent.stoppingDistance)
        {
            agent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {
            agent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                    CalcPMovement();
                else
                    CalcEMovement();
                break;

            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.time < attackStartTime + attackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / attackSlideDuration;
                        movementVelocity = Vector3.Lerp(attackSlideDirection * attackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if (playerInput.MouseButtonDown && C.isGrounded)
                    {
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f)
                        {
                            playerInput.MouseButtonDown = false;
                            _animator.ResetTrigger("Attack");
                            _animator.SetTrigger("Attack");
                            PlayAttackSFX();

                            // Change only to make player attack constantly.
                            //playerInput.MouseButtonDown = false;
                            //SwitchStateTo(CharacterState.Attacking);

                            attackStartTime = Time.time;
                            attackSlideDirection = transform.forward;
                            //SwitchStateTo(CharacterState.Attacking);
                        }
                    }
                }
                break;

            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:
                break;

            case CharacterState.Slide:
                movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if (currentSpawnTime <= 0f)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if (ImpactOnCharacter.magnitude > 0.2f)
        {
            movementVelocity = ImpactOnCharacter * Time.deltaTime;
        }
        ImpactOnCharacter = Vector3.Lerp(ImpactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (isPlayer)
        {
            if (C != null)
            {
                if (C.isGrounded)
                    verticalVelocity = gravity;
                else
                    verticalVelocity = gravity * 0.3f;

                movementVelocity += verticalVelocity * Vector3.up * Time.deltaTime;
                C.Move(movementVelocity);
            }
            else
            {
                transform.position += movementVelocity;
            }

            movementVelocity = Vector3.zero;
        }
        else
        {
            // For enemies, only apply manual movement (knockback) when not in Normal
            if (CurrentState != CharacterState.Normal)
            {
                bool moved = false;

                // Prefer NavMeshAgent.Move for enemies
                if (agent != null && agent.enabled)
                {
                    agent.Move(movementVelocity);
                    moved = true;
                }

                if (!moved && C != null)
                {
                    C.Move(movementVelocity);
                    moved = true;
                }

                if (!moved)
                {
                    transform.position += movementVelocity;
                }

                movementVelocity = Vector3.zero;
            }
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            playerInput.ClearCache();
        }

        // Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (_damageCaster != null)
                    _damageCaster.DisableDamageCaster();

                if (isPlayer)
                    GetComponent<PlayerVFX>().StopBlade();

                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                if (!isPlayer && agent != null) agent.isStopped = false;
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                isInvincible = false;
                break;
        }

        // Entering state
        switch (newState)
        {
            case CharacterState.Normal:
                break;

            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    if (TargetPlayer != null)
                    {
                        Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                        transform.rotation = newRotation;
                    }
                }

                _animator.ResetTrigger("Attack");
                _animator.SetTrigger("Attack");
                //Debug.Log(_animator.GetBool("Attack"));

                if (isPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();
                    PlayAttackSFX();
                    attackSlideDirection = transform.forward;
                }
                break;

            case CharacterState.Dead:
                if(C != null)
                    C.enabled = false;
                _animator.SetTrigger("Dead");
                if (DeathSFX != null)
                    DeathSFX.Play();

                if (!isPlayer)
                {
                    var enemyVFX = GetComponent<EnemyVFXManager>();
                    if (enemyVFX != null)
                        enemyVFX.OnDeadCleanup();
                }

                StartCoroutine(MaterialDissolve());
                break;

            case CharacterState.BeingHit:

                _animator.SetTrigger("BeingHit");
                PlayHitSFX();
                if (!isPlayer && agent != null) agent.isStopped = true;
                break;

            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;

            case CharacterState.Spawn:
                isInvincible = true;    //Player doesn't hit the enemy during spawn
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }

        CurrentState = newState;
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos)
    {
        if (!isInvincible && _health != null)
        {
            _health.ApplyDamage(damage);

            if (isPlayer)
            {
                isInvincible = true;
                StartCoroutine(DelayCancelInvincible());
            }
        }

        if (isPlayer)
        {
            // Camera shake on player hit
            var cam = Camera.main ? Camera.main.GetComponent<Camere>() : null;
            if (cam != null) cam.Shake(0.15f, 0.2f);

            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);
            StartCoroutine(RecoverFromHit());
        }
        else
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 2.5f);
            StartCoroutine(RecoverFromHit());
        }

        if (!isPlayer)
        {
            var vfxManager = GetComponent<EnemyVFXManager>();
            if (vfxManager != null)
                vfxManager.PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MaterialBlink());
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        ImpactOnCharacter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        if (_damageCaster != null)
            _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        if (_damageCaster != null)
            _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        PropBlock.SetFloat("_blink", 0.4f);
        _skinMeshRenderer.SetPropertyBlock(PropBlock);

        yield return new WaitForSeconds(0.2f);

        PropBlock.SetFloat("_blink", 0f);
        _skinMeshRenderer.SetPropertyBlock(PropBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0f;
        float dissolveHight_start = 20f;
        float dissolveHight_target = -25f;
        float dissolveHight;

        PropBlock.SetFloat("_enableDissolve", 1f);
        _skinMeshRenderer.SetPropertyBlock(PropBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            PropBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinMeshRenderer.SetPropertyBlock(PropBlock);
            yield return null;
        }

        DropItem();
    }

    public void DropItem()
    {
        if(ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.Value);
                break;

            case PickUp.PickUpType.Coin:
                PlayCoinSFX();
                AddCoin(item.Value);
                break;
        }
    }

    private void AddHealth(int health)
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFX>().PlayHealVFX();
    }

    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    private IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.2f);
        if (CurrentState == CharacterState.BeingHit)
            SwitchStateTo(CharacterState.Normal);
    }

    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = SpawnDuration;
        float currentDissolveTime = 0f;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissolveHight;

        if (Spawn != null)
            Spawn.Play();

        PropBlock.SetFloat("_enableDissolve", 1f);
        _skinMeshRenderer.SetPropertyBlock(PropBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            PropBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinMeshRenderer.SetPropertyBlock(PropBlock);
            yield return null;
        }

        PropBlock.SetFloat("_enableDissolve", 0f);
        _skinMeshRenderer.SetPropertyBlock(PropBlock);
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1f);
        }
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }

    private void PlayAttackSFX()
    {
        if (P_Attack != null)
            P_Attack.Play();
    }

    private void E_AttackSFX()
    {
        if (E_Attack != null)
            E_Attack.Play();
    }

    private void DisableE_AttackSFX()
    {
        if (E_Attack.isPlaying)
            E_Attack.Stop();
    }

    private void PlayHitSFX()
    {
        if(isPlayer)
        {
            if (P_BeingHit != null)
                P_BeingHit.Play();
        }
        else
        {
            if (E_BeingHit != null)
                E_BeingHit.Play();
        }
    }

    public void PlayCoinSFX()
    {
        if (CoinSFX != null)
            CoinSFX.Play();
    }

    public void PlayHealSFX()
    {
        if (HealSFX != null)
            HealSFX.Play();
    }
}