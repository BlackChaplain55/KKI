using UnityEditor;
using UnityEngine;
using TMPro;
using DG.Tweening;

//Этот модкль отвечает за визуал карты

public class CardView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Card _card;
    [SerializeField] private Game _game;
    [SerializeField] private ParticleSystem _selectedVFX;
    [SerializeField] private Animator _anim;

    public Animator Anim => _anim;
    public ParticleSystem SelectedVFX => _selectedVFX;

    private void OnEnable()
    {
        _card.StateChanged += (state, oldState, sender) => StateChanged(state, sender.PointerEnter);
        _card.PointerChanged += (pointerEnter, sender) => StateChanged(sender.CurrentState, pointerEnter);
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
        
    }


}