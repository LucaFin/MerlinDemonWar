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

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public Hand playersHand = new Hand();
    public Hand enemysHand = new Hand();

    public GameObject cardPrefab;
    public Canvas canvas;
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
    }
}
