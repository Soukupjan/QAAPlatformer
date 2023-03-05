using QAAPlatformer.Damage;
using UnityEngine;

namespace QAAPlatformer.Character
{
	/// <summary>
	/// Handles 2D character movement in 2D environment
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public class CharacterController2D : MonoBehaviour
	{
		[Header("Character Movement")]
		[SerializeField]
		protected float m_MovementSpeed = 6.4f;
		[SerializeField]
		protected float m_JumpVelocity = 6.2f;

		[Header("Character Physics")]
		[SerializeField]
		protected float m_GroundingRayDistance = 0.1f;
		[SerializeField]
		protected float m_MaxSpeed = 5.0f;
		protected bool m_IsGrounded;
		protected bool m_IsJumping;
		protected bool m_JumpQueued;
		protected Vector2 m_DesiredInput;
		protected Vector3 m_Extents;
		protected Rigidbody2D m_Rigidbody;
		protected CharacterDamageManager m_DamageManager;
		/// <summary>
		/// Does up to three rays from the character downwards, checking for collision.
		/// If any object is hit, true is returned.
		/// </summary>
		bool GetGroundingRayResult(Vector3 position, float downExtents, float sideExtents, float rayDistance)
		{
			// The origin at bottom centre
			Vector3 bottomOrigin = new Vector3(position.x, position.y - downExtents, 0.0f);

			RaycastHit2D groundRayCenter = Physics2D.Raycast(bottomOrigin, -Vector2.up, rayDistance);
			if (groundRayCenter)
				return true;

			float tqExtents = 0.75f * sideExtents;

			Vector3 leftOrigin = new Vector3(bottomOrigin.x - tqExtents, bottomOrigin.y, 0.0f);
			RaycastHit2D groundRayLeft = Physics2D.Raycast(leftOrigin, -Vector2.up, rayDistance);
			if (groundRayLeft)
				return true;

			Vector3 rightOrigin = new Vector3(bottomOrigin.x + tqExtents, bottomOrigin.y, 0.0f);
			RaycastHit2D groundRayRight = Physics2D.Raycast(rightOrigin, -Vector2.up, rayDistance);
			if (groundRayRight)
				return true;

			return false;
		}

		/// <summary>
		/// Initializes components and internal variables
		/// </summary>
		void Start()
		{
			m_Rigidbody = GetComponent<Rigidbody2D>();
			m_DamageManager = GetComponent<CharacterDamageManager>();
			Collider2D collider = GetComponent<Collider2D>();
			m_Extents = collider.bounds.extents;
		}

		/// <summary>
		/// Processes inputs to add velocity to physical rigidbody of the player, only if game is running
		/// </summary>
		void FixedUpdate()
		{
			if (!GameManager.IsRunning)
				return;

			if (m_DamageManager && m_DamageManager.IsDead())
			{
				if (m_Rigidbody.freezeRotation == true)
					m_Rigidbody.freezeRotation = false;
				return;
			}


			bool wasGrounded = m_IsGrounded;
			// Check if character is grounded
			m_IsGrounded = GetGroundingRayResult(transform.position, m_Extents.y, m_Extents.x, m_GroundingRayDistance);

			if (wasGrounded && m_IsGrounded)
			{
				m_IsJumping = false;
			}

			// If jump is possible, jump
			if (m_JumpQueued && m_IsGrounded && !m_IsJumping)
			{
				m_JumpQueued = false;
				m_IsGrounded = false;
				m_IsJumping = true;

				// m_Rigidbody.AddForce(Vector2.up * m_JumpVelocity * m_Rigidbody.mass, ForceMode2D.Impulse);
				m_Rigidbody.velocity = CalculateJump(m_DesiredInput.x, Time.fixedDeltaTime);
                //m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y + m_JumpVelocity * m_Rigidbody.mass * Time.fixedDeltaTime); //origo jump
            }

			// Move
			// .velocity = new Vector2(m_DesiredInput.x * m_MovementSpeed * m_Rigidbody.mass * Time.fixedDeltaTime, m_Rigidbody.velocity.y);
			if (Mathf.Abs(m_Rigidbody.velocity.x) < m_MaxSpeed)
               

            // m_Rigidbody.AddForce(new Vector2(m_DesiredInput.x * m_MovementSpeed * m_Rigidbody.mass * Time.fixedDeltaTime, 0.0f), ForceMode2D.Impulse); //origo
            m_Rigidbody.AddForce(CalculateMovement(m_DesiredInput.x, Time.fixedDeltaTime), ForceMode2D.Impulse);

        }

		/// <summary>
		/// Handles input from the keyboard
		/// </summary>

		public Vector2 CalculateJump(float x, float time)


		{
			return new Vector2(x, m_Rigidbody.velocity.y + m_JumpVelocity * m_Rigidbody.mass * time);
		}
		public Vector2 CalculateMovement(float x, float time)


		{
			return new Vector2(x * m_MovementSpeed * m_Rigidbody.mass * time, 0.0f);
		}
	
        void Update()
		{
			if (m_DamageManager && m_DamageManager.IsDead())
				return;

			m_DesiredInput.x = Input.GetAxis("Horizontal");
			m_JumpQueued = Input.GetButton("Jump") || Input.GetAxis("Vertical") > 0;

			// If the timer is not started, start it on first input
			if (!GameManager.IsTimerRunning)
			{
				if (Mathf.Abs(m_DesiredInput.x) > float.Epsilon || m_JumpQueued)
					GameManager.StartTimer();
			}
		}
	}
}