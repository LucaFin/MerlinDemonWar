using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public bool isPlayable = false;

    public GameObject effectFromLeftPrefab;
    public GameObject effectFromRightPrefab;

    public Sprite fireBallImage;
    public Sprite iceBallImage;
    public Sprite multiFireBallImage;
    public Sprite multiIceBallImage;
    public Sprite fireAndIceBallImage;

    public bool playersTurn = true;

    public Text turnText;
    public Text scoreText;
    public Image EnemySkipTurn;

    public Sprite fireDemon;
    public Sprite iceDemon;

    public int playerScore = 0;
    public int playerKill = 0;

    public AudioSource playerDieAudio;
    public AudioSource enemyDieAudio;
    private void Awake()
    {
        instance = this;
        SetUpEnemy();
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
        if(playersTurn && isPlayable)
        {
            NextPlayersTurn();
        }
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
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (!CardValid(card, usingOnPlayer, fromHand))
        {
            return false;
        }
        isPlayable = false;
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
                valid = (usingOnPlayer.isPlayer == cardBeingPlayed.cardData.isDefenseCard);
            }
        }
        else
        {
            if (cardBeingPlayed.cardData.cost <= enemy.mana)
            {
                valid = (usingOnPlayer.isPlayer != cardBeingPlayed.cardData.isDefenseCard);
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            usingOnPlayer.PlayMirrorSound();
            NextPlayersTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenseCard)
            {
                usingOnPlayer.health += card.cardData.damage;
                usingOnPlayer.PlayHealSound();
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
            if (fromHand.isPlayers)
            {
                playerScore += card.cardData.damage;
                UpdateScore();
            }
        }
        if (fromHand.isPlayers)
        {
            GameController.instance.player.mana -= card.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else
        {
            GameController.instance.enemy.mana -= card.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        NextPlayersTurn();
        isPlayable = true;
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
                    effect.PlayFireballSound();
                    break;
                case CardData.DamageType.Ice:
                    effect.effectImage.sprite = card.cardData.isMulti ? multiIceBallImage : iceBallImage;
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                    effect.PlayIceSound();
                    effect.PlayFireballSound();
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
            StartCoroutine(GameOver());
        }
        if (enemy.health <= 0)
        {
            playerKill++;
            playerScore += 100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemysHand.ClearHand();
        yield return new WaitForSeconds(0.75f);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = UnityEngine.Random.Range(0, 2) == 1;
        enemy.playerImage.sprite = enemy.isFire ? fireDemon : iceDemon;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    internal void NextPlayersTurn()
    {
        bool enemyIsDead = false;
        playersTurn = !playersTurn;
        if (playersTurn)
        {
            if (player.mana < 5)
            {
                player.mana++;
            }
        }
        else
        {
            if (enemy.health > 0)
            {
                if (enemy.mana < 5)
                {
                    enemy.mana++;
                }
            }
            else
            {
                enemyIsDead = true;
            }
        }


        if (enemyIsDead)
        {
            playersTurn = !playersTurn;
        }
        else
        {
            SetTurnNext();
            if (!playersTurn)
            {
                MonsterTurn();
            }
        }
        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    private void MonsterTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(MonsterCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> avaiable = new List<Card>();
        for (int i = 0; i < 3; i++)
        {
            if (CardValid(enemysHand.cards[i], enemy, enemysHand))
            {
                avaiable.Add(enemysHand.cards[i]);
            }
            else if (CardValid(enemysHand.cards[i], player, enemysHand))
            {
                avaiable.Add(enemysHand.cards[i]);
            }
        }
        if (avaiable.Count == 0)
        {
            NextPlayersTurn();
            return null;
        }
        int choise = UnityEngine.Random.Range(0, avaiable.Count);
        return avaiable[choise];
    }

    internal void SetTurnNext()
    {
        turnText.text = playersTurn ? "Merlin's Turn" : "Enemy's Turn";
    }

    private IEnumerator MonsterCastCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);
        if (card)
        {
            TurnCard(card);
            yield return new WaitForSeconds(2f);
            UseCard(card, card.cardData.isDefenseCard ? enemy : player, enemysHand);
            yield return new WaitForSeconds(1f);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1f);
        }
        else
        {
            EnemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            EnemySkipTurn.gameObject.SetActive(false);
        }
        
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.Log("no animator found");
        }

    }

    private void UpdateScore()
    {
        scoreText.text = "Demons killed: " + playerKill.ToString() + " Score: " + playerScore.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }

    internal void PlayEnemyDieSound()
    {
        enemyDieAudio.Play();
    }
}
