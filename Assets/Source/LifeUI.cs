using TMPro;
using UnityEngine;

namespace Quinn
{
    public class LifeUI : MonoBehaviour
    {
		[SerializeField]
		private TextMeshProUGUI Text;

		public int Value { set => Text.text = $"{value} Life"; }
    }
}
