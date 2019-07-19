using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] List<Card> deck = new List<Card>();

    [SerializeField] int numPlayers = 2;
    [SerializeField] List<Player> players = new List<Player>();

    public Card cardTemplate;
    public Player playerTemplate;

    bool slapped;

    void Awake()
    {

        CreateStdDeck();
        Shuffle(deck);

        SetupGame();

        foreach (Player player in players)
        {
            player.PrintHand();
        }
        PrintDeck(deck);
    }

    private void Update()
    {
        
    }

    void SetupGame()
    {
        // Ask for player names
        List<string> playerNames = new List<string>();
        playerNames.Add("Bill");
        playerNames.Add("Alice");

        // Creating deck and shuffling
        CreateStdDeck();
        Shuffle(deck);

        players.Clear();
        List<Card>[] splits = Split(deck, numPlayers);

        // Adding player 1
        Player player = Instantiate(playerTemplate);
        players.Add(player);
        players[0].Setup(playerNames[0], 0, this, splits[0]);

        // Adding player 2
        player = Instantiate(playerTemplate);
        players.Add(player);
        players[1].Setup(playerNames[1], 1, this, splits[1]);

        //for (int i = 0; i < numPlayers; i++)
        //{
        //    Player player = Instantiate(playerTemplate);
        //    players.Add(player);
        //    players[i].Setup(playerNames[i], this, splits[i]);
        //}
    }

    // pid = of player whose turn it is
    void Turn(int pid)
    {
        foreach (Player player in players)
        {
            player.isMyTurn = false;
        }
        players[pid].isMyTurn = true;
    }

    // pid = of player who completed turn
    public void nextTurn(int pid)
    {
        Turn((pid + 1) % numPlayers);
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
                deck.Add(card);
            }
        }
    }

    // Move card from deck Src to deck Dst
    // Returns index of moved card in dst deck
    void MoveCard(Card card, List<Card> deckSrc, List<Card> deckDst, int loc = -1)
    {
        if (loc < 0)
        {
            deckDst.Add(card);
        }
        else
        {
            deckDst.Insert(loc, card);
        }
        deckSrc.Remove(card);
    }

    public void Deal(List<Card> playerHand)
    {
        MoveCard(playerHand[0], playerHand, deck);
    }

    public void Slap(Player player)
    {
        // do slapping animation for each player

        if (!slapped && deck[deck.Count-1].Rank == deck[deck.Count-2].Rank)
        {
            slapped = true;
            player.hand.AddRange(deck);
            deck.Clear();
        }
        else
        {
            Penalty(player);
        }
    }

    void Penalty(Player player)
    {
        MoveCard(player.hand[0], player.hand, deck, 0);
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

    List<Card>[] Split(List<Card> _deck, int divisions)
    {
        List<Card>[] splits = new List<Card>[divisions];
        for (int i = 0; i < divisions; i++)
        {
            splits[i] = new List<Card>();
        }
        int deckSize = _deck.Count;
        for (int i = 0; i < deckSize; i++)
        {
            MoveCard(_deck[0], _deck, splits[i % divisions]);
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
            print("Deck : " + card.Name);
        }
    }
}