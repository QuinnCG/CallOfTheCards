using Sirenix.OdinInspector;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace Quinn
{
	[ExecuteAlways]
	public class CardUtility : MonoBehaviour
	{
		[SerializeField, Required]
		private TextMeshProUGUI DP, HP, Cost;

		private Card _card;

#if UNITY_EDITOR
		private void Awake()
		{
			_card = GetComponent<Card>();
		}

		private void Update()
		{
			if (PrefabStageUtility.GetCurrentPrefabStage() == null)
			{
				return;
			}

            if (_card == null)
			{
				Awake();
			}

			DP.text = _card.BaseDP.ToString();
			HP.text = _card.BaseHP.ToString();
			Cost.text = _card.Cost.ToString();
		}
#endif
	}
}
