using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDeckController
{
    void Initialize();
    void ToggleCardSelection(CardData cardData);
    void ClearAllSelections();
    void ResetDeckState();
    void ReturnToDeckSelection();
    bool IsCardSelected(CardData cardData);
    int GetSelectedCount();
    List<CardUIView> GetInstantiatedCardViews();
}