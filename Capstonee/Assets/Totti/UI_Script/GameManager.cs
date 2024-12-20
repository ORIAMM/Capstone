using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ICommand
{
    public void Execute();
    public void Undo();
}
public class OpenCommand : ICommand
{
    private List<GameObject> objects;
    public OpenCommand(List<GameObject> objects)
    {
        this.objects = objects;
    }
    public OpenCommand(GameObject objects)
    {
        this.objects = new() { objects };
    }
    public void Execute()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }
    public void Undo()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}
public static class Invoker
{
    private static Stack<ICommand> commands = new();
    public static int count =>  commands.Count;
    public static void ExecuteCommand(ICommand command)
    {
        commands.Push(command);
        command.Execute();
    }
    public static void UndoCommand()
    {
        if(count > 0)
        {
            commands.Pop().Undo();
        }
    }
}
public class GameManager : MonoBehaviour
{
    //public List<MenuSettings> settings;
    
    public string ThemeSong = "MainMenuTheme";
    public GameObject mainPanel;
    
    public static bool IsPaused;


    [Header("SoundClip")]
    [SerializeField] private SoundCLIP BGM;
    [SerializeField] private SoundCLIP Click;
    [SerializeField] private SoundCLIP Play;

/*    [Header("Settings")]
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Slider MasterSlider;*/

    [SerializeField] private AudioClip clip;
    [SerializeField] private InputAction escape;


    private void OnEnable()
    {
        escape.Enable();
    }
    private void OnDisable()
    {
        escape.Disable();
    }
    public void Awake()
    {
        escape.performed += (val) => OnEscape();
        Application.targetFrameRate = 30;
        this.enabled = true;

/*        if (MasterSlider)
        {
            MasterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
            MasterSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.Master, value));
        }
        if (SFXSlider)
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
            SFXSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.SFX, value));
        }
        if (BGMSlider)
        {
            BGMSlider.value = PlayerPrefs.GetFloat("BGM", 1f);
            BGMSlider.onValueChanged.AddListener(value => SoundCEO.instance.SetVolume(AudioCategorys.BGM, value));
        }*/
        
    }
    private void Start()
    {
        PlayMusic();

    }
    public void OnEscape()
    {
        if (Invoker.count > 0)
        {
            CloseWithSound();
            PauseGame(false);
        }
        else
        {
            OpenWithSound(mainPanel);
            PauseGame(true);
        }

    }
    public static void PauseGame(bool isPause)
    {
        IsPaused = isPause;
        Time.timeScale = isPause ? 0 : 1;
        //if(isPause == true)SoundCEO.instance.StopAllSound();
        //else SoundCEO.instance.ResumeAllSound();
    }
    public void PlayMusic()
    {
        //SoundManager.instance.StopAllMusic();
        //SoundManager.instance.StopAllSFX();
        //SoundManager.instance.LoadPref();
        //SoundManager.instance.PlayMusic(ThemeSong);

        SoundCEO.instance.StopAllSound();
        SoundCEO.instance.PlaySound(BGM);


        //string sceneName = SceneManager.GetActiveScene().name;
        //var setting = settings.Find(x => x.sceneName == sceneName);
        //if (setting != null) SoundManager.instance.PlayMusic(setting.sound.name);
    }
    public static void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void QuitGame() => Application.Quit();
    public static void Open(GameObject obj) => Invoker.ExecuteCommand(new OpenCommand(obj));
    public static void Close() => Invoker.UndoCommand();

    public void OpenNewGame()
    {
        //SoundCEO.instance.PlaySound(Play);
        SoundCEO.instance.StopAllSound();
    }

    public void OpenWithSound(GameObject obj)
    {
        //if(clip != null)SoundManager.instance.PlaySFX(clip.name);
        SoundCEO.instance.PlaySound(Click);
        Open(obj);
    }

    public void CloseWithSound()
    {
        //if (clip != null) SoundManager.instance.PlaySFX(clip.name);
        SoundCEO.instance.PlaySound(Click);
        PauseGame(false);
        Close();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    [System.Serializable]
    public class MenuSettings
    {
        public string sceneName;

        [Header("Music Settings")]
        public SoundManager.Sound sound;
    }
}
