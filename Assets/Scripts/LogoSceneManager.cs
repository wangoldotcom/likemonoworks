using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LogoSceneManager : MonoBehaviour
{
    public GameObject videoPlayerObject; // VideoPlayer가 붙어있는 오브젝트를 참조합니다.
    private VideoPlayer videoPlayer;
    public string[] resourceFolders = new string[] { "Resources/Sounds", "Resources/Sprites", "Mov" }; // 사전 로딩할 리소스 경로
    private bool isResourcesLoaded = false;
    private bool isVideoEnded = false;

    private void Start()
    {
        StartCoroutine(StartWithDelay());
    }

    private IEnumerator StartWithDelay()
    {
        videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            yield return new WaitForSeconds(1f); // 1초 대기
            videoPlayer.loopPointReached += OnVideoEnd; // 동영상이 끝났을 때 호출될 메서드 등록
            videoPlayer.Play();
            StartCoroutine(PreloadResources());
        }
        else
        {
            Debug.LogError("VideoPlayer component is missing on the specified object.");
            StartCoroutine(LoadTitleScene());
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        isVideoEnded = true;
        if (isResourcesLoaded)
        {
            StartCoroutine(LoadTitleScene());
        }
    }

    private IEnumerator PreloadResources()
    {
        List<Object> loadedResources = new List<Object>();

        foreach (string folder in resourceFolders)
        {
            Object[] resources = Resources.LoadAll(folder);
            foreach (Object resource in resources)
            {
                loadedResources.Add(resource);
                yield return null; // 각 리소스 로드 후 한 프레임 대기
            }
        }

        isResourcesLoaded = true;
        Debug.Log("All resources preloaded");

        if (isVideoEnded)
        {
            StartCoroutine(LoadTitleScene());
        }
    }

    private IEnumerator LoadTitleScene()
    {
        yield return new WaitForSeconds(1f); // 짧은 대기 후 타이틀 씬 로드
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
