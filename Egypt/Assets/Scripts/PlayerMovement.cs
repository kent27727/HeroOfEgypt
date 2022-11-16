using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header ("Variables")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float runSpeed;
	


	

	private Vector3 moveDir;
	private Vector3 velocity;

	[SerializeField] private bool isGrounded;
	[SerializeField] private float groundCheckDistance;
	[SerializeField] private LayerMask groundMask;
	[SerializeField] private float gravity;
	[SerializeField] private GameObject groundChechkObject;
	[SerializeField] private float jumpHeight;
	[Space(20f)]

	[Header("Attack")]
	public float cooldownTime = 2f;
	private float nextFireTime = 0f;
	public static int noOfAttacks = 0;
	float lastAttackTime;
	float maxComboDelay = 1;
	private float lastAttackAt = -9999f;
	public Transform attackPoint;
	public float attackRange = 0.5f;
	public LayerMask enemyLayers;
	public int attackDamage = 40;

	public int playerMaxHealth = 100;
	public int currentHealth;
	public bool isDeath;



	private CharacterController controller;
	private Animator animator;

	private void Start()
	{
		isDeath = false;
		currentHealth = playerMaxHealth;
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		Movement();
		#region Attack
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
		{
			animator.SetBool("hit1", false);
		}

		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
		{
			animator.SetBool("hit2", false);
		}

		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
		{
			animator.SetBool("hit3", false);
			noOfAttacks = 0;
		}

		if (Time.time - lastAttackTime > maxComboDelay)
		{
			noOfAttacks = 0;
		}

		if (Time.time > nextFireTime)
		{
			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				Attack();
			}
		}

		#endregion
	}
	private void Movement()
	{
		isGrounded = Physics.CheckSphere(groundChechkObject.transform.position, groundCheckDistance, groundMask);

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
		}

		float moveZ = Input.GetAxis("Vertical");
		moveDir = new Vector3(0, 0, moveZ);
		moveDir = transform.TransformDirection(moveDir);
		

		if (isGrounded)
		{
			if (moveDir != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
			{
				Walk();
			}
			else if (moveDir != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
			{
				Run();
			}
			else if (moveDir == Vector3.zero)
			{
				Idle();
			}

			moveDir *= moveSpeed;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Jump();
			}
		}
		controller.Move(moveDir * Time.deltaTime);

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	private void Idle()
	{
		animator.SetFloat("speed", 0, 0.1f, Time.deltaTime);
	}

	private void Walk()
	{
		moveSpeed = walkSpeed;
		animator.SetFloat("speed", 0.5f, 0.1f, Time.deltaTime);
	}

	private void Run()
	{
		moveSpeed = runSpeed;
		animator.SetFloat("speed", 1 , 0.1f , Time.deltaTime);
	}

	private void Jump()
	{
		velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
	}

	private void Attack()
	{
		
		lastAttackTime = Time.time;
		noOfAttacks++;
		if (noOfAttacks == 1)
		{
			
			animator.SetBool("hit1", true);
			
		}
		noOfAttacks = Mathf.Clamp(noOfAttacks, 0, 3);

		if (noOfAttacks >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
		{
			
			animator.SetBool("hit1", false);
			animator.SetBool("hit2", true);
			
		}

		if (noOfAttacks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
		{
			
			animator.SetBool("hit2", false);
			animator.SetBool("hit3", true);
			
		}

		#region Attack Overlap
		Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position , attackRange , enemyLayers);
		foreach (Collider enemy in hitEnemies)
		{
			if (Time.time >= lastAttackAt + cooldownTime)
			{
				enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
				lastAttackAt = Time.time;
			}
			
		}
		#endregion
	}

	public void TakePlayerDamage(int damage)
	{
		currentHealth -= damage;
		animator.SetTrigger("hurt");

		if (currentHealth <= 0)
		{
			PlayerDie();
		}
	}

	void PlayerDie()
	{
		animator.SetBool("death", true);
		isDeath = true;
		GetComponent<Collider>().enabled = false;
		this.enabled = false;
		
	}
	private void OnDrawGizmos()
	{
		if (attackPoint == null)
			return;
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}
