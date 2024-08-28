using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] AudioSource music;
    [SerializeField] float defaultVolume = 10f;
    [SerializeField] AudioSource UISource;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip drawCard;
    [SerializeField] AudioClip attackHit;
    [SerializeField] AudioClip shieldBlock;

    void Awake()
    {
        // Singleton pattern to ensure only one instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            music.volume = defaultVolume / 100f;
            UISource.volume = defaultVolume / 100f;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public float GetMusicVolume()
    {
        return music.volume;
    }

    // Method to change the volume, which can be called from your UI slider
    public void SetVolume(float volume)
    {
        music.volume = volume / 100f;
        UISource.volume = volume / 100f;
    }

    public void PlayButtonClick()
    {
        UISource.PlayOneShot(buttonClick);
    }

    public void PlayDrawCard()
    {
        UISource.PlayOneShot(drawCard);
    }

    public void PlayAttackHit()
    {
        UISource.PlayOneShot(attackHit);
    }

    public void PlayShieldBlock()
    {
        UISource.PlayOneShot(shieldBlock);
    }

    public IEnumerator WaitAndPlaySFX(float sec, string s){
        yield return new WaitForSeconds(sec);
        if (s.Equals("shield")){
            PlayShieldBlock();
        }
        if (s.Equals("attack")){
            PlayAttackHit();
        }
    }
}
