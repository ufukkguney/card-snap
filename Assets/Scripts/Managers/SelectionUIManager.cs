using UnityEngine;

public class SelectionUIManager
{
    private readonly DeckConfiguration config;

    public SelectionUIManager(DeckConfiguration config)
    {
        this.config = config;
    }

    public void UpdateStartButton(SelectionManager selectionManager)
    {
        if (config.StartButton == null) return;

        bool isMaxReached = selectionManager.IsMaxReached;
        config.StartButton.gameObject.SetActive(isMaxReached);
        config.StartButton.interactable = isMaxReached;
        
        Debug.Log($"Start button: {(isMaxReached ? "ACTIVE" : "INACTIVE")} - Selected: {selectionManager.GetSelectedCount()}/{config.maxSelectedCards}");
    }

    public void InitializeUI(SelectionManager selectionManager)
    {
        UpdateStartButton(selectionManager);
    }
}
