using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData;

    public Text titleText;
    public Text descriptionText;
    public Image damageImage;
    public Image costImage;
    public Image cardImage;
    public Image frameImage;
    public Image burnImage;

    public void Initialise()
    {
        if (cardData == null)
        {
            Debug.LogError("Card has no CardData!");
            return;
        }

        titleText.text = cardData.cardTitle;
        descriptionText.text = cardData.description;
        cardImage.sprite = cardData.cardImage;
        frameImage.sprite = cardData.frameImage;
        costImage.sprite = GameController.instance.healthNumbers[cardData.cost];
        damageImage.sprite = GameController.instance.damageNumbers[cardData.damage];

    }
}
