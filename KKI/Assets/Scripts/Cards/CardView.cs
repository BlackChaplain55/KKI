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
    [SerializeField] private GameObject _fullViewPanel;

    public Animator Anim => _anim;
    public ParticleSystem SelectedVFX => _selectedVFX;

    private void OnEnable()
    {
        _fullViewPanel.SetActive(false);
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
        if (!_fullViewPanel) _fullViewPanel = transform.Find("CardModel")?.Find("Card")?.Find("CardInfoCanvas")?.Find("FullView")?.gameObject;
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
        if (!_card) _card = GetComponent<Card>();
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_anim) _anim = GetComponent<Animator>();
        if (!_selectedVFX) _selectedVFX = transform.Find("CardModel").Find("SelectedVFX").GetComponent<ParticleSystem>();
    }

    private void StateChanged(IState cardState, bool pointerEnter)
    {
        
    }

    public void SetFullView()
    {
        _fullViewPanel.SetActive(true);
        _anim.SetTrigger("FullView");
    }

    public void SetAnimation(string animation, bool value, bool isTrigger)
    {
        if (!isTrigger)
        {
            _anim.SetBool(animation, value);
        }
        else
        {
            _anim.SetTrigger(animation);
        }
    }
}