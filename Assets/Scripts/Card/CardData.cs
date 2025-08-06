using UnityEngine;

[System.Serializable]
public struct CardData
{
    [SerializeField] private CardType cardType;
    [SerializeField] private int attack;
    [SerializeField] private int defense;

    public CardData(CardType type, int attackValue, int defenseValue)
    {
        cardType = type;
        attack = Mathf.Max(0, attackValue);
        defense = Mathf.Max(0, defenseValue);
    }

     public CardType CardType
    {
        get { return cardType; }
        set { cardType = value; }
    }
    public int Attack 
    { 
        get => attack; 
        set => attack = Mathf.Max(0, value); 
    }
    public int Defense 
    { 
        get => defense; 
        set => defense = Mathf.Max(0, value); 
    }
}

public enum CardType
{
    Messi,
    Ronaldo,
    Neymar,
    Mbappe,
    Hazard,
    Salah,
    Modric,
    Kane,
    DeBruyne,
    ArdaTuran
}