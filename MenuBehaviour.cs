using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf) menu.SetActive(false);
            else menu.SetActive(true);
        }
    }
}