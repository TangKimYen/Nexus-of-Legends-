using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour
{
    // Nút cho nhạc nền
    public Button musicButtonOn;
    public Button musicButtonOff;

    // Nút cho âm thanh của nhân vật
    public Button characterSoundButtonOn;
    public Button characterSoundButtonOff;

    // AudioSource cho nhạc nền
    private RandomMusicPlayer randomMusicPlayer;

    // AudioSource cho âm thanh của nhân vật
    private AudioSource characterAudioSource;

    // Các AudioClip liên quan đến nhân vật
    public AudioSource[] characterAudioSources;

    private void Start()
    {
        // Tìm đối tượng có script RandomMusicPlayer
        randomMusicPlayer = FindObjectOfType<RandomMusicPlayer>();

        if (randomMusicPlayer == null)
        {
            Debug.LogError("Không tìm thấy RandomMusicPlayer trong scene!");
        }

        // Gán các sự kiện cho các nút nhạc nền
        if (musicButtonOn != null && musicButtonOff != null)
        {
            musicButtonOn.onClick.AddListener(TurnOnMusic);
            musicButtonOff.onClick.AddListener(TurnOffMusic);
        }
        else
        {
            Debug.LogError("Music buttons không được gán đúng!");
        }

        // Gán các sự kiện cho các nút âm thanh của nhân vật
        if (characterSoundButtonOn != null && characterSoundButtonOff != null)
        {
            characterSoundButtonOn.onClick.AddListener(TurnOnCharacterSounds);
            characterSoundButtonOff.onClick.AddListener(TurnOffCharacterSounds);
        }
        else
        {
            Debug.LogError("Character sound buttons không được gán đúng!");
        }
    }

    // Bật nhạc nền
    public void TurnOnMusic()
    {
        if (randomMusicPlayer != null)
        {
            randomMusicPlayer.MuteAudio(false);
        }
    }

    // Tắt nhạc nền
    public void TurnOffMusic()
    {
        if (randomMusicPlayer != null)
        {
            randomMusicPlayer.MuteAudio(true);
        }
    }

    // Bật âm thanh của nhân vật
    public void TurnOnCharacterSounds()
    {
        MuteCharacterSounds(false);
    }

    // Tắt âm thanh của nhân vật
    public void TurnOffCharacterSounds()
    {
        MuteCharacterSounds(true);
    }

    // Tắt hoặc bật âm thanh của nhân vật
    private void MuteCharacterSounds(bool isMuted)
    {
        foreach (AudioSource source in characterAudioSources)
        {
            if (source != null)
            {
                source.mute = isMuted;
            }
        }
    }
}