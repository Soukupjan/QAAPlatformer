using QAAPlatformer.Damage;
using System.Collections.Generic;
using UnityEngine;

namespace QAAPlatformer.Obstacles
{
	/// <summary>
	/// Obstacle that will damage entering objects that have a damage manager
	/// </summary>
	public class Spike : MonoBehaviour
	{
		[SerializeField] protected int m_OnContactDamage = 20;
		[SerializeField] protected float m_OnDamageImpulse = 20.0f;
		[SerializeField] protected float m_DamageInterval = 1f;

		/// <summary>
		/// Convenience class for holding damageable objects data
		/// </summary>
		protected class SpikeDamageable
		{
			public Rigidbody2D m_RigidBody;
			public CharacterDamageManager m_DamageManager;
			public float m_LastBouncedTime;

			public SpikeDamageable(Rigidbody2D rigidBody, CharacterDamageManager damageManager, float lastBouncedTime = 0.0f)
			{
				this.m_RigidBody = rigidBody;
				this.m_DamageManager = damageManager;
				this.m_LastBouncedTime = lastBouncedTime;
			}
		}
		protected Dictionary<Collider2D, SpikeDamageable> m_CollidingDamageables = new Dictionary<Collider2D, SpikeDamageable>();

		/// <summary>
		/// When an object collides with this spike, it's added to the list of damageables
		/// </summary>
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!m_CollidingDamageables.ContainsKey(collision))
			{
				m_CollidingDamageables.Add(collision, new SpikeDamageable(collision.attachedRigidbody, collision.GetComponent<CharacterDamageManager>()));
			}
		}

		/// <summary>
		/// When an object collides with this spike, it's removed from the list of damageable
		/// </summary>
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (m_CollidingDamageables.ContainsKey(collision))
			{
				m_CollidingDamageables.Remove(collision);
			}
		}

		/// <summary>
		/// For every damageable object in the list add damage and some physical impulse, pushing the object away 
		/// </summary>
		private void FixedUpdate()
		{
			foreach (KeyValuePair<Collider2D, SpikeDamageable> keyPair in m_CollidingDamageables)
			{
				SpikeDamageable damageable = keyPair.Value;
				if (damageable.m_LastBouncedTime + m_DamageInterval > Time.time)
					continue;

				Rigidbody2D rigidBody = damageable.m_RigidBody;
				CharacterDamageManager damageManager = damageable.m_DamageManager;
				damageable.m_LastBouncedTime = Time.time;

				if (rigidBody)
				{
					rigidBody.AddForce(Vector2.up * m_OnDamageImpulse * rigidBody.mass, ForceMode2D.Impulse);
				}
				if (damageManager)
				{
					damageManager.ReceiveDamage(m_OnContactDamage);
				}
			}
		}
	}
}