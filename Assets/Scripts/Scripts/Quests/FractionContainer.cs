using UnityEngine;
using System.Collections.Generic;
using Items;
using Utils;
using Quests;

/// <summary>
/// Used to manage the logic of a fraction alatair, deciding 
/// which fractions are going to be given to the player.
/// </summary>
public class FractionContainer : MonoBehaviour {
    
	public List<Fraction> toGiveFractions;
	FractionShooter _fractionShooter;
	AudioClip _gotFractionsSound;

	void Awake()
	{
		_fractionShooter = GetComponent<FractionShooter> ();
	}

	void Start()
	{
		_gotFractionsSound = JukeBox.instance.LoadSound ("objective_done");
		int numOptions = 3;
		int choosenIndex;
		Fraction.SimplifiedFraction solution = DataManager.instance.ActualQuest.solution;
		List<int> possibilities = new List<int> ();
		for (int i = 0; i < solution.down; i++) {
			if (i == solution.top)
				continue;
			possibilities.Add (i);
		}

		toGiveFractions = new List<Fraction> ();

		if (solution.down <= 3) {
			numOptions = solution.down - 1;
		}
        int multiplier;
        for (int i = 0; i < numOptions; i++){
            multiplier = Random.Range(1, Mathf.RoundToInt(DataManager.instance.Level / 5) + 1);
            multiplier = multiplier == 0 ? 1 : multiplier;
			choosenIndex = Random.Range (0, possibilities.Count);
			toGiveFractions.Add (new Fraction (Constants.ID_FRACTION, "Fraction", "inventory_fraction", possibilities[choosenIndex] * multiplier, DataManager.instance.ActualQuest.solution.down * multiplier));
			possibilities.RemoveAt (choosenIndex);
		}
        multiplier = Random.Range(1, Mathf.RoundToInt(DataManager.instance.Level / 5));
        multiplier = multiplier == 0 ? 1 : multiplier;
        var rnd = new System.Random ();
		List<Fraction> tmp = new List<Fraction> ();
        tmp.Add(new Fraction(Constants.ID_FRACTION, "Fraction", "inventory_fraction", DataManager.instance.ActualQuest.solution.top * multiplier, DataManager.instance.ActualQuest.solution.down * multiplier));

        while (toGiveFractions.Count > 0) {
			choosenIndex = Random.Range (0, toGiveFractions.Count);
			tmp.Add (toGiveFractions [choosenIndex]);
			toGiveFractions.RemoveAt (choosenIndex);
		}
		toGiveFractions = tmp;

        int pos = Random.Range(0, toGiveFractions.Count);
        var frac = toGiveFractions[pos];
        toGiveFractions[pos] = toGiveFractions[0];
        toGiveFractions[0] = frac;
	}

	public void GiveFractions()
	{
		JukeBox.instance.Play (_gotFractionsSound, 1);
		foreach (Fraction fraction in toGiveFractions) {
            QuestManager.instance.Inventory.addItem(fraction, 1);	
		}
		Destroy (gameObject, 0.1f);
        LevelController.instance.Hero.PlayAnimation(HumanNPC.NPC_ANIMATION.IDLE_1);
    }

}
