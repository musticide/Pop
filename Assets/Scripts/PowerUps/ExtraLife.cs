using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PowerUps/ExtraLife")]
public class ExtraLife : PowerUp
{
    public override void Apply()
    {
        GameManager.Instance.GainLife();
    }
}

