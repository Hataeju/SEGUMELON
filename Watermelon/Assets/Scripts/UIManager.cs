using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance { get { return instance; } }

    public TextMeshProUGUI tmpScore;
    [SerializeField] private TextMeshProUGUI tmpBestScore;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
         
        /*if(!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        }*/

        tmpBestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        //합쳐질때
        //tmpScore.text = GameManager.Instance.score.ToString();
    }
}
