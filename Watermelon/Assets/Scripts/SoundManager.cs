using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SFX
{
    LevelUP,
    Next = 3,
    Touch,
    Button,
    GameOver
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance { get { return instance; } }

    [SerializeField] private AudioSource[] sfxPlayer;
    [SerializeField] private AudioClip[] sfxClip;

    private int sfxIndex;
    private const int Level_UP_Clip_Count = 3;

    



    private void Awake()
    {
        if (instance == null) instance = this; 
    }

    public void PlaySFX(SFX sfx)
    {
        switch(sfx)
        {
            case SFX.LevelUP:
                sfxPlayer[sfxIndex].clip = sfxClip[Random.Range(0,Level_UP_Clip_Count)];
                break;

            case SFX.Next:
                sfxPlayer[sfxIndex].clip = sfxClip[(int)SFX.Next]; 
                break;

            case SFX.Touch:
                sfxPlayer[sfxIndex].clip = sfxClip[(int)SFX.Touch];
                break;

            case SFX.Button:
                sfxPlayer[sfxIndex].clip = sfxClip[(int)SFX.Button];
                break;

            case SFX.GameOver:
                sfxPlayer[sfxIndex].clip = sfxClip[(int)SFX.GameOver];
                break;

            
        }
        sfxPlayer[sfxIndex].Play();

        //나머지 연산
        sfxIndex = (sfxIndex+1) % sfxPlayer.Length;

    }
}
