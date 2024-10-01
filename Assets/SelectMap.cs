using UnityEngine;
using Services;
using UnityEngine.UI;

public class SelectMap : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown dropdown;
    private void OnEnable()
    {
     /*   GameManager gm = ServiceLocator.Get<GameManager>();
        dropdown = GetComponent<TMPro.TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(
            delegate
            {
                gm.SetMapID(dropdown.value);
            });*/
    }
}
