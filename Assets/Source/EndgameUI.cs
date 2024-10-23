using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn
{
	public class EndgameUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI Title;
		[SerializeField]
		private string VictoryTitle, DefeatTitle;

		private void Awake()
		{
			Title.text = Player.HasHumanWon ? VictoryTitle : DefeatTitle;
		}

		public async void Retry_Button()
		{
			await SceneManager.LoadSceneAsync("GameScene");
		}

		public void Quit_Button()
		{
			Application.Quit();
		}
	}
}
