using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] List<Card> deck = new List<Card>();

    [SerializeField] int numPlayers = 2;
    [SerializeField] List<Player> players = new List<Player>();

    public Card cardTemplate;

    void Awake()
    {

        CreateStdDeck();

        SetupGame();

        
    }

    void SetupGame()
    {
        

        // Ask for player names
        List<string> playerNames = new List<string>();
        if (players.Count > 0)
        {
            foreach (Player player in players)
            {
                playerNames.Add(player.playerName);
            }
        }
        
        players.Clear();
        List<Card>[] splits = Split(numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player());                      // THIS NOT GONNA WORK
            players[i].Setup(playerNames[i], splits[i]);
        }
    }

    // Create 52 card deck (no jokers)
    void CreateStdDeck()
    {
        for (int suit = 1; suit <= 4; suit++)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card card = Instantiate(cardTemplate, gameObject.transform);
                card.Setup((SuitEnum)suit, (RankEnum)rank);
                print(card.Name);
                deck.Add(card);
            }
        }
    }

    // Move card from deck Src to deck Dst
    // Returns index of moved card in dst deck
    int MoveCard(Card card, List<Card> deckSrc, List<Card> deckDst)
    {
        deckDst.Add(card);
        deckSrc.Remove(card);
        return deckDst.Count - 1;
    }

    void Shuffle(List<Card> _deck)
    {
        int count = _deck.Count;
        for (int i = 0; i < count - 1; ++i)
        {
            int r = Random.Range(i, count);
            Card tmp = _deck[i];
            _deck[i] = _deck[r];
            _deck[r] = tmp;
        }
    }

    List<Card>[] Split(int divisions)
    {
        List<Card>[] splits = new List<Card>[divisions];
        //for (int i = 0; i < divisions; i++)
        //{
        //    splits[i] = new List<Card>();
        //}
        for (int i = 0; i < deck.Count; i++)
        {
            splits[i % divisions].Add(deck[i]);
        }
        return splits;
    }

    void ResetGame()
    {

    }

    void PrintDeck(List<Card> _deck)
    {
        foreach (Card card in _deck)
        {
            print(card.Name);
        }
    }
}