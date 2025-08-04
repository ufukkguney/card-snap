using VContainer;
using VContainer.Unity;

internal class GameInitializer : IStartable
{
    [Inject] private DeckController deckController;

    public void Start()
    {
        deckController.Initialize();
    }
}
