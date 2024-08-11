using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    public AudioClip[] musicClips; // Danh sách các bài nhạc
    private AudioSource audioSource; // AudioSource component
    private AudioClip lastPlayedClip; // Lưu trữ bài nhạc vừa phát
    private List<AudioClip> availableClips; // Danh sách các bài nhạc còn lại để phát

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Thêm AudioSource component
        availableClips = new List<AudioClip>(musicClips); // Khởi tạo danh sách các bài nhạc có thể phát
        PlayNextMusic(); // Phát nhạc ngẫu nhiên khi scene bắt đầu
    }

    void PlayNextMusic()
    {
        if (availableClips.Count > 0)
        {
            // Lấy một bài nhạc ngẫu nhiên từ danh sách còn lại
            int randomIndex = Random.Range(0, availableClips.Count);
            AudioClip newClip = availableClips[randomIndex];

            // Đặt bài nhạc mới vào AudioSource và phát
            audioSource.clip = newClip;
            audioSource.Play();

            // Cập nhật biến lưu trữ bài nhạc vừa phát
            lastPlayedClip = newClip;

            // Loại bỏ bài nhạc vừa phát khỏi danh sách có thể phát
            availableClips.RemoveAt(randomIndex);

            // Nếu danh sách còn lại chỉ có một bài, phát bài đó và làm mới danh sách
            if (availableClips.Count == 0)
            {
                availableClips = new List<AudioClip>(musicClips);
            }

            // Đặt một hàm để gọi `PlayNextMusic` khi bài nhạc kết thúc
            StartCoroutine(WaitForMusicToEnd());
        }
    }

    IEnumerator WaitForMusicToEnd()
    {
        // Chờ cho đến khi bài nhạc kết thúc
        yield return new WaitForSeconds(audioSource.clip.length);

        // Phát bài nhạc tiếp theo
        PlayNextMusic();
    }
    public void MuteAudio(bool isMuted)
    {
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
    }
}