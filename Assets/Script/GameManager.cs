using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera[] cameras = null;
    [SerializeField] private Character character = null;
    [SerializeField] private GameObject PauseMenu = null;
    [SerializeField] private Text textDefaite = null;
    [SerializeField] private Text textVictoire = null;
    [SerializeField] private Canvas HUD = null;
    private bool Defaite = false;
    private bool Win = false;
    private float timereturn = 0f;
    private Camera ActivCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        cameras = FindObjectsOfType<Camera>();

        foreach (Camera cam in cameras)
        {
            Vector3 check = cam.WorldToViewportPoint(character.transform.position);

            if (check.x < 0f || check.x > 1f || check.y < 0f || check.y > 1f)
            {
                cam.gameObject.SetActive(false);
            }
            else
            {
                ActivCamera = cam;
                character.ActivCam = ActivCamera;
                ActivCamera.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ActivCamera != null)
        {
            Vector3 check = ActivCamera.WorldToViewportPoint(character.transform.position);

            if (check.x < 0f || check.x > 1f || check.y < 0f || check.y > 1f)
            {
                ActivCamera.gameObject.SetActive(false);
                ActivCamera = null;
            }
        }

        if (ActivCamera == null)
        {
            foreach (Camera cam in cameras)
            {
                Vector3 check = cam.WorldToViewportPoint(character.transform.position);
                if (check.x >= 0f && check.x <= 1f && check.y >= 0f && check.y <= 1f)
                {
                    ActivCamera = cam;
                    character.ActivCam = ActivCamera;
                    ActivCamera.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (Input.GetButtonDown("Pause"))
        {
            PauseGame(true);
        }

        if (character.Life <= 0 && !Defaite)
        {
            Defaite = true;
            Time.timeScale = 0f;
            textDefaite.gameObject.SetActive(true);
        }
        if (Defaite)
        {
            timereturn += Time.unscaledDeltaTime;
            if (timereturn > 2f)
            {
                Defaite = false;
                timereturn = 0f;
                Time.timeScale = 1f;
                textDefaite.gameObject.SetActive(false);
                Reset();
            }
        }

        if (character.Win && !Win)
        {
            Win = true;
            Time.timeScale = 0f;
            textVictoire.gameObject.SetActive(true);
        }
        if (Win)
        {
            timereturn += Time.unscaledDeltaTime;
            if (timereturn > 2f)
            {
                Win = false;
                timereturn = 0f;
                Time.timeScale = 1f;
                textVictoire.gameObject.SetActive(false);
                QuitGame("Menu");
            }
        }
    }

    public void PauseGame(bool isPause)
    {
        Time.timeScale = isPause ? 0f : 1f;
        PauseMenu.SetActive(isPause);
        HUD.gameObject.SetActive(!isPause);
    }

    public void Reset()
    {
        PauseGame(false);
        QuitGame("SampleScene");
    }

    public void QuitGame(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}

