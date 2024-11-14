using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Sedol : MonoBehaviour
{
    const float Create_Position_Y = 8f;

    Camera mainCamera;

    [Header("마우스 따라오는 감도 (높은 수록 빠르게 따라옴)")]
    [Range(0, 1)]
    [SerializeField] float mouseSensitivity = 0.5f;

    [Header("합쳐지는 속도 (높은 수록 빠르게 흡수)")]
    [Range(0, 1)]
    [SerializeField] float mergeSpeed = 0.5f;

    [Header("상대가 나에게 오는 시간")]
    [SerializeField] private float absorbTime = 0.2f;

    [Header("애니메이션 재생 시간")]
    [SerializeField] private float playAniamtion = 0.3f;

    private WaitForSeconds waitAbsorb;
    private WaitForSeconds waitAnimation;


    public bool isDrag;
    public Rigidbody2D circleRigidbody;

    public int level;
    public int max = 0;
    Animator animator;

    public bool isMerge;
    CircleCollider2D circlecollider;

    public ParticleSystem effect;

    private float stayTime;
    SpriteRenderer spriteRenderer;

    private WaitForSeconds waitTouch;
    private bool isTouch;

    private void Awake()
    {
        mainCamera = Camera.main;
        circleRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circlecollider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        waitAbsorb = new WaitForSeconds(absorbTime);
        waitAnimation = new WaitForSeconds(playAniamtion);
        waitTouch = new WaitForSeconds(0.2f);
    }
    // 1. 마우스 커서를 따리다니는 오브젝트 => 휴대폰 빌드에서 손가락 터치랑 대응이 된다.
    //
    // 2. 

    private void OnEnable()
    {
        animator.SetInteger("Level", level);
    }
    // Update is called once per frame
    void Update()
    {
        if (isDrag)
        {
            // Camera.main : 카메라 중 메인카메라를 태그를 달고 있는 것
            // ScreenToWorldPoint : 유니티 특성 상 두 공간이 서로 격리되어 있기 때문에 스크린 포인트의 좌표를 월드 자표계로 변경해주세요.
            // ScreenPoint : 화면 상의 공간 (UI공간)
            // WorldPoint : 실제 게임이 구축되는 공간 (게임 오브젝트)
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // 경계선 정의
            float borderLeft = -4.2f + transform.localScale.x / 2;
            float borderRight = 4.2f - transform.localScale.x / 2;


            mousePosition.y = Create_Position_Y;
            mousePosition.z = 0;        // 메인 카메라의 z값이 -10이기 때문에 화면에 보이지 않는다
                                        //transform.position = mousePosition;
            if (mousePosition.x < borderLeft)
            {
                mousePosition.x = borderLeft;
            }
            else if (mousePosition.x > borderRight)
            {
                mousePosition.x = borderRight;
                //transform.position = new Vector3(borderRight, mousePosition.y, mousePosition.z);
            }

            //마우스를 부드럽게 따라오도록
            // Lerp 두 지점간 선형보간
            // Lerp(a, b, t) : a지점, b지점, 시간(0~1) 1에 가까워질수록 빠름, 0과 1 두값은 의미가 없는 결과가 나온다.
            transform.position = Vector3.Lerp(transform.position, mousePosition, mouseSensitivity);
        }
    }

    // 물리 충돌 중일 때 호출되는 유니티 이벤트 함수
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Sedol"))
        {
            // 상대방의 컴포넌트를 가져온다.
            Sedol other = collision.gameObject.GetComponent<Sedol>();

            // 합체 중에 다른 써클의 개입 금지
            // 합체는 1:1로 이루어져야 한다.
            // 우리의 현재 상황에서는 맥스레벨이 7이다.
            if (level.Equals(other.level) && !isMerge && !other.isMerge && level < 7)
            {
                // 합체를 내 쪽으로 흡수하는 표현하기위해 위치가 필요하다.
                // 나와 상대방의 위치를 가져온다.
                float x = transform.position.x;
                float y = transform.position.y;

                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 만약 y값이 다르다면 위에 있는 친구가 아래로 떨어지며 흡수
                // 만약 y값이 다르다면 왼쪽이 나에게 흡수

                /*if(y > otherY)
                {
                    {
                        x = otherX;
                        y = otherY;
                    }
                }
                else if(otherY > y)
                {
                    otherX = x;
                    otherY = y;
                }
                else
                {
                    if(x > otherX)
                    {
                        otherX = x;
                    }
                    if(otherX > x)
                    {
                        x = otherX;
                    }
                }  */

                if (y < otherY || (y.Equals(otherY) && x > otherX))
                {
                    // 상대방이 나에게 흡수
                    other.Absorb(transform.position);
                    // 나는 레벨업
                    LevelUp();
                }
            }
        }
    }

    // 상대가 나에게 흡수되는 함수
    public void Absorb(Vector3 target)
    {
        isMerge = true;
        circleRigidbody.simulated = false;
        circlecollider.enabled = false;
        if(GameManager.Instance.isGameOver) PlayEffect();
        StartCoroutine(Move(target));
    }

    // 나의 위치로 이동하는 코루틴
    private IEnumerator Move(Vector3 target)
    {
        int frame = 0;
        while (frame < 20)
        {
            frame++;
            transform.position = Vector3.Lerp(transform.position, target, mergeSpeed);
            yield return null; // 한프레임을 쉰다
        }
        isMerge = false;
        gameObject.SetActive(false);

        // 레벨에 비례하여 점수 부여
        // 더 높은 레벨 생성시 극적인 효과를 주고싶었다
        GameManager.Instance.score += (int)Mathf.Pow(2, level);

        UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

    }

    /// <summary>
    /// 레벨업 함수 
    /// </summary>
    private void LevelUp()
    {
        isMerge = true;

        // 합치는 동안 속력괴 회전을 0으로 해야한다.
        circleRigidbody.velocity = Vector2.zero;
        circleRigidbody.angularVelocity = 0;

        StartCoroutine(NextLevel());

    }


    // 다음 레벨 코루틴
    private IEnumerator NextLevel()
    {
        // 상대가 나에게 흡수되는 시간동안 기다림
        yield return waitAbsorb;

        animator.SetInteger("Level", level + 1);
        PlayEffect();
        SoundManager.Instance.PlaySFX(SFX.LevelUP);
        // 애니메이션 재싱시간 또한 기다린다.
        yield return waitAnimation;

        level = level + 1;
        /*if(level > GameManager.Instance.maxLevel)
        {
            GameManager.Instance.maxLevel = level;
        }*/

        // 매니저의 맥스레벨에 올바른 값을 ㅎ라당
        // Math.Max(a,b); a,b중 높은 값 리턴
        GameManager.Instance.maxLevel = Mathf.Max(level, GameManager.Instance.maxLevel);

        isMerge = false;
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        circleRigidbody.simulated = true;
    }

    //이펙트 재생 함수
    private void PlayEffect()
    {
        // 이펙트의 포지션을 나의 위치로
        effect.transform.position = transform.position;
        // 이펙트의 크기를 나의 스케일로
        effect.transform.localScale = transform.localScale;
        // 이펙트 재생
        effect.Play();
    }

    //점선에 닿고 2초가 경과하면 빨간색으로 경고, 

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            stayTime += Time.deltaTime;
            if (stayTime >= 5f)
            {
                GameManager.Instance.Gameover();
                
            }
            else if(stayTime > 2f)
            {
                spriteRenderer.color = Color.red;
                
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            spriteRenderer.color = Color.white;
            stayTime = 0;
        }
    }

    private void OnCollisonEnter2D(Collision2D collision)
    {
        StartCoroutine(TouchSound());  
    }

    private IEnumerator TouchSound()
    {
        if (isTouch) yield break;   // yield break 코루틴 탈출

        isTouch = true;
        SoundManager.Instance.PlaySFX(SFX.Touch);

        yield return waitTouch;
        isTouch = false;

    }







    // id,pw 입력
    // 로그인 버튼 클릭
    // 서버에서 보냄
    // 서버에서 검사함
    // 서버에서 ok or no sign 준다
    // 클라에서 싸인을 보고 에러메시지 또는 로그인 진행
}
