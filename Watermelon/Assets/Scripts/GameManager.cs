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




    public Sedol lastSedol;

    [SerializeField] private GameObject sedolPrefab;
    [SerializeField] private Transform sedolGroup;

    private WaitForSeconds waitNextCircle;

    public int maxLevel;


    private void Awake()
    {
        if (instance == null) instance = this;

        // 프레임 제한(모바일의 경우)
        Application.targetFrameRate = 60;
        waitNextCircle = new WaitForSeconds(1.5f);
    }
    // Start is called before the first frame update
    void Start()
    {
        NextSedol();
    }

    private void NextSedol()
    {
        Sedol newSedol = GetSedol();
        lastSedol = newSedol;

        lastSedol.level = Random.Range(0, maxLevel);
        lastSedol.gameObject.SetActive(true);

        // 코루틴의 호출 형태
        StartCoroutine(WaitLastSedolNull());
        //StartCoroutine("WaitLastSedolNull");
    }

    private Sedol GetSedol()
    {
        // instantiate 게임 오브젝트를 생성하는 코드
        GameObject temp = Instantiate(sedolPrefab, sedolGroup);
        Sedol sedol = temp.GetComponent<Sedol>();
        return sedol;
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
}
