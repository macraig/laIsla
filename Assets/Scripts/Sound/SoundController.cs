using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Settings;
using System;

namespace Assets.Scripts.Sound{
    public class SoundController : MonoBehaviour{
	    private static SoundController instance;


		[SerializeField]
		private AudioClip failureSound,rightAnswerSound,dragSound,dropSound;
		[SerializeField]
		private AudioClip levelCompleteSound;
		[SerializeField]
		private AudioClip music;
		[SerializeField]
		private AudioClip clickSound,switchSound,typeSound;

        [SerializeField] private AudioClip[] _musicNotesClips;



        public AudioSource mySource;
	    public AudioSource musicSource;
	    public List<List<AudioClip>> instructionSounds;

		private bool concatenatingAudios = false;


    //Awake is always called before any Start functions
        void Awake()
    {
	    if (instance == null)           
		    instance = this;
	    else if (instance != this)
		    Destroy(gameObject);


	    DontDestroyOnLoad(transform.root.gameObject);
    }

		public static SoundController GetController()
		{
			return instance;
		}


        public void PlayClip(AudioClip myClip)
    {
	    mySource.clip = myClip;
		mySource.Play();
    }
			
	public void PlayFailureSound()  {
		PlaySFXSound (failureSound);
    }

    public void PlayClickSound() {
		PlaySFXSound (clickSound);
    }
	public void PlaySwitchSound(){
		PlaySFXSound (switchSound);
	}

	public void PlayTypingSound()
	{
		PlaySFXSound (typeSound);
	}

    public void PlayRightAnswerSound(){
		PlaySFXSound (rightAnswerSound);
    }

    public void PlayLevelCompleteSound()
    {
		PlaySFXSound (levelCompleteSound);
    }

		public void PlayDragSound()
		{
			PlaySFXSound (dragSound);
		}

		public void PlayDropSound()
		{
			PlaySFXSound (dropSound);
		}

	public void PlaySFXSound(AudioClip clip){
		if (SettingsController.GetController().SfxOn()) {
			mySource.clip = clip;
			mySource.Play();
		}
	}

    public void PlayInstruction(int level, int language)
    {
			if (SettingsController.GetController().SfxOn()) {
		    mySource.clip = instructionSounds[language][level];
		    mySource.Play();         	
	    }
    }

    public void PlayMusic()
   	{
			if (SettingsController.GetController().MusicOn()&&!musicSource.isPlaying) {
				musicSource.clip = music;
		    	musicSource.Play();         	
	    }
    }

        public void StopMusic()
    {
	    musicSource.Stop();
    }

		public void PlayLevelMusic(AudioSource mySource, AudioClip clip){
			mySource.clip = clip;
			mySource.Play();      
		}

		public void StopLevelMusic(AudioSource mySource)
		{
			mySource.Stop();
		}

        public void StopSound()
            {
	            mySource.Stop();
				concatenatingAudios = false;
            }

		//Trying to concatenate audios!

		private List<AudioClip> playingAudios;
		private AudioClip currentAudio;
		private int currentAudioIndex;
		private Action action;

		public void ConcatenateAudio(AudioClip audio, Action f){
			List<AudioClip> l = new List<AudioClip> ();
			l.Add (audio);
			ConcatenateAudios (l, f);
		}

		public void ConcatenateAudios(List<AudioClip> audios, Action f){
			concatenatingAudios = true;
			action = f;
			currentAudio = null;
			playingAudios = audios;
			Invoke ("PlayCurrentAudios", 0.01f);
		}

		public void SetConcatenatingAudios(bool concatenating){
			concatenatingAudios = concatenating;
		}

		public void PlayCurrentAudios(){
			if (concatenatingAudios) {
				if (currentAudio == null) {
					currentAudio = playingAudios [0];
					currentAudioIndex = 0;
				}
				else if (currentAudioIndex == playingAudios.Count - 1) {
					currentAudio = null;
					concatenatingAudios = false;
					if(action!=null)action.Invoke ();
					return;
				} else {
					currentAudioIndex++;
					currentAudio = playingAudios [currentAudioIndex];
				}
				SoundController.instance.PlayClip(currentAudio);
				Invoke ("PlayCurrentAudios", currentAudio.length);
			}



		}

        public void PlayMusicNote(string name)
        {

            switch (name)
            {
                case "RED":
                    PlayClip(_musicNotesClips[0]);
                    return;
                case "YELLOW":
                    PlayClip(_musicNotesClips[1]);
                    return;
                case "CYAN":
                    PlayClip(_musicNotesClips[2]);
                    return;
                case "GREEN":
                    PlayClip(_musicNotesClips[3]);
                    return;
                case "PINK":
                    PlayClip(_musicNotesClips[4]);
                    return;
                case "BLUE":
                    PlayClip(_musicNotesClips[5]);
                    return;
                case "BROWN":
                    PlayClip(_musicNotesClips[6]);
                    return;
            }
            // case blank
            PlayDropSound();
           
        }
    }

}