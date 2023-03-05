using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace QAAPlatformer
{
	/// <summary>
	/// Manages game flow and provides basic events
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		[SerializeField] protected GameObject m_RestartScreen = null;
		[SerializeField] protected Button m_RestartScreenRestartButton = null;
		[SerializeField] protected GameObject m_LevelCompleteScreen = null;
		[SerializeField] protected Button m_LevelCompleteScreenRestartButton = null;
		[SerializeField] protected Text m_TimerText = null;
		protected static bool m_IsRunning = true;
		protected static bool m_IsTimerRunning = false;

		public static bool IsRunning { get => m_IsRunning; protected set => m_IsRunning = value; }
		public static bool IsTimerRunning { get => m_IsTimerRunning; }
		public static float LevelTime { get; protected set; } = 0.0f;
		public static GameManager Instance { get; protected set; } = null;

		/// <summary>
		/// Enables or disables m_RestartScreen based on state
		/// </summary>
		public void ShowDeathScreen(bool show = true)
		{
			if (m_RestartScreen)
				m_RestartScreen.gameObject.SetActive(show);

			if (m_RestartScreenRestartButton)
				EventSystem.current.SetSelectedGameObject(m_RestartScreenRestartButton.gameObject);
		}

		/// <summary>
		/// Enabled or disables m_LevelComplete screen based on state,
		/// Updates m_TimerText with timer value
		/// </summary>
		public void ShowLevelCompleteScreen(bool show = true)
		{
			if (m_LevelCompleteScreen)
				m_LevelCompleteScreen.gameObject.SetActive(show);

			if (m_LevelCompleteScreenRestartButton)
				EventSystem.current.SetSelectedGameObject(m_LevelCompleteScreenRestartButton.gameObject);

			if (m_TimerText)
				m_TimerText.text = "Your time: " + LevelTime.ToString();
		}

		/// <summary>
		/// Start the game timer.
		/// </summary>
		public static void StartTimer()
		{
			m_IsTimerRunning = true;
		}

		/// <summary>
		/// Gets current level and restarts it
		/// </summary>
		public static void RestartCurrentLevel()
		{
			LevelTime = 0.0f;
			m_IsTimerRunning = false;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		/// <summary>
		/// Calls the static method, convenience method for UI Unity events
		/// </summary>
		public void RestartLevel()
		{
			GameManager.RestartCurrentLevel();
		}

		/// <summary>
		/// Called when player dies.
		/// </summary>
		/// <param name="playerControlledObject">The gameobject the player was controlling.</param>
		public static void OnPlayerDeath(GameObject playerControlledObject)
		{
			Debug.Log("Game Over: Player (" + playerControlledObject.name + ") has died.");
			if (Instance)
				Instance.ShowDeathScreen();
		}

		/// <summary>
		/// Called when level is completed.
		/// </summary>
		public static void OnLevelComplete(GameObject triggeringGameObject)
		{
			Debug.Log("Level complete.");
			if (Instance)
				Instance.ShowLevelCompleteScreen();

			triggeringGameObject.SetActive(false);
			IsRunning = false;
			m_IsTimerRunning = false;
		}

		/// <summary>
		/// Initializes singleton for this GameManager
		/// </summary>
		private void Awake()
		{
			if (Instance != this)
				Destroy(Instance);
			Instance = this;
		}

		/// <summary>
		/// Initializes game
		/// </summary>
		private void Start()
		{
			IsRunning = true;
			Debug.Log("Game started.");
		}

		/// <summary>
		/// Updates timer
		/// </summary>
		private void Update()
		{
			if (m_IsTimerRunning)
				LevelTime += Time.deltaTime;
		}
	}
}