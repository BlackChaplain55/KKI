using UnityEditor;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] private Color _defaultColor = Color.green;
    [SerializeField] private Color _enterColor = Color.yellow;
    [SerializeField] private Color _selectColor = Color.yellow;
    [SerializeField] private Color _selectEnterColor = Color.red;
    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Card _card;
    [SerializeField] private Game _game;
    [SerializeField] private ParticleSystem _selectedVFX;
    [SerializeField] private Animator _anim;

    public Animator Anim => _anim;
    public ParticleSystem SelectedVFX => _selectedVFX;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        _card.StateChanged += (state, oldState, sender) => StateChanged(state, sender.PointerEnter);
        _card.PointerChanged += (pointerEnter, sender) => StateChanged(sender.CurrentState, pointerEnter);
        // _meshRenderer.material.color = _defaultColor;
    }

    private void OnDisable()
    {
        _card.StateChanged -= (state, oldState, sender) => StateChanged(state, sender.PointerEnter);
        _card.PointerChanged -= (pointerEnter, sender) => StateChanged(sender.CurrentState, pointerEnter);
    }

    private void OnValidate()
    {
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
        if (!_card) _card = GetComponent<Card>();
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_anim) _anim = GetComponent<Animator>();
        if (!_selectedVFX) _selectedVFX = transform.Find("CardModel").Find("SelectedVFX").GetComponent<ParticleSystem>();
    }

    private void StateChanged(IState cardState, bool pointerEnter)
    {
        if(cardState is CardDefaultState&& pointerEnter)
        {
            //_anim.SetBool("Highlight", true);
        }else if(cardState is CardDefaultState && !pointerEnter)
        {
            //_anim.SetBool("Highlight", false);
        }
    }


}