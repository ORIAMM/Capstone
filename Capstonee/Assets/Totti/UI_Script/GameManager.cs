using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    }
    private void Start()
    {
        PlayMusic();
    }
    public void OnEscape()
    {
        if (Invoker.count > 0) Close();
        else Open(mainPanel);
    }
    public static void PauseGame(bool isPause)
    {
        IsPaused = isPause;
        Time.timeScale = isPause ? 0 : 1;
    }
    public void PlayMusic()
    {
        SoundManager.instance.StopAllMusic();
        SoundManager.instance.StopAllSFX();
        SoundManager.instance.LoadPref();
        SoundManager.instance.PlayMusic(ThemeSong);
        //string sceneName = SceneManager.GetActiveScene().name;
        //var setting = settings.Find(x => x.sceneName == sceneName);
        //if (setting != null) SoundManager.instance.PlayMusic(setting.sound.name);
    }
    public static void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void QuitGame() => Application.Quit();
    public static void Open(GameObject obj) => Invoker.ExecuteCommand(new OpenCommand(obj));
    public static void Close() => Invoker.UndoCommand();

    [System.Serializable]
    public class MenuSettings
    {
        public string sceneName;

        [Header("Music Settings")]
        public SoundManager.Sound sound;
    }
}
