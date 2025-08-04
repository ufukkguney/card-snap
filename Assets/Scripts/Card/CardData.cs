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
        attack = attackValue;
        defense = defenseValue;
    }

    public CardType CardType
    {
        get { return cardType; }
        set { cardType = value; }
    }

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public int Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public override string ToString()
    {
        return $"Card: {cardType}, Attack: {attack}, Defense: {defense}";
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
    Lewandowski,
    Kane,
    DeBruyne,
    ArdaTuran
}