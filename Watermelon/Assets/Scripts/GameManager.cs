using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 디자인패턴
// 싱글톤 : 게임 개발에서 많이 사용되는 패턴, 유일한 객체로 만들며, 어디서나 접근이 용이하도록 한다.
// static : 유일하게 만들고 , 항상 메모리에 태운다.
// 싱글톤은 여러 스크립트에서 접근해야만 하는 매니저와 같은 클래스에만 사용하세요.

public class GameManager : MonoBehaviour
{
    // 싱글톤 작성 방법
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }

    [Header("상대가 나에게 오는 시간")]
    [SerializeField] private float absorbTime = 0.2f;

    private WaitForSeconds inOrderTime = new WaitForSeconds(0.5f);

    public Sedol lastSedol;

    [SerializeField] private GameObject sedolPrefab;
    [SerializeField] private Transform sedolGroup;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectGroup;

    private WaitForSeconds waitNextCircle;

    public int maxLevel;
    public int score;

    public ParticleSystem effect;

    public bool isGameOver;

    // 오브젝트 풀링 : 재사용 가능한 게임 오브젝트를 재활용하는 것
    public List<Sedol> sedolList = new List<Sedol>();
    public List<ParticleSystem> effectList = new List<ParticleSystem>();

    public int poolSize = 10;
    public int poolIndex;



    private void Awake()
    {
        if (instance == null) instance = this;

        // 프레임 제한(모바일의 경우)
        Application.targetFrameRate = 60;
        waitNextCircle = new WaitForSeconds(1.5f);

        // 미리 풀 사이즈만큼 세돌 생성
        for(int i = 0; i < poolSize; i++)
        {
            CreateSedol();
        }
    }
    // Start is called before the first frame update
    

    public void NextSedol()
    {
        if (!isGameOver)
        {
            lastSedol = GetSedol();

            lastSedol.level = Random.Range(0, maxLevel);
            lastSedol.gameObject.SetActive(true);

            SoundManager.Instance.PlaySFX(SFX.Next);

            // 코루틴의 호출 형태
            StartCoroutine(WaitLastSedolNull());
            //StartCoroutine("WaitLastSedolNull");
        }
    }

    //써클 생성 함수
    private Sedol CreateSedol()
    {
        ParticleSystem effect = Instantiate(effectPrefab, effectGroup).GetComponent<ParticleSystem>();
        effect.name = "Effect" + effectList.Count;
        effectList.Add(effect);

        // instantiate 게임 오브젝트를 생성하는 코드
        GameObject temp = Instantiate(sedolPrefab, sedolGroup);
        Sedol sedol = temp.GetComponent<Sedol>();
        sedol.name = "Sedol" + sedolList.Count;
        sedol.effect = effect;
        sedolList.Add(sedol);

        return sedol;
    }

    //써클을 가져오는 함수
    private Sedol GetSedol()
    {
        for(int i = 0 ; i<sedolList.Count ; i++)
        {
            // 비활성화인 오브젝트를 찾음
            if (!sedolList[i].gameObject.activeSelf)
            {
                return sedolList[i];
            }
        }
        // 비활성화인 오브젝트가 없다면 새로 생성
        return CreateSedol();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TouchDown()
    {   if( lastSedol == null)
        {
            return;
        }
        lastSedol.Drag();
    }


    public void TouchUp()
    {
        if (lastSedol == null)
        {
            return;
        }
        lastSedol.Drop();
        lastSedol = null;
    }

    // 코루틴 : 정지와 시작을 제어하여 게임 개발을 원할하게 돌아가도록 도와주는 서브루틴이다.
    // 몇 초 기다리는 데 자주 쓰기도 함
    IEnumerator WaitLastSedolNull()
    {
        // 우리가 제어를 하고 있을 때
        while (lastSedol != null)
        {
            yield return null; // 코루틴에서는 return 대신 yield return null;해줘야한다
        }
        yield return waitNextCircle;
        NextSedol();
    }

    public void Gameover()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("게임오버!");

        // 하이어라키에 활성화된 써클 오브젝트 찾기
        // 1. Sedol[] sedols = FindObjectsOfType<Sedol>(); --> 메모리 과다사용

        // 2.
        Sedol[] sedols = sedolGroup.GetComponentsInChildren<Sedol>();
        for (int i = 0; i < sedols.Length; i++)
        {
            sedols[i].circleRigidbody.simulated=false;
        }
        
        // !gameObject.activeSelf 비활성

        StartCoroutine(InOrder(sedols));
    }

    private IEnumerator InOrder(Sedol[] sedols)
    {
        for (int i = 0; i < sedols.Length; i++)
        {
            // 게임오버시에는 움직이는 효과가 필요없으므로 나의 위치를 넘긴다.
            sedols[i].Absorb(sedols[i].transform.position);
            SoundManager.Instance.PlaySFX(SFX.Next);
            yield return inOrderTime;
        }
        // 최고 기록 저장
        int bestscore = Mathf.Max(score, PlayerPrefs.GetInt("BestScore"));
        PlayerPrefs.SetInt("BestScore", bestscore);

        UIManager.Instance.gameoverUI.SetActive(true);
        UIManager.Instance.tempGameoverScore.text = "점수: " + score.ToString();

        SoundManager.Instance.PlaySFX(SFX.GameOver);
    }

}
