using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList() 
    {
        foreach(Transform child in verticalLayoutGroup.transform) 
        {
            Destroy(child.gameObject);
        }

        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo) 
    {
        SessionInfoListUIItem addedSessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();

        addedSessionInfoListUIItem.SetInformation(sessionInfo);

        addedSessionInfoListUIItem.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo sessionInfo) 
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.JoinGame(sessionInfo);
    }

    public void OnNoSessionFound() 
    {
        ClearList();

        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }
}
