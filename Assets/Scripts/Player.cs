using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Card> hand = new List<Card>();
    public string playerName;
    int playerID;
    public bool isMyTurn;

    CardManager cm;

    private void Update()
    {
        if (isMyTurn && (Input.GetKeyDown(KeyCode.A) && playerID == 0) || (Input.GetKeyDown(KeyCode.K) && playerID == 1))
        {
            cm.Deal(hand);
            cm.nextTurn(playerID);
        }
        else if ((Input.GetKeyDown(KeyCode.S) && playerID == 0) || (Input.GetKeyDown(KeyCode.L) && playerID == 1))
        {
            cm.Slap(this);
        }
    }

    public void Setup(string _playerName, int _playerID, CardManager _cm, List<Card> initHand)
    {
        playerName = _playerName;
        playerID = _playerID;
        cm = _cm;
        hand = initHand;
    }

    public void PrintHand()
    {
        foreach (Card card in hand)
        {
            print(playerName + " : " + card.Name);
        }
    }
}