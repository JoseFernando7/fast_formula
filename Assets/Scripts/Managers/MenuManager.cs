using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Cerrando juego...");
    }

    public void OpenGreyScreen(GameObject panel,  GameObject imageOG, Image greenImage)
    {
       Image image = imageOG.GetComponent<Image>();

       image = greenImage;

        panel.gameObject.SetActive(true);
    
    }

    public void CloseGreyScreen(GameObject panel, GameObject imageOG, Image imageToChange)
    {
        Image image = imageOG.GetComponent<Image>();

        image = imageToChange;

        panel.gameObject.SetActive(false);

    }
}
