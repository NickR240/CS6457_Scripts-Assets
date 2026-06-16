using UnityEngine;
using System.Collections;

public class TentacleAttack : MonoBehaviour
{
    // player refs
    [Header("References")]
    public Transform player;
    public EmberControl emberControl;

    // follow settings
    [Header("Follow")]
    public bool followPlayer = true;
    public Vector3 followOffset = new Vector3(0f, 4f, -2f);
    public float followSpeed = 6f;

    // check range and ember
    [Header("Detection")]
    public float detectDistance = 10f;
    public bool requireEmberOut = true;

    // attack timing
    [Header("Attack Timing")]
    public float attackCooldown = 2f;
    public float swingDownTime = 0.2f;
    public float returnTime = 0.35f;
    public float holdAtBottomTime = 0.1f;

    // idle and hit angle
    [Header("Angles")]
    public Vector3 idleRotationEuler = new Vector3(0f, 0f, 0f);
    public Vector3 attackRotationEuler = new Vector3(0f, 0f, 90f);

    // damage values
    [Header("Damage")]
    public int damage = 10;
    public float hitDistance = 2.5f;

    // saved rotations
    private Quaternion idleRotation;
    private Quaternion attackRotation;

    // attack state
    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    // room check
    [Header("Room Activation")]
    public bool requirePlayerInRoom = true;
    public bool playerInActiveRoom = false;

    void Start()
    {
        // set start and hit rot
        idleRotation = Quaternion.Euler(idleRotationEuler);
        attackRotation = Quaternion.Euler(attackRotationEuler);

        // start in idle rot
        transform.localRotation = idleRotation;

        // find player if empty
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // get ember script
        if (emberControl == null && player != null)
        {
            emberControl = player.GetComponent<EmberControl>();
        }
    }

    void Update()
    {
        // stop if no player
        if (player == null)
            return;

        // stop if room not active
        if (requirePlayerInRoom && !playerInActiveRoom)
            return;

        // follow player pos
        if (followPlayer && !isAttacking)
        {
            Vector3 targetPosition = player.position + followOffset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * followSpeed
            );
        }

        // get player range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // stop if too far
        if (distanceToPlayer > detectDistance)
            return;

        // need ember out
        if (requireEmberOut)
        {
            if (emberControl == null || !emberControl.IsEmberHeld())
            {
                // go back idle rot
                transform.localRotation = idleRotation;
                return;
            }
        }

        // start attack on timer
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        // start attack state
        isAttacking = true;
        lastAttackTime = Time.time;

        // prep swing vars
        float elapsed = 0f;
        Quaternion startRotation = transform.localRotation;

        // swing down
        while (elapsed < swingDownTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDownTime;
            transform.localRotation = Quaternion.Slerp(startRotation, attackRotation, t);
            yield return null;
        }

        // lock hit rot
        transform.localRotation = attackRotation;

        // try hit player
        TryDamagePlayer();

        // wait at bottom
        yield return new WaitForSeconds(holdAtBottomTime);

        // reset swing vars
        elapsed = 0f;
        startRotation = transform.localRotation;

        // go back up
        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnTime;
            transform.localRotation = Quaternion.Slerp(startRotation, idleRotation, t);
            yield return null;
        }

        // back to idle
        transform.localRotation = idleRotation;
        isAttacking = false;
    }

    void TryDamagePlayer()
    {
        // stop if no player
        if (player == null)
            return;

        // check hit range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // do damage if close
        if (distanceToPlayer <= hitDistance)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // test log
            Debug.Log("Tentacle hit player.");
        }
    }
}
