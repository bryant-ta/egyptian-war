using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] List<Card> deck = new List<Card>();

    [SerializeField] int numPlayers = 2;
    public List<Player> players = new List<Player>();
    List<string> playerNames = new List<string>();

    public Card cardTemplate;
    public Transform player1Loc;
    public Transform player2Loc;
    public GameObject deckLoc;
    public Transform penaltyLoc;

    bool slapped;
    public bool gameEnded = true;

    public Image player1BarFill;
    public Text player1NameTxt;
    public Text player2NameTxt;
    public InputField player1Input;
    public InputField player2Input;
    public GameObject menu;
    public Text winText;

    public void StartGame()
    {
        playerNames.Clear();
        playerNames.Add(player1Input.text);
        playerNames.Add(player2Input.text);
        if (player1Input.text == "Enter Name" || player1Input.text == "") playerNames[0] = "Player 1";
        if (player2Input.text == "Enter Name" || player2Input.text == "") playerNames[1] = "Player 2";
        player1NameTxt.text = playerNames[0];
        player2NameTxt.text = playerNames[1];

        player1BarFill.fillAmount = 0.5f;

        menu.SetActive(false);
        winText.gameObject.SetActive(false);

        SetupGame();
    }

    void SetupGame()
    {
        foreach (Player player in players)
            player.gameObject.SetActive(true);

        // Creating deck and shuffling
        CreateStdDeck();
        Shuffle(deck);

        List<Card>[] splits = Split(deck, numPlayers);
        players[0].Setup(playerNames[0], 0, this, splits[0]);
        players[1].Setup(playerNames[1], 1, this, splits[1]);

        gameEnded = false;
        Turn(0);
    }

    // pid = of player whose turn it is
    void Turn(int pid)
    {
        foreach (Player player in players)
        {
            player.isMyTurn = false;
        }
        players[pid].isMyTurn = true;

        if (players[pid].hand.Count == 0)
        {
            EndGame(pid);
        }
    }

    // pid = of player who completed turn
    public void nextTurn(int pid)
    {
        if (pid == 0)
        {
            Turn(1);
        }
        else
        {
            Turn(0);
        }
    }

    // Create 52 card deck (no jokers)
    void CreateStdDeck()
    {
        for (int suit = 1; suit <= 4; suit++)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card card = Instantiate(cardTemplate, deckLoc.transform);
                card.Setup((SuitEnum)suit, (RankEnum)rank);
                deck.Add(card);
            }
        }
    }

    // Move card from deck Src to deck Dst
    // Returns index of moved card in dst deck
    Card MoveCard(Card card, List<Card> deckSrc, List<Card> deckDst, Transform loc, int index = -1)
    {
        card.transform.SetParent(loc, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        if (index < 0)
        {
            deckDst.Add(card);
            deckDst[deckDst.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = deckDst.Count - 1;
        }
        else
        {
            deckDst.Insert(index, card);
            deckDst[index].GetComponent<SpriteRenderer>().sortingOrder = deckDst.Count - 1; // lazy but it works
        }
        if (deckDst == deck)
            RuffleCard(card);
        deckSrc.Remove(card);
        return card;
    }

    public void Deal(Player player)
    {
        Player otherPlayer;
        if (player.playerID == 0)
            otherPlayer = players[1];
        else
            otherPlayer = players[0];

        Card playedCard = MoveCard(player.hand[0], player.hand, deck, deckLoc.transform);
        slapped = false;

        switch ((int)playedCard.Rank)
        {
            case 11:
                player.cardsToFulfill = 0;
                otherPlayer.cardsToFulfill = 1;
                break;
            case 12:
                player.cardsToFulfill = 0;
                otherPlayer.cardsToFulfill = 2;
                break;
            case 13:
                player.cardsToFulfill = 0;
                otherPlayer.cardsToFulfill = 3;
                break;
            case 1:
                player.cardsToFulfill = 0;
                otherPlayer.cardsToFulfill = 4;
                break;
            default:
                break;
        }

        if (player.cardsToFulfill == 0)
        {
            nextTurn(player.playerID);
        }
        else
        {
            if (player.hand.Count == 0)
            {
                EndGame(player.playerID);
            }

            player.cardsToFulfill--;
            if (player.cardsToFulfill == 0)
            {
                otherPlayer.canTake = true;
                Turn(otherPlayer.playerID);
            }
        }
    }

    public void Slap(Player player)
    {
        if (deck.Count < 2) return;

        // do slapping animation for each player

        if ((((int)deck[deck.Count - 1].Rank == (int)deck[deck.Count - 2].Rank) ||
            (deck.Count > 2 && (int)deck[deck.Count - 1].Rank == ((int)deck[deck.Count - 3].Rank))))
        {
            slapped = true;
            TakeAll(player);
        }
        else if (!slapped)
        {
            Penalty(player);
        }
    }

    void Penalty(Player player)
    {
        MoveCard(player.hand[0], player.hand, deck, penaltyLoc, 0);
    }

    public void TakeAll(Player player)
    {
        int deckSize = deck.Count;
        for (int i = 0; i < deckSize; i++)
        {
            MoveCard(deck[0], deck, player.hand, player.transform);
        }
        players[0].cardsToFulfill = 0;
        players[1].cardsToFulfill = 0;

        player1BarFill.fillAmount = (float)players[0].hand.Count / 52.0f;

        Turn(player.playerID);
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
            MoveCard(_deck[0], _deck, splits[i % divisions], players[i%divisions].gameObject.transform);
        }
        return splits;
    }

    void EndGame(int pid)
    {
        Player otherPlayer;
        if (pid == 0)
            otherPlayer = players[1];
        else
            otherPlayer = players[0];

        gameEnded = true;
        menu.SetActive(true);
        winText.gameObject.SetActive(true);
        winText.text = otherPlayer.playerName + " Won!";

        foreach (Player player in players)
            player.gameObject.SetActive(false);
    }

    void RuffleCard(Card card)
    {
        card.transform.Rotate(new Vector3(0, 0, Random.Range(-20, 21)));
    }

    void PrintDeck(List<Card> _deck)
    {
        foreach (Card card in _deck)
        {
            print("Deck : " + card.Name);
        }
    }
}