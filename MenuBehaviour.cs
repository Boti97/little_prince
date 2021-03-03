using UnityEngine;
using Bolt;
using UnityEngine.SceneManagement;

public class MenuBehaviour : GlobalEventListener
{
    [SerializeField]
    private GameObject menu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf) menu.SetActive(false);
            else menu.SetActive(true);
            //Cursor.visible = true;
        }
    }

    public void OnExitClick()
    {
        SceneManager.LoadScene("Start");
        BoltLauncher.Shutdown();
    }

    public void OnResumeClick()
    {
        menu.SetActive(false);
    }
}