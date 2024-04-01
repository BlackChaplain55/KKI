using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/Puzzle Collection")]

public class PuzzleSO : ScriptableObject
{
    public List<Puzzle> PuzzleCollection;

    public Puzzle GetPuzzle(string puzzleName)
    {
        Puzzle puzzle = PuzzleCollection.Find(p => p.puzzleName == puzzleName);
        return puzzle;
    }
}

[Serializable]
public struct Puzzle
{
    public string puzzleName;
    public List<PuzzleAnswer> Answers;
    [TextArea(15, 20)] public string Question;
    public Card RewardCard;
    public bool solved;
}

[Serializable]
public struct PuzzleAnswer
{
    public string Answer;
    public string GodReaction;
    public GameObject RewardCard;
    public int InitialDeckBonus;
    public int TurnCardBonus;
    public int InitialAP;
    public int TurnAPBonus;
}