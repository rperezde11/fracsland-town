using UnityEngine;
using Items;
using Utils;
using Quests;

/// <summary>
/// In this quest the player has to kill captain john, disabling its 
/// inmortality with a fraction.
/// </summary>
public class BossQuest : Quest
{ 
    public MinimapMarker SelfMarker;
    private LivingBeing _livingBeing;
    public GameObject PieZone;
    private PieFraction _pieFractionCreator;
    private EnemyHuman _human;
    private bool _isPlayerInRange = false;

    bool hasBeenConfigured = false;

    public override void Configure(ConfiguredQuest configuration)
    {
        hasBeenConfigured = true;
        SelfMarker.EnableMarker();
        GUIController.instance.ClearAllGoals();
        GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, Description, gameObject, this);
        _livingBeing = GetComponent<LivingBeing>();
        _livingBeing.MakeInMortal(true);
        _human = GetComponent<EnemyHuman>();
        _pieFractionCreator = PieZone.GetComponent<PieFraction>();
        _pieFractionCreator.GenerateTexture();
    }

    public override void Fail()
    {
        JukeBox.instance.LoadAndPlaySound("Jaja", 1);
        ((JohnTheBuilder)_human).AttackAndStunPlayer();
    }

    public override void Interact()
    {
        if (!hasBeenConfigured)
        {
            Destroy(gameObject);
            return;
        }
        if (_isDone) return;
        if (_livingBeing.isDead())
        {
            PostSuccess();
            return;
        }
        if (Vector3.Distance(transform.position, LevelController.instance.Hero.transform.position) < 6)
        {
            if (_isPlayerInRange) return;
            GUIController.instance.ClearQuestGoals(this);
            if (((JohnTheBuilder)_human).IsInmortal)
            {
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Destroy his power with a fraction!!", gameObject, this);
            }
            else
            {
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Now you can defeat or forgive him!!", gameObject, this);
            }
            
            _isPlayerInRange = true;
        }
        else
        {
            if (!_isPlayerInRange) return;
            GUIController.instance.ClearQuestGoals(this);
            if (((JohnTheBuilder)_human).IsInmortal)
            {
                GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, Description, gameObject, this);
            }
            else
            {
                PostSuccess();
            }
            _isPlayerInRange = false;
        }
    }

    public override void Restart()
    {

    }

    public override void Setup()
    {
    }

    public override void Solve(Fraction fraction)
    {
        if (fraction == DataManager.instance.ActualQuest.solution)
        {
            Success();
        }
        else
        {
            Fail();
        }
        SendTry(fraction, DataManager.instance.ActualQuest.questConfigurationID);
    }

    public override void Success()
    {
        JukeBox.instance.LoadAndPlaySound("objective_done", 1);
        _livingBeing.MakeInMortal(false);
        _human.FallToTheGround();
        ((JohnTheBuilder)_human).DowngradeForce();
        _pieFractionCreator.gameObject.SetActive(false);
        GUIController.instance.ClearQuestGoals(this);
        GUIController.instance.ShowGoal(UIGoal.GOALTYPE.VIOLENT, "Now you can defeat or forgive him!!", gameObject, this);
    }

    public void PostSuccess()
    {
        QuestManager.instance.ChageActualQuestState(QuestManager.ActualQuestStates.PENDING_TO_REWARD);
        _isDone = true;
    }
}
