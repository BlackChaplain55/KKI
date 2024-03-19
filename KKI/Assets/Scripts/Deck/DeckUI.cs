using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _debugText;
    [SerializeField] private Deck _deck;

    private void OnValidate()
    {
        
    }

    public void Init(Deck deck)
    {
        _deck = deck;
    }

    public void UpdateUI()
    {
        _debugText.text = "Состояние колоды:";
        _debugText.text += "\r\nДека:";
        foreach (string card in _deck.PlayerDeck)
        {
            _debugText.text += "\r\n  -" + card;
        }
        _debugText.text += "\r\nОстаток деки:";
        foreach (string card in _deck.PlayerCurrentDeck)
        {
            _debugText.text += "\r\n  -" + card;
        }
        _debugText.text += "\r\nРука:";
        foreach (string card in _deck.PlayerHand)
        {
            _debugText.text += "\r\n  -" + card;
        }
        _debugText.text += "\r\nОтбой:";
        foreach (string card in _deck.PlayerDiscard)
        {
            _debugText.text += "\r\n  -" + card;
        }
    }
}
