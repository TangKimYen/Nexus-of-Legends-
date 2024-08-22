using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour
{
    public Button musicButtonOn;
    public Button musicButtonOff;
    public Button characterSoundButtonOn;
    public Button characterSoundButtonOff;

    private RandomMusicPlayer randomMusicPlayer;

    // Mảng chứa tất cả các AudioSource của nhân vật
    public AudioSource[] characterAudioSources;
    private PlayerAttack playerAttack;
    private AssassinMovements assassinMovements;
    private PlayerHealth playerHealth;

    private void Start()
    {
        randomMusicPlayer = FindObjectOfType<RandomMusicPlayer>();

        playerAttack = FindObjectOfType<PlayerAttack>();
        assassinMovements = FindObjectOfType<AssassinMovements>();
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerAttack != null && assassinMovements != null && playerHealth != null)
        {
            // Gán tất cả các AudioSource từ PlayerAttack, AssassinMovements và PlayerHealth vào mảng characterAudioSources
            characterAudioSources = new AudioSource[]
            {
                playerAttack.attackSound,
                playerAttack.skill1Sound,
                playerAttack.skill2Sound,
                playerAttack.skill3Sound,
                assassinMovements.jumpSoundEffect,
                playerHealth.hurtSound,
                playerHealth.deathSound
            };
        }
        if (randomMusicPlayer == null)
        {
            Debug.LogError("Không tìm thấy RandomMusicPlayer trong scene!");
        }

        // Gán sự kiện cho các nút nhạc nền
        if (musicButtonOn != null && musicButtonOff != null)
        {
            musicButtonOn.onClick.AddListener(TurnOnMusic);
            musicButtonOff.onClick.AddListener(TurnOffMusic);
        }
        else
        {
            Debug.LogError("Music buttons không được gán đúng!");
        }

        // Gán sự kiện cho các nút âm thanh của nhân vật
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

    // Bật tất cả âm thanh của nhân vật
    public void TurnOnCharacterSounds()
    {
        MuteCharacterSounds(false);
    }

    // Tắt tất cả âm thanh của nhân vật
    public void TurnOffCharacterSounds()
    {
        MuteCharacterSounds(true);
    }

    // Hàm bật/tắt âm thanh của nhân vật
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