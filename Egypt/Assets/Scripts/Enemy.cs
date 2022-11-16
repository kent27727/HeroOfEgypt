using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform player;
    [SerializeField] private float detectRange;
    [SerializeField] private float followRange;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float backRange;
    [SerializeField] private float forceSpeed;
    [SerializeField] private bool playerIsDeath;

    public float attackCoolDown = 2f;
    private float lastAttackTime;

    [SerializeField] private bool isDetect;
    [SerializeField] private bool isFollow;
    [SerializeField] private bool isAttack;

    private Animator animator;

    public int maxHealth = 100;
    public int currentHealth;

    public Transform playerAttackPoint;
    public float playerAttackRange = 0.5f;
    public LayerMask playerLayer;
    public int attackDamage = 40;
    

    void Start()
    {
        
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        EnemyDistance();
        EnemyFollow();  
    }

    private void EnemyDistance()
	{
        float dist = Vector3.Distance(player.position, transform.position);
        float startDist = Vector3.Distance(startPoint.position, transform.position);

		if (dist > detectRange)
		{
			if (startPoint.transform.position != this.transform.position)
			{
                isDetect = false;
                transform.position = Vector3.MoveTowards(transform.position, startPoint.position, moveSpeed * Time.deltaTime);
                EnemyRun();
                transform.LookAt(startPoint);
            }
           
			if (startPoint.transform.position == this.transform.position)
			{
                EnemyIdle();
			}
        }

		

		
		else if (dist <= detectRange && dist > followRange && dist > attackRange)
		{
            isDetect = true;
            isFollow = false;
            isAttack = false;
            
        }
		else if (dist < followRange && dist > attackRange)
		{
            isAttack = false;
            isDetect = false;
            isFollow = true;
           
        }
		else if (dist < attackRange)
		{
            isDetect = false;
            isFollow = false;
            isAttack = true;
            
        } 
	}

    private void EnemyFollow()
	{
		if (isDetect)
		{
            transform.LookAt(player);
            EnemyIdle();
            animator.SetBool("attack", false);
        }
		else if (isFollow)
		{
            transform.LookAt(player);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            EnemyRun();
            animator.SetBool("attack", false);
        }
		else if (isAttack)
		{
            transform.LookAt(player);
			if (Time.time > lastAttackTime + attackCoolDown)
			{
                EnemyAttack();
                lastAttackTime = Time.time;
            }
        }
		
	}

    private void EnemyIdle()
	{
        animator.SetFloat("speed", 0, 0.1f, Time.deltaTime);
    }

    private void EnemyRun()
	{
        animator.SetFloat("speed", 1, 0.1f, Time.deltaTime);
    }

    private void EnemyAttack()
	{
        animator.SetBool("attack" , true);
        #region Attack Overlap
        Collider[] hitEnemies = Physics.OverlapSphere(playerAttackPoint.position, playerAttackRange, playerLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<PlayerMovement>().TakePlayerDamage(attackDamage);
        }
        #endregion
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GetComponent<Rigidbody>().AddForce(-transform.forward * forceSpeed, ForceMode.Impulse);
        animator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("death", true);
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
        
    }

    private void OnDrawGizmos()
    {
        if (playerAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(playerAttackPoint.position, playerAttackRange);
    }





}
