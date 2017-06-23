using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// A floating health bar used to show the actual life of an enemy.
/// </summary>
public class HealthBar : UIElement {

    LivingBeing target;
    public Image remainingHealth;
    Transform position;

    public void Initialize(LivingBeing t)
    {
        target = t;
        position = target.transform.Find("HealthBarPosition");
        position.GetComponent<MeshRenderer>().enabled = false;
    }

	void Update () {
        try
        {
            transform.position = Camera.main.WorldToScreenPoint(position.position);
            remainingHealth.fillAmount = target.GetNormalizedHealth();
        }catch(MissingReferenceException ex)
        {
            Destroy(gameObject);
        }
    }
}
