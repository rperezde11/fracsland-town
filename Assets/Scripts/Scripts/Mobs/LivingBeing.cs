using UnityEngine;
using System.Collections;

/// <summary>
/// Every entity of fracsland who can die (or eventually it's inmortal) is a living being
/// this class handles the life state of an entity.
/// </summary>
public abstract class LivingBeing : MonoBehaviour {
    
    public enum LIFE_STATE {DEAD, ALIVE};
    protected LIFE_STATE _actualLifeState;
    protected float _health, _maxHealth;
    public delegate void ActualLogicUpdate();
    protected ActualLogicUpdate _actualUpdateLogic, _newUpdateLogic, _lastUpdateLogic;
    public GameObject HealthBarPosition = null;
    protected HealthBar _healthBar;

    protected bool _isInmortal = false;
    public bool IsInmortal { get { return _isInmortal; } }

    protected float _changeUpdateMethodTime = 0;

    protected abstract void Die();    

    public bool isDead()
    {
        return _health <= 0;
    } 

    public virtual void TakeDammage(float dmg)
    {
        if (_isInmortal)
        {
            GUIController.instance.CreateFloatingText("I'm Inmortal", transform, Color.white);
        }
        else
        {
            GUIController.instance.CreateFloatingText("-" + dmg.ToString(), transform, Color.red);
            _health -= dmg;
            if (isDead())
            {
                _actualLifeState = LIFE_STATE.DEAD;
            }
        }
    }

    public void RestoreHealth(float quant)
    {
        _health = _health + quant < _maxHealth ? _health + quant : _maxHealth;
        GUIController.instance.CreateFloatingText("+" + quant.ToString(), transform, Color.green);
    }

    public void Ressurect()
    {
        _health = _maxHealth;
    }

    /// <summary>
    /// Returns the current health with a number between 0 and 1 in order to display it on the GUI
    /// </summary>
    public float GetNormalizedHealth()
    {
        return _health / _maxHealth;
    }

    public float GetCurrentHealthAmount()
    {
        return _health;
    }

    protected void ChangeUpdateMethodIn(float secondsToChange, ActualLogicUpdate nextUpdateLogic, bool removeActualLogic=false)
    {
        if (removeActualLogic) _actualUpdateLogic = null;
        _changeUpdateMethodTime = Time.time + secondsToChange;
        _newUpdateLogic = nextUpdateLogic;
        _actualUpdateLogic = ChangingUpdateMethodUpdate;
    }

    protected void ChangingUpdateMethodUpdate()
    {
        if (_changeUpdateMethodTime < Time.time)
        {
            _lastUpdateLogic = _actualUpdateLogic;
            _actualUpdateLogic = _newUpdateLogic;  
        }
    }

    protected void ChangeUpdateMethod(ActualLogicUpdate nextUpdateLogic)
    {
        _lastUpdateLogic = _actualUpdateLogic;
        _actualUpdateLogic = nextUpdateLogic;
    }

    public void MakeInMortal(bool inmortal)
    {
        _isInmortal = inmortal;
    }
}
