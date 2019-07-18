using UnityEngine;

public enum SuitEnum
{
    Hearts = 1,
    Clubs = 2,
    Diamonds = 3,
    Spades = 4
}

public enum RankEnum
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public class Card : MonoBehaviour
{
    

    [SerializeField] SuitEnum suit;
    [SerializeField] RankEnum rank;
    [SerializeField] string cardName;

    public void Setup(SuitEnum _suit, RankEnum _rank)
    {
        suit = _suit;
        rank = _rank;
        cardName = "" + rank.ToString() +" of " + suit.ToString();
    }

    public SuitEnum Suit { get { return suit; } }
    public RankEnum Rank { get { return rank; } }
    public string Name { get { return cardName; } }
}