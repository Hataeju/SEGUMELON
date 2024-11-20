using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����������
// �̱��� : ���� ���߿��� ���� ���Ǵ� ����, ������ ��ü�� �����, ��𼭳� ������ �����ϵ��� �Ѵ�.
// static : �����ϰ� ����� , �׻� �޸𸮿� �¿��.
// �̱����� ���� ��ũ��Ʈ���� �����ؾ߸� �ϴ� �Ŵ����� ���� Ŭ�������� ����ϼ���.

public class GameManager : MonoBehaviour
{
    // �̱��� �ۼ� ���
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }

    [Header("��밡 ������ ���� �ð�")]
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

    // ������Ʈ Ǯ�� : ���� ������ ���� ������Ʈ�� ��Ȱ���ϴ� ��
    public List<Sedol> sedolList = new List<Sedol>();
    public List<ParticleSystem> effectList = new List<ParticleSystem>();

    public int poolSize = 10;
    public int poolIndex;



    private void Awake()
    {
        if (instance == null) instance = this;

        // ������ ����(������� ���)
        Application.targetFrameRate = 60;
        waitNextCircle = new WaitForSeconds(1.5f);

        // �̸� Ǯ �����ŭ ���� ����
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

            // �ڷ�ƾ�� ȣ�� ����
            StartCoroutine(WaitLastSedolNull());
            //StartCoroutine("WaitLastSedolNull");
        }
    }

    //��Ŭ ���� �Լ�
    private Sedol CreateSedol()
    {
        ParticleSystem effect = Instantiate(effectPrefab, effectGroup).GetComponent<ParticleSystem>();
        effect.name = "Effect" + effectList.Count;
        effectList.Add(effect);

        // instantiate ���� ������Ʈ�� �����ϴ� �ڵ�
        GameObject temp = Instantiate(sedolPrefab, sedolGroup);
        Sedol sedol = temp.GetComponent<Sedol>();
        sedol.name = "Sedol" + sedolList.Count;
        sedol.effect = effect;
        sedolList.Add(sedol);

        return sedol;
    }

    //��Ŭ�� �������� �Լ�
    private Sedol GetSedol()
    {
        for(int i = 0 ; i<sedolList.Count ; i++)
        {
            // ��Ȱ��ȭ�� ������Ʈ�� ã��
            if (!sedolList[i].gameObject.activeSelf)
            {
                return sedolList[i];
            }
        }
        // ��Ȱ��ȭ�� ������Ʈ�� ���ٸ� ���� ����
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

    // �ڷ�ƾ : ������ ������ �����Ͽ� ���� ������ �����ϰ� ���ư����� �����ִ� �����ƾ�̴�.
    // �� �� ��ٸ��� �� ���� ���⵵ ��
    IEnumerator WaitLastSedolNull()
    {
        // �츮�� ��� �ϰ� ���� ��
        while (lastSedol != null)
        {
            yield return null; // �ڷ�ƾ������ return ��� yield return null;������Ѵ�
        }
        yield return waitNextCircle;
        NextSedol();
    }

    public void Gameover()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("���ӿ���!");

        // ���̾��Ű�� Ȱ��ȭ�� ��Ŭ ������Ʈ ã��
        // 1. Sedol[] sedols = FindObjectsOfType<Sedol>(); --> �޸� ���ٻ��

        // 2.
        Sedol[] sedols = sedolGroup.GetComponentsInChildren<Sedol>();
        for (int i = 0; i < sedols.Length; i++)
        {
            sedols[i].circleRigidbody.simulated=false;
        }
        
        // !gameObject.activeSelf ��Ȱ��

        StartCoroutine(InOrder(sedols));
    }

    private IEnumerator InOrder(Sedol[] sedols)
    {
        for (int i = 0; i < sedols.Length; i++)
        {
            // ���ӿ����ÿ��� �����̴� ȿ���� �ʿ�����Ƿ� ���� ��ġ�� �ѱ��.
            sedols[i].Absorb(sedols[i].transform.position);
            SoundManager.Instance.PlaySFX(SFX.Next);
            yield return inOrderTime;
        }
        // �ְ� ��� ����
        int bestscore = Mathf.Max(score, PlayerPrefs.GetInt("BestScore"));
        PlayerPrefs.SetInt("BestScore", bestscore);

        UIManager.Instance.gameoverUI.SetActive(true);
        UIManager.Instance.tempGameoverScore.text = "����: " + score.ToString();

        SoundManager.Instance.PlaySFX(SFX.GameOver);
    }

}
