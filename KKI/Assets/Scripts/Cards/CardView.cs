using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

//Этот модкль отвечает за визуал карты

public class CardView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    //[SerializeField] private MeshRenderer _cardFront;
    [SerializeField] private Card _card;
    [SerializeField] private Game _game;
    [SerializeField] private ParticleSystem _selectedVFX;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _fullViewPanel;
    [SerializeField] private Material _darkMaterial;
    [SerializeField] private TMP_Text _stats;
    [SerializeField] private TMP_Text _personalStats;
    [SerializeField] private TMP_Text _APCost;
    [SerializeField] private TMP_Text _fullViewInfo;
    [SerializeField] private GameObject _deckBuildsButtons;
    [SerializeField] private GameObject _playModeButtons;

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
        Transform canvas = transform.Find("CardModel")?.Find("Card")?.Find("CardInfoCanvas");
        if (!_fullViewPanel) _fullViewPanel = canvas?.Find("FullView")?.gameObject;
        if (!_APCost) _APCost = canvas?.Find("APCost")?.GetComponent<TMP_Text>();
        if (!_stats) _stats = canvas?.Find("Stats")?.GetComponent<TMP_Text>();
        if (!_personalStats) _personalStats = canvas?.Find("PersonalStats")?.GetComponent<TMP_Text>();
        if (!_fullViewPanel) _fullViewPanel = canvas?.Find("FullView")?.gameObject;
        if (!_fullViewInfo) _fullViewInfo = canvas?.Find("FullView")?.Find("CardInfoPanel")?.Find("Info")?.GetComponent<TMP_Text>();
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
        if (!_card) _card = GetComponent<Card>();
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_anim) _anim = GetComponent<Animator>();
        if (!_selectedVFX) _selectedVFX = transform.Find("CardModel").Find("SelectedVFX").GetComponent<ParticleSystem>();
    }

    public void Init(Card card)
    {
        _card = card;

        EventBus.Instance.UpdateCards.AddListener(OnUpdateCards);
        EventBus.Instance.UnitActivationFinished.AddListener(OnDeactivateUnit);
        List<Material> materials = new();
        if (_card.IsDark)
        {
            materials.Add(_darkMaterial);
        }
        else
        {
            materials.Add(_meshRenderer.sharedMaterials[0]); 
        }
        materials.Add(_card.CurrentGame.CardCollection.GetMaterial(_card.Color));

        _meshRenderer.SetMaterials(materials);

        _APCost.text = "ОД:" + _card.APCost.ToString();

        FillStats(_stats,true);
        FillStats(_fullViewInfo,false,true);
        _personalStats.text = "";
    }    

    public void UpdateView()
    {
        FillStats(_stats, true);
        FillStats(_personalStats);
    }

    private void OnUpdateCards()
    {
        UpdateView();
    }

    private void OnDeactivateUnit()
    {
        _personalStats.text = "";
    }

    private void StateChanged(IState cardState, bool pointerEnter)
    {
        
    }

    private void FillStats(TMP_Text textPanel, bool mainStats = false, bool allUnits = false, bool clear = true)
    {
        if (clear) textPanel.text = "";
        Unit activeUnit = _card.CurrentGame.Combat?.ActiveUnit;

        if (mainStats)
        {
            textPanel.text = GetEffectDescriptionString(_card.Effect, activeUnit);
        }
        else if (allUnits)
        {
            List<Unit> units = _card.GetPersonatEffectsUnitList();

            foreach (Unit unit in units)
            {
                textPanel.text += "<b><u>" + unit.Name + ":</u><b>\r\n";
                List<CardEffect> allEffects = _card.GetPersonatEffectsList(unit);
                foreach (var personalEffect in allEffects)
                {
                    textPanel.text += GetEffectDescriptionString(personalEffect, unit);
                }
            }
        }
        else
        {    
            var personalEffects = _card.GetPersonalEffect(activeUnit.name);
            if (personalEffects.Count == 0) return;
            textPanel.text = "<b><u>" +activeUnit.Name + ":</u><b>\r\n";
            foreach (CardEffect personalEffect in personalEffects)
            {
                textPanel.text += GetEffectDescriptionString(personalEffect, activeUnit);
            }
        }
    }

    private string GetEffectDescriptionString(CardEffect effect, Unit cardUser = null)
    {
        string greenColor = ColorUtility.ToHtmlStringRGB(Color.green);
        string redColor = ColorUtility.ToHtmlStringRGB(Color.red);
        if (_card.CurrentGame.Combat)
        {
            greenColor = ColorUtility.ToHtmlStringRGB(_card.CurrentGame.Combat.InfoBonusColor);
            redColor = ColorUtility.ToHtmlStringRGB(_card.CurrentGame.Combat.InfoMalusColor);
        }
        
        string description = "";
        if (effect.type != EffectTypes.none)
        {
            if (effect.EffectName!="") description += effect.EffectName;
            string cardEffectDescription = _card.CurrentGame.CardCollection.GetEffectDescription(effect.type);
            if (cardEffectDescription!="") description += " - " + cardEffectDescription;

            if (effect.isAOE)
            {
                description += "(AOE)" + "\r\n";
            }
            else
            {
                if(effect.EffectName != "" && cardEffectDescription != "") description += "\r\n";
            }
        }
        else
        {
            description += effect.EffectName+ "\r\n";
            description += "Эффект зависит от героя";
        }
        

        if (effect.Damage > 0)
        {
            
            if (cardUser)
            {

                float pDamageBonus = effect.Damage * (cardUser.Damage + cardUser.Bonus.Damage) - effect.Damage;
                //string damageBonusColor = pDamageBonus >= 0 ? "<color=#" + greenColor + ">" : "<color=\"red\">";
                //description += " + " + damageBonusColor + pDamageBonus.ToString() + "</color>" + "(бонус "+cardUser.Name+")";
                description += pDamageBonus.ToString();
            }
            else
            {
                description += " " + effect.Damage.ToString()+" основного ";
            }
            description += " физ. урона" + "\r\n";
        }

        if (effect.MDamage > 0)
        {
            description += " " + effect.MDamage.ToString();
            if (cardUser)
            {
                float mDamageBonus = effect.MDamage * (cardUser.MDamage + cardUser.Bonus.MDamage) - effect.MDamage;
                string damageBonusColor = mDamageBonus >= 0 ? "<color=#" + greenColor + ">" : "<color=\"red\">";
                description += " " + effect.MDamage.ToString() + damageBonusColor + mDamageBonus.ToString() + "</color>";
            }
            description += " маг. урона" + "\r\n";
        }

        if (effect.DamageBonus > 0)
        {
            description += " бонус силы" + " +" + effect.DamageBonus.ToString();
        }

        if (effect.DamageBonus < 0)
        {
            description += " штраф силы" + " " + effect.DamageBonus.ToString();
        }

        if (effect.DefenceBonus > 0)
        {
            description += " +" + effect.DefenceBonus.ToString() + " бонус защиты";
        }

        if (effect.DefenceBonus < 0)
        {
            description += " " + effect.DefenceBonus.ToString() + " штраф защиты";
        }

        if (effect.Heal > 0)
        {
            description += " +" + effect.Damage.ToString() + " здоровья";
        }

        if (effect.InitiativeBonus > 0)
        {
            description += " +" + effect.InitiativeBonus.ToString() + "";
        }

        if (effect.InitiativeBonus < 0)
        {
            description += " " + effect.InitiativeBonus.ToString() + "";
        }

        if (effect.InitiativeBoost != 0)
        {
            description += " на " + effect.InitiativeBoost.ToString();
        }

        if (effect.Vampiric > 0)
        {
            description += " лечение на " + effect.Vampiric.ToString() + "% от урона ";
        }

        if (effect.MovesCount > 0)
        {
            description += " на " + effect.MovesCount.ToString() + " х." + "\r\n";
        }
        else
        {
            //description += "\r\n";
        }

        return description;
    }

    public void SetFullView(bool playState)
    {
        _fullViewPanel.SetActive(true);
        _anim.SetTrigger("FullView");
        if (playState)
        {
            _deckBuildsButtons.SetActive(false);
            _playModeButtons.SetActive(true);
        }
        else
        {
            _deckBuildsButtons.SetActive(true);
            _playModeButtons.SetActive(false);
        }
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