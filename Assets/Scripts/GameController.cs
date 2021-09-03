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
                if(usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
                if(!usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
            }
        }
        else
        {
            if (!usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
            {
                valid = true;
            }
            if (usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
            {
                valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {

        }
        else
        {
            if (card.cardData.isDefenseCard)
            {

            }
            else
            {

            }
        }
    }

    internal void CastAttackEffect(Card card, Player usingOnPlayer)
    {

    }
}
