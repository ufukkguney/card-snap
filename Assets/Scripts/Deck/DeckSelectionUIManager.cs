public class DeckSelectionUIManager
{
    private DeckConfiguration config;

    public void Initialize(DeckConfiguration config)
    {
        this.config = config;
    }

    public void UpdateStartButton(DeckSelectionManager selectionManager)
    {
        if (config.StartButton == null) return;

        bool isMaxReached = selectionManager.IsMaxReached;
        config.StartButton.gameObject.SetActive(isMaxReached);
        config.StartButton.interactable = isMaxReached;

    }

    public void InitializeUI(DeckSelectionManager selectionManager)
    {
        UpdateStartButton(selectionManager);
    }
}
