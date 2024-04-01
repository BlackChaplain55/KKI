using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PuzzleController : MonoBehaviour
{
    [SerializeField] private GameObject _puzzlePanel;
    [SerializeField] private GlobalMapManager _glomalMapManager;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private PuzzleSO _puzzles;
    [SerializeField] private TMP_Text _questionHeader;
    [SerializeField] private GameObject _reactionPanel;
    [SerializeField] private TMP_Text _reaction;
    [SerializeField] private Encounter _enc;

    private Game _game;
    private Puzzle _currentPuzzle;

    // Start is called before the first frame update

    private void OnValidate()
    {
        if (!_buttonContainer) _buttonContainer = transform.Find("Anwers");
        if (!_puzzlePanel) _puzzlePanel = GameObject.Find("Puzzles/PuzzleCanvas").transform.Find("PuzzlePanel").gameObject;
    }
    void Start()
    {
        
    }

    public void Init(Game game, GlobalMapManager globalMapManager)
    {
        _game = game;
        _glomalMapManager = globalMapManager;
        EventBus.Instance.OnMapTrigger.AddListener(InitPuzzle);
    }

    public void InitPuzzle(GameObject puzzleGO, string puzzleName)
    {
        if (puzzleName != "")
        {
            _currentPuzzle = _puzzles.GetPuzzle(puzzleName);
        }
        else
        {
            int rnd = Random.Range(0, _puzzles.PuzzleCollection.Count - 1);
            _currentPuzzle = _puzzles.PuzzleCollection[rnd];
        }

        _enc = puzzleGO.GetComponent<Encounter>();

        _questionHeader.text = _currentPuzzle.Question;
        for (int i = _buttonContainer.childCount-1; i >= 0; i--)
        {
            Destroy(_buttonContainer.GetChild(i).gameObject);
        }
        for (int i = 0; i < _currentPuzzle.Answers.Count; i++)
        {
            PuzzleAnswer answer = _currentPuzzle.Answers[i];
            GameObject buttonGO = Instantiate(_buttonPrefab, _buttonContainer);
            buttonGO.name = "Answer" + i;
            TMP_Text label = buttonGO.GetComponentInChildren<TMP_Text>();
            label.text = answer.Answer;
            Button button = buttonGO.GetComponent<Button>();
            int currentIndex = i;
            button.onClick.AddListener(delegate { OnClickAnswer(currentIndex); });
        }
        _puzzlePanel.SetActive(true);
    }

    public void OnClickAnswer(int index)
    {
        // Debug.Log("Button click - "+ index);
        _puzzlePanel.SetActive(false);
        _reactionPanel.transform.localScale = Vector3.zero;
        _reactionPanel.SetActive(true);
        PuzzleAnswer answer = _currentPuzzle.Answers[index];
        _reaction.text = answer.GodReaction;
        _reactionPanel.transform.DOPunchScale(Vector3.one, 5,0);
        _glomalMapManager.ReckonPuzzle(_enc, answer, _enc.transform);
    }
}
