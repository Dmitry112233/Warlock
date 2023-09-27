using TMPro;
using UnityEngine;

public class MainMenuUIHandler : MonoBehaviour
{
    [Header("Panels")]
    public GameObject sessionsPanel;
    public GameObject createSessionPanel;

    [Header("New game session")]
    public TMP_InputField sessionNameInputField;

    private void Start()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        if (networkRunnerHandler != null)
        {
            networkRunnerHandler.OnJoinLobby();
        }
    }

    private void HideAllPanels() 
   {
        sessionsPanel.SetActive(false);
        createSessionPanel.SetActive(false);
   }

    public void OnCreateNewSessionClicked() 
    {
        HideAllPanels();
        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionNameInputField.text, GameData.Scenes.Battle);

        HideAllPanels();
    }

    public void OnJoinLobbyClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();
    }
}
