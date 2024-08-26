using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    private Camera playerCamera;
    //[SerializeField] Transform target;
    // Start is called before the first frame update
    private void Start()
    {
        playerCamera = Camera.main;
    }

    public void UpdateHealthbar(float remaindingHealth, float maxHealth)
    {
        healthSlider.value = remaindingHealth/maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerCamera.transform.rotation;
        //transform.position = target.position;
    }
}
