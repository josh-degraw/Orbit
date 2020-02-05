﻿using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBarCircle : MonoBehaviour
{
    [Header("Title Setting")]
    public string Title;

    public Color TitleColor;
    public Font TitleFont;
    private Text txtTitle;

    [Header("Bar Setting")]
    public Color BarColor;

    public Color BarBackGroundColor;
    public Color MaskColor;
    public Sprite BarBackGroundSprite;

    [Range(1f, 100f)]
    public int Alert = 20;

    public Color BarAlertColor;
    private Image bar, barBackground, Mask;

    [Header("Sound Alert")]
    public AudioClip sound;

    public bool repeat = false;
    public float RepearRate = 1f;
    private AudioSource audiosource;

    private float nextPlay;
    private float barValue;

    public float BarValue {
        get { return barValue; }

        set {
            value = Mathf.Clamp(value, 0, 100);
            barValue = value;
            UpdateValue(barValue);
        }
    }

    private void Awake()
    {
        txtTitle = transform.Find("Text").GetComponent<Text>();
        barBackground = transform.Find("BarBackgroundCircle").GetComponent<Image>();
        bar = transform.Find("BarCircle").GetComponent<Image>();
        audiosource = GetComponent<AudioSource>();
        Mask = transform.Find("Mask").GetComponent<Image>();
    }

    private void Start()
    {
        txtTitle.text = Title;
        txtTitle.color = TitleColor;
        txtTitle.font = TitleFont;

        bar.color = BarColor;
        Mask.color = MaskColor;
        barBackground.color = BarBackGroundColor;
        barBackground.sprite = BarBackGroundSprite;

        UpdateValue(barValue);
    }

    private void UpdateValue(float val)
    {
        bar.fillAmount = -(val / 100) + 1f;

        txtTitle.text = Title + " " + val + "%";

        if (Alert >= val)
        {
            barBackground.color = BarAlertColor;
        }
        else
        {
            barBackground.color = BarBackGroundColor;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateValue(50);
            txtTitle.color = TitleColor;
            txtTitle.font = TitleFont;
            Mask.color = MaskColor;
            bar.color = BarColor;
            barBackground.color = BarBackGroundColor;
            barBackground.sprite = BarBackGroundSprite;
        }
        else
        {
            if (Alert >= barValue && Time.time > nextPlay)
            {
                nextPlay = Time.time + RepearRate;
                audiosource.PlayOneShot(sound);
            }
        }
    }
}