using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Sprite greyImage;

    public Sprite imageOG;

    private void Start()
    {
        if (greyImage == null)
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

        // Oculta los elementos sueltos (química), el estante y el Perol para
        // que no se sobrepongan con el panel que se acaba de abrir (Lista, Pedido, etc.).
        DraggableElement.SetAllVisible(false);
        ElementSpawner.SetAllVisible(false);
        Cauldron.SetAllVisible(false);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);

        // Vuelve a mostrarlos apenas se cierra el panel, salvo que el juego
        // ya se haya terminado (no tendría sentido que reaparezcan).
        if (!ReactionLogger.IsGameOver)
        {
            DraggableElement.SetAllVisible(true);
            ElementSpawner.SetAllVisible(true);
            Cauldron.SetAllVisible(true);
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Cerrando juego...");
    }

    public void OpenGreyScreen(GameObject panel)
    {

        if (greyImage != null)
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

    public void OpenOrderPanel(GameObject panel)
    {
        panel.SetActive(true);

        // Oculta los elementos sueltos (química), el estante y el Perol para
        // que no se sobrepongan con el panel que se acaba de abrir (Lista, Pedido, etc.).
        DraggableElement.SetAllVisible(false);
        ElementSpawner.SetAllVisible(false);
        Cauldron.SetAllVisible(false);
    }

    public void CloseOrderPanel(GameObject panel)
    {
        panel.SetActive(false);

        // Vuelve a mostrarlos apenas se cierra el panel, salvo que el juego
        // ya se haya terminado (no tendría sentido que reaparezcan).
        if (!ReactionLogger.IsGameOver)
        {
            DraggableElement.SetAllVisible(true);
            ElementSpawner.SetAllVisible(true);
            Cauldron.SetAllVisible(true);
        }
    }
}
