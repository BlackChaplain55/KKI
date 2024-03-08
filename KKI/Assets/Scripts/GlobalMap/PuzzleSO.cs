using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/Puzzle Collection")]

public class PuzzleSO : ScriptableObject
{
    public List<Puzzle> PuzzleCollection;
}

[Serializable]
public struct Puzzle
{
    public List<PuzzleAnswer> Answers;
    public string Question;
    public Card RewardCard;
    public bool solved;
}

[Serializable]
public struct PuzzleAnswer
{
    public string Answer;
    public string GodReaction;
}