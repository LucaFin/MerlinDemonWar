using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public List<CardData> cards = new List<CardData>();

    public Player player;
    public Player enemy;

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public Hand playersHand = new Hand();
    public Hand enemysHand = new Hand();

    public GameObject cardPrefab;
    public Canvas canvas;

    public bool isPlayeble = false;

    public GameObject effectFromLeftPrefab;
    public GameObject effectFromRightPrefab;

    public Sprite fireBallImage;
    public Sprite iceBallImage;
    public Sprite multiFireBallImage;
    public Sprite multiIceBallImage;
    public Sprite fireAndIceBallImage;

    private void Awake()
    {
        instance = this;
        playerDeck.Create();
        enemyDeck.Create();
        StartCoroutine(DealHands());
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        Debug.Log("SkipTurn");
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for (int t = 0; t < 3; t++)
        {
            playerDeck.DealCard(playersHand);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1);
        }
        isPlayeble = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (!CardValid(card, usingOnPlayer, fromHand))
        {
            return false;
        }
        isPlayeble = false;
        CastCard(card, usingOnPlayer, fromHand);
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
        fromHand.RemoveCard(card);
        return false;
    }

    internal bool CardValid(Card cardBeingPlayed, Player usingOnPlayer, Hand fromHand)
    {
        bool valid = false;
        if (cardBeingPlayed == null)
        {
            return false;
        }
        if (fromHand.isPlayers)
        {
            if (cardBeingPlayed.cardData.cost <= player.mana)
            {
                valid = usingOnPlayer.isPlayer == cardBeingPlayed.cardData.isDefenseCard;
            }
        }
        else
        {
            valid = usingOnPlayer.isPlayer != cardBeingPlayed.cardData.isDefenseCard;
        }
        return valid;
    }

    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            isPlayeble = true;
        }
        else
        {
            if (card.cardData.isDefenseCard)
            {
                usingOnPlayer.health += card.cardData.damage;
                if (usingOnPlayer.health > usingOnPlayer.maxHealth)
                {
                    usingOnPlayer.health = usingOnPlayer.maxHealth;
                }
                UpdateHealths();

                StartCoroutine(CastHealEffect(usingOnPlayer));
                
            }
            else
            {
                CastAttackEffect(card, usingOnPlayer);
            }
        }
    }

    private IEnumerator CastHealEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        isPlayeble = true;
    }

    internal void CastAttackEffect(Card card, Player usingOnPlayer)
    {
        GameObject effectGO;
        if (usingOnPlayer.isPlayer)
        {
            effectGO = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        }
        else
        {
            effectGO = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);
        }
        Effect effect = effectGO.GetComponent<Effect>();
        if (effect)
        {
            effect.targetPlayer = usingOnPlayer;
            effect.sourceCard = card;

            switch (card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    effect.effectImage.sprite = card.cardData.isMulti ? multiFireBallImage : fireBallImage;
                    break;
                case CardData.DamageType.Ice:
                    effect.effectImage.sprite = card.cardData.isMulti ? multiIceBallImage : iceBallImage;
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                    break;
            }
        }
    }
    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();
        if (player.health <= 0)
        {

        }
        if (enemy.health <= 0)
        {

        }
    }
}
