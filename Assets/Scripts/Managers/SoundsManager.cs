using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundsManager : Singleton<SoundsManager>
{

	#region Audiosource pool

	[SerializeField] private AudioSource prefab;
	[SerializeField] private Transform sfxParent;
	[SerializeField] private Transform musciParent;

	private List<AudioSource> sfxPool;
	private List<AudioSource> musicPool;

	protected virtual void InitializeSFX() {

		if (sfxPool == null) sfxPool = new List<AudioSource>();

		for (int i = 0; i < 5; i++) {
			AudioSource audioSource = Instantiate<AudioSource>(prefab, sfxParent);
			sfxPool.Add(audioSource);
		}
	}

	protected virtual void InitializeMusic() {
		if (musicPool == null) musicPool = new List<AudioSource>();

		for (int i = 0; i < 5; i++) {
			AudioSource audioSource = Instantiate<AudioSource>(prefab, musciParent);
			musicPool.Add(audioSource);
		}
	}

	private AudioSource GetSfxSource() {

		for(int i = 0; i < sfxPool.Count; i++) {
			if (!sfxPool[i].isPlaying) {
				return sfxPool[i];
			}
		}

		AudioSource audioSource = Instantiate<AudioSource>(prefab, sfxParent);
		sfxPool.Add(audioSource);
		return audioSource;
	}

	private AudioSource GetMusicSource() {
		for (int i = 0; i < musicPool.Count; i++) {
			if (!musicPool[i].isPlaying) {
				return musicPool[i];
			}
		}

		AudioSource audioSource = Instantiate<AudioSource>(prefab, musciParent);
		musicPool.Add(audioSource);
		return audioSource;
	}

	#endregion

	[SerializeField] private List<AudioClip> store;

	protected override void Awake() {
		base.Awake();
		InitializeSFX();
		InitializeMusic();
	}

	public void PlaySound(string name, float volume=0.5f,float pitch=1) {
		AudioSource sfx = GetSfxSource();
		AudioClip clip = store.FirstOrDefault(x => x.name.Equals(name));
		sfx.pitch = pitch;
		sfx.volume = volume;
		sfx.PlayOneShot(clip);
	}

	public void PlayMusic(string name, float volume = 0.5f, float pitch = 1, bool loop=true) {
		AudioSource sfx = GetMusicSource();
		AudioClip clip = store.FirstOrDefault(x => x.name.Equals(name));
		sfx.pitch = pitch;
		sfx.volume = volume;
		sfx.loop = loop;
		sfx.clip = clip;
		sfx.Play();
	}

	public void StopMusic() {
		if(musicPool!=null) musicPool.ForEach(x => x.Stop());
	}
}
