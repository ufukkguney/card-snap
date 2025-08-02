using UnityEngine;

[System.Serializable]
public struct CardData
{
    [SerializeField] private string cardName;
    [SerializeField] private int attack;
    [SerializeField] private int defense;

    public CardData(string name, int attackValue, int defenseValue)
    {
        cardName = name;
        attack = attackValue;
        defense = defenseValue;
    }

    public string CardName
    {
        get { return cardName; }
        set { cardName = value; }
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
        return $"Card: {cardName}, Attack: {attack}, Defense: {defense}";
    }
}