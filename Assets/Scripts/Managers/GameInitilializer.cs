using VContainer;
using VContainer.Unity;

internal class GameInitializer : IStartable
{
    [Inject] private IDeckController deckController;

    public void Start()
    {
        deckController.Initialize();
    }
}
