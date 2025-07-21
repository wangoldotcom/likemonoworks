using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class TitleSceneManager : MonoBehaviour
{
    public VideoPlayer startVideoPlayer;
    public VideoPlayer loopVideoPlayer;
    public VideoPlayer endVideoPlayer;
    public TextMeshProUGUI loadingText;

    public string[] additionalResourceFolders = new string[] { "Resources/Prefabs", "Resources/Textures", "Resources/Ui", "Effects" }; // 타이틀 씬에서 추가로 로드할 리소스 경로

    private bool isLoopPlaying = false;
    private bool isLoopVideoPrepared = false;
    private bool isEndVideoPrepared = false;
    private bool isAdditionalResourcesLoaded = false;

    void Start()
    {
        if (startVideoPlayer != null)
        {
            startVideoPlayer.loopPointReached += OnStartVideoEnd;
            startVideoPlayer.prepareCompleted += StartVideoPrepared;
            startVideoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("Start VideoPlayer is not assigned in the inspector.");
        }

        if (loopVideoPlayer != null)
        {
            loopVideoPlayer.loopPointReached += OnLoopVideoLoop;
            loopVideoPlayer.prepareCompleted += LoopVideoPrepared;
            loopVideoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("Loop VideoPlayer is not assigned in the inspector.");
        }

        if (endVideoPlayer != null)
        {
            endVideoPlayer.loopPointReached += OnEndVideoEnd;
            endVideoPlayer.prepareCompleted += EndVideoPrepared;
            endVideoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("End VideoPlayer is not assigned in the inspector.");
        }

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("LoadingText is not assigned in the inspector.");
        }

        // 타이틀 씬에서 추가 리소스를 로드
        StartCoroutine(PreloadAdditionalResources());
    }

    private void StartVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Start video prepared.");
        startVideoPlayer.Play();
    }

    private void LoopVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Loop video prepared.");
        isLoopVideoPrepared = true;
    }

    private void EndVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("End video prepared.");
        isEndVideoPrepared = true;
    }

    void Update()
    {
        if (isLoopPlaying && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 또는 터치
        {
            loopVideoPlayer.Stop();
            PlayEndVideo();
        }
    }

    private void OnStartVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Start video ended.");
        startVideoPlayer.gameObject.SetActive(false);
        if (isLoopVideoPrepared)
        {
            loopVideoPlayer.gameObject.SetActive(true);
            loopVideoPlayer.Play();
            isLoopPlaying = true;
        }
    }

    private void OnLoopVideoLoop(VideoPlayer vp)
    {
        if (isLoopPlaying)
        {
            loopVideoPlayer.Play(); // 루프 영상 반복 재생
        }
    }

    private void PlayEndVideo()
    {
        Debug.Log("Playing end video.");
        isLoopPlaying = false;
        loopVideoPlayer.gameObject.SetActive(false);
        if (isEndVideoPrepared && isAdditionalResourcesLoaded)
        {
            endVideoPlayer.gameObject.SetActive(true);
            endVideoPlayer.Play();
        }
    }

    private void OnEndVideoEnd(VideoPlayer vp)
    {
        Debug.Log("End video ended.");
        StartCoroutine(WaitAndLoadMainScene());
    }

    private IEnumerator WaitAndLoadMainScene()
    {
        Debug.Log("Waiting and loading main scene.");
        yield return new WaitForSeconds(1f); // 1초 대기
        StartCoroutine(LoadMainScene());
    }

    private IEnumerator LoadMainScene()
    {
        Debug.Log("Loading main scene.");
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = "Loading... 0%";
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (loadingText != null)
            {
                loadingText.text = "Loading... " + (progress * 100) + "%";
            }
            yield return null;
        }
    }

    private IEnumerator PreloadAdditionalResources()
    {
        List<Object> loadedResources = new List<Object>();

        foreach (string folder in additionalResourceFolders)
        {
            Debug.Log($"Loading resources from folder: {folder}");
            Object[] resources = Resources.LoadAll(folder);
            foreach (Object resource in resources)
            {
                loadedResources.Add(resource);
                yield return null; // 각 리소스 로드 후 한 프레임 대기
            }
        }

        isAdditionalResourcesLoaded = true;
        Debug.Log("All additional resources preloaded");

        if (isEndVideoPrepared && !endVideoPlayer.isPlaying)
        {
            PlayEndVideo();
            yield return new WaitForSeconds(2f);
        }
    }
}
