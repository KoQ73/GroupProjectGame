using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnButtonClick()
    {
        audioManager.PlayButtonClick();
    }

    public void OnCardDraw()
    {
        audioManager.PlayDrawCard();
    }
}
