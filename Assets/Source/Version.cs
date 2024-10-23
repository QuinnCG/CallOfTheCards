using TMPro;
using UnityEngine;

namespace Quinn
{
    public class Version : MonoBehaviour
    {
        void Start()
        {
			GetComponent<TextMeshProUGUI>().text = Application.version;
        }
    }
}
