using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage;
    public Image mirrorImage;
    public Image healthNumberImage;
    public Image glowImage;

    public int maxHealth = 5;
    public int health = 5;
    public int mana = 1;

    public bool isPlayer;
    public bool isFire;

    public GameObject[] manaBalls = new GameObject[5];

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateHealth();
    }

    internal void PlayHitAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayeble)
        {
            return;
        }
        GameObject obj = eventData.pointerDrag;
        if (obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if (card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playersHand);
            }
        }
    }

    internal void UpdateHealth()
    {
        if(health>=0 && health < GameController.instance.healthNumbers.Length)
        {
            healthNumberImage.sprite = GameController.instance.healthNumbers[health];
        }
        else
        {
            Debug.Log("Health is not a valid number," + health.ToString());
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }

    internal bool hasMirror()
    {
        return mirrorImage.gameObject.activeInHierarchy;
    }
}
