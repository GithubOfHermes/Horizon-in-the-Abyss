using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // ■ BGM 구분용 열거형
    public enum BGMType { Game_BGM, Fight_BGM, Boss_BGM, Player_Die_BGM, Clear_BGM, Shop_BGM }

    [Header("BGM Clips")]
    public AudioClip GameBGM;
    public AudioClip FightBGM;
    public AudioClip BossBGM;
    public AudioClip PlayerDieBGM;
    public AudioClip ClearBGM;
    public AudioClip ShopBGM;

    [Header("페이드/볼륨 설정")]
    public float fadeDuration = 1.0f;
    // ========== 수정: BGM 최종 재생 볼륨 값 통일 ============
    public float targetVolume = 0.1f;

    private AudioSource currentSource;
    private AudioSource nextSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // ========== 수정: AudioSource 2개 할당 및 초기 볼륨 설정 ============
            currentSource = gameObject.AddComponent<AudioSource>();
            nextSource    = gameObject.AddComponent<AudioSource>();
            currentSource.loop = true;
            nextSource.loop    = true;
            currentSource.volume = targetVolume;   // ========== 수정: 초기 currentSource 볼륨 ============
            nextSource.volume    = 0f;             // ========== 수정: 초기 nextSource 볼륨 ============
            // ========== 수정: 씬 로드 콜백 등록 ============
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
         void OnDestroy()
     {
        // ========== 수정: 이벤트 해제 ==========
         SceneManager.sceneLoaded -= OnSceneLoaded;
     }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentSource == null || nextSource == null)
        {
            currentSource = gameObject.AddComponent<AudioSource>();
            nextSource    = gameObject.AddComponent<AudioSource>();
            currentSource.loop = nextSource.loop = true;
            currentSource.volume = targetVolume;
            nextSource.volume    = 0f;
        }
        if (scene.name == "Game_Scene")
            PlayBGM(BGMType.Game_BGM);
    }

    public void PlayBGM(BGMType type)
    {
        // ========== 수정: AudioSource 참조 유효성 검사 및 재초기화 ==========
        if (currentSource == null || nextSource == null)
        {
            currentSource = gameObject.AddComponent<AudioSource>();
            nextSource = gameObject.AddComponent<AudioSource>();
            currentSource.loop = nextSource.loop = true;
            currentSource.volume = targetVolume;
            nextSource.volume = 0f;
        }

        AudioClip clip = GetClip(type);
        if (clip == null) return;
        if (currentSource.clip == clip) return;
        StartCoroutine(CrossFade(clip));
    }

    private AudioClip GetClip(BGMType type)
    {
        switch (type)
        {
            case BGMType.Game_BGM:      return GameBGM;
            case BGMType.Fight_BGM:     return FightBGM;
            case BGMType.Boss_BGM:      return BossBGM;
            case BGMType.Player_Die_BGM:return PlayerDieBGM;
            case BGMType.Clear_BGM:     return ClearBGM;
            case BGMType.Shop_BGM:      return ShopBGM;
            default:                    return null;
        }
    }

    private IEnumerator CrossFade(AudioClip newClip)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;  // ========== 수정: 페이드 시작 시 nextSource 볼륨 초기화 ============
        nextSource.Play();

        float time = 0f;
        float startVol = currentSource.volume;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fadeDuration;
            // ========== 수정: currentSource는 기존 볼륨 → 0으로 페이드아웃 ============
            currentSource.volume = Mathf.Lerp(startVol, 0f, t);
            // ========== 수정: nextSource는 0 → targetVolume으로 페이드인 ============
            nextSource.volume    = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        currentSource.Stop();
        // ========== 수정: 페이드 후 볼륨 확실히 설정 ============
        nextSource.volume    = targetVolume;
        // 스왑
        var tmp = currentSource;
        currentSource = nextSource;
        nextSource    = tmp;
    }
}
