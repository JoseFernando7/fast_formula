using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Sprite greyImage;

    public Sprite imageOG;

    private void Start()
    {
        if(greyImage == null)
        {
            greyImage = null;
        }

        if (imageOG == null)
        {
            imageOG = null;
        }
    }

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

    public void OpenGreyScreen(GameObject panel)
    {
       
        if(greyImage != null)
        {
            Image image = panel.GetComponent<Image>();
            image.sprite = greyImage;
        }
    }

    public void CloseGreyScreen(GameObject panel)
    {
        if (greyImage != null)
        {
            Image image = panel.GetComponent<Image>();
            image.sprite = imageOG;
        }

    }
}
