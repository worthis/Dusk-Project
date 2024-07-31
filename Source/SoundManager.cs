namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Resources;
    using SDL2;

    public enum SFX
    {
        Attack,
        Blocked,
        BoneShield,
        Click,
        Coin,
        Critical,
        Defeat,
        Fire,
        Heal,
        HPDrain,
        Miss,
        MPDrain,
        Run,
        Unlock,
    }

    public class SoundManager
    {
        private static SoundManager instance;
        private static object instanceLock = new object();

        private ResourceManager _resourceManager;

        private bool _useSound = false;
        private int _volumeMusic = 128;
        private int _volumeSound = 128;

        private SoundResource[] _sounds;
        private MusicResource _music;

        private SoundManager()
        {
        }

        public static SoundManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SoundManager();
                        Console.WriteLine("SoundManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _resourceManager = ResourceManager.GetInstance();

            if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_AUDIO) == -1)
            {
                _useSound = false;
                Console.WriteLine("Error: Failed to init audio subsystem");

                return;
            }

            _useSound = true;

            SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_OGG);
            SDL_mixer.Mix_OpenAudio(44100, SDL.AUDIO_S16SYS, 2, 1024);
            SDL_mixer.Mix_VolumeMusic(_volumeMusic);

            LoadSounds();

            Console.WriteLine("SoundManager initialized");
        }

        public void Quit()
        {
            if (!_useSound)
            {
                return;
            }

            SDL_mixer.Mix_PauseMusic();
            SDL_mixer.Mix_HaltMusic();

            SDL_mixer.Mix_CloseAudio();
        }

        public void PlaySound(SFX sound, int channel = -1)
        {
            if (!_useSound)
            {
                return;
            }

            SDL_mixer.Mix_PlayChannel(channel, _sounds[(int)sound].GetSound(), 0);
        }

        public bool IsPlaying(int channel = -1)
        {
            if (!_useSound)
            {
                return false;
            }

            return SDL_mixer.Mix_Playing(channel) != 0;
        }

        public void StopSound(int channel = -1)
        {
            if (!_useSound)
            {
                return;
            }

            SDL_mixer.Mix_HaltChannel(channel);
        }

        public void PlayMusic(string musicName)
        {
            if (!_useSound)
            {
                return;
            }

            string fileName = string.Format("Data/music/{0}.ogg", musicName);

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Failed to open music {0}", fileName);
                return;
            }

            if (_music is not null &&
                !_music.Name.Equals(fileName))
            {
                SDL_mixer.Mix_HaltMusic();
                _resourceManager.Free(_music);
                _music = null;
            }

            if (_music is null)
            {
                _music = _resourceManager.LoadMusic(fileName);
            }

            Console.WriteLine("Playing music is {0}", SDL_mixer.Mix_PlayingMusic());

            if (_music is not null &&
                _music.GetMusic() != IntPtr.Zero &&
                SDL_mixer.Mix_PlayingMusic() <= 0)
            {
                SDL_mixer.Mix_PlayMusic(_music.GetMusic(), -1);
            }
        }

        public void PauseMusic()
        {
            if (!_useSound)
            {
                return;
            }

            SDL_mixer.Mix_PauseMusic();
        }

        public void StopMusic()
        {
            if (!_useSound)
            {
                return;
            }

            SDL_mixer.Mix_HaltMusic();
        }

        private void LoadSounds()
        {
            _sounds = new SoundResource[Enum.GetValues(typeof(SFX)).Length];
            _sounds[(int)SFX.Attack] = _resourceManager.LoadSound("Data/sounds/attack.wav");
            _sounds[(int)SFX.Blocked] = _resourceManager.LoadSound("Data/sounds/blocked.wav");
            _sounds[(int)SFX.BoneShield] = _resourceManager.LoadSound("Data/sounds/boneshield.wav");
            _sounds[(int)SFX.Click] = _resourceManager.LoadSound("Data/sounds/click.wav");
            _sounds[(int)SFX.Coin] = _resourceManager.LoadSound("Data/sounds/coin.wav");
            _sounds[(int)SFX.Critical] = _resourceManager.LoadSound("Data/sounds/critical.wav");
            _sounds[(int)SFX.Defeat] = _resourceManager.LoadSound("Data/sounds/defeat.wav");
            _sounds[(int)SFX.Fire] = _resourceManager.LoadSound("Data/sounds/fire.wav");
            _sounds[(int)SFX.Heal] = _resourceManager.LoadSound("Data/sounds/heal.wav");
            _sounds[(int)SFX.HPDrain] = _resourceManager.LoadSound("Data/sounds/hpdrain.wav");
            _sounds[(int)SFX.Miss] = _resourceManager.LoadSound("Data/sounds/miss.wav");
            _sounds[(int)SFX.MPDrain] = _resourceManager.LoadSound("Data/sounds/mpdrain.wav");
            _sounds[(int)SFX.Run] = _resourceManager.LoadSound("Data/sounds/run.wav");
            _sounds[(int)SFX.Unlock] = _resourceManager.LoadSound("Data/sounds/unlock.wav");
        }
    }
}
