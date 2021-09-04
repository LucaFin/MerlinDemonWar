﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer;
    public Card sourceCard;
    public Image effectImage;

    public void EndTrigger()
    {
        if (targetPlayer.hasMirror())
        {
            targetPlayer.SetMirror(false);
            if (targetPlayer.isPlayer)
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.enemy);
            }else
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.player);
            }
        }
        else
        {

            int damage = sourceCard.cardData.damage;
            if (!targetPlayer.isPlayer)
            {
                if (sourceCard.cardData.damageType == CardData.DamageType.Fire && targetPlayer.isFire)
                {
                    damage = damage / 2;
                }
                if (sourceCard.cardData.damageType == CardData.DamageType.Ice && !targetPlayer.isFire)
                {
                    damage = damage / 2;
                }
                targetPlayer.health -= damage;
                targetPlayer.PlayHitAnim();
                GameController.instance.UpdateHealths();
                GameController.instance.isPlayeble = true;
                
            }
        }
        Destroy(gameObject);
    }
}
