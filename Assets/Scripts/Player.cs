using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] List<Card> hand = new List<Card>();
    public string playerName;

    void Awake()
    {
        
    }

    public void Setup(string _playerName, List<Card> initHand)
    {
        playerName = _playerName;
        hand = initHand;
    }
}