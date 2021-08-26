using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Image playerImage;
    public Image mirrorImage;
    public Image healthNumberImage;
    public Image glowImage;

    public int health = 5;
    public int mana = 1;

    public bool isPlayer;
    public bool isFire;

    public GameObject[] manaBalls = new GameObject[5];
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
