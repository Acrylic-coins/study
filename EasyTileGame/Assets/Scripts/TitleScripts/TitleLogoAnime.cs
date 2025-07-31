using UnityEngine;

public class TitleLogoAnime : MonoBehaviour
{
    [SerializeField] private GameObject heartbeatObj;
    [SerializeField] private GameObject carnivalObj;

    private Animator heartbeatAnimator;
    private Animator carnivalAnimator;

    private bool isFalseHeartbeatObj = false;   // heartbeatObj를 비활성화할지 결정하는 변수
    private bool isFalsecarnivalObj = false;    // carnivalObj를 비활성화할지 결정하는 변수

    private bool isLogoEffect = true; // 로고 이펙트를 위한 함수을 생략할 수 있는 변수 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // 캐싱
        heartbeatAnimator = heartbeatObj.GetComponent<Animator>();
        carnivalAnimator = carnivalObj.GetComponent<Animator>();
        // 두 곳에 자기 주소 보내기
        heartbeatObj.GetComponent<TitleHeartbeat>().InitAnimeScript(this);
        carnivalObj.GetComponent<TitleCarnival>().InitAnimeScript(this);
    }

	private void Start()
	{
        isLogoEffect = true;
        heartbeatObj.SetActive(true);
	}

    // 타이틀로고 애니메이션 특정 프레임에서 확률적으로 타이틀 로고에 효과를 준다.
    public void ThinkCGHeartbeatLogo()
    {
        if (!isLogoEffect) { return;  }

        int r = Random.Range(0, 10);
        // 20% 확률로 carnival 문구를 활성화하고, heartbeat 문구를 지운다.
        if (r > 7)
        {
            heartbeatAnimator.SetTrigger("IsDisappear");
            carnivalObj.SetActive(true);
        }
        // 30% 확률로 글리치 효과를 일으킨다.
        else if (r > 4)
        {
			heartbeatAnimator.SetTrigger("IsGlitch");
		}
        // 50% 확률로 그대로 간다.
        else
        {
            return;
        }
    }
    // 타이틀 로고 중 heartbeat를 비활성화시킨다.
    public void ActiveFalseHeartbeatLogo()
    {
        if (!isLogoEffect) { return; }

        heartbeatObj.SetActive(false);
        if (!carnivalObj.activeSelf)
        {
            carnivalObj.SetActive(true);
        }
    }

    public void ThinkCGCarnivalLogo()
    {
        if (!isLogoEffect) { return; }

        int r = Random.Range(0, 10);
        // 20% 확률로 heartbeat 문구를 활성화하고, carnival 문구를 지운다.
        if (r > 7)
        {
            carnivalAnimator.SetTrigger("IsDisappear");
            heartbeatObj.SetActive(true);
        }
        // 30% 확률로 글리치 효과를 일으킨다.
        else if (r > 4)
        {
            carnivalAnimator.SetTrigger("IsGlitch");
        }
        // 50% 확률로 그대로 간다.
        else
        {
            return;
        }
    }
    // 타이틀 로고 중 carnival을 비활성화시킨다.
    public void ActiveFalseCarnivalLogo()
    {
        if (!isLogoEffect) { return; }

        carnivalObj.SetActive(false);
        if (!heartbeatObj.activeSelf)
        {
            heartbeatObj.SetActive(true);
        }
    }

    public void DisappearTitleLogo()
    {
        isLogoEffect = false;

		carnivalAnimator.SetTrigger("IsDisappear");
        heartbeatAnimator.SetTrigger("IsDisappear");

        Invoke("SetActiveFalseLogo", 3f);
    }
    private void SetActiveFalseLogo()
    {
        carnivalObj.SetActive(false);
        heartbeatObj.SetActive(false);
    }
}
