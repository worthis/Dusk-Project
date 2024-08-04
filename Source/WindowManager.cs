namespace DuskProject.Source
{
    using System;
    using System.Runtime.InteropServices;
    using DuskProject.Source.Enums;
    using SDL2;

    public class WindowManager
    {
        private static WindowManager instance;
        private static object instanceLock = new object();

        private int _renderWidth = 320;
        private int _renderHeight = 240;
        private int _renderScale = 2;
        private int _windowWidth = 640;
        private int _windowHeight = 480;
        private int _windowBPP = 32;

        private bool _windowOpened = false;

        private IntPtr _window = IntPtr.Zero;
        private IntPtr _renderer;
        private IntPtr _texture;
        private IntPtr _screenPtr;
        private SDL.SDL_Surface _screenSurface;
        private SDL.SDL_Rect _screenRect;

        private List<InputKey> _keyPressed = new List<InputKey>();

        private WindowManager()
        {
        }

        public int WindowWidth { get => _windowWidth; }

        public int WindowHeight { get => _windowHeight; }

        public int RenderWidth { get => _renderWidth; }

        public int RenderHeight { get => _renderHeight; }

        public IntPtr Renderer { get => _renderer; }

        public IntPtr ScreenSurfacePtr { get => _screenPtr; }

        public bool WindowOpened { get => _windowOpened; }

        public static WindowManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new WindowManager();
                        Console.WriteLine("WindowManager created");
                    }
                }
            }

            return instance;
        }

        public void CreateWindow(string title)
        {
            if (_window == IntPtr.Zero)
            {
                CheckSDLError(SDL.SDL_Init(SDL.SDL_INIT_VIDEO));
            }

            SDL.SDL_ShowCursor(SDL.SDL_ENABLE);

            _window = SDL.SDL_CreateWindow(
                title,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                _windowWidth,
                _windowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            _renderer = SDL.SDL_CreateRenderer(
                _window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            _screenPtr = SDL.SDL_CreateRGBSurface(0, _renderWidth, _renderHeight, _windowBPP, 0, 0, 0, 0);
            _screenSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(_screenPtr);

            _texture = SDL.SDL_CreateTexture(
                _renderer,
                SDL.SDL_PIXELFORMAT_ARGB8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                _renderWidth,
                _renderHeight);

            SDL.SDL_GetClipRect(_screenPtr, out _screenRect);

            SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(_renderer);

            _windowOpened = true;

            Console.WriteLine("Window created");
        }

        public void Display()
        {
            CheckSDLError(SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, _screenSurface.pixels, _screenSurface.pitch));
            CheckSDLError(SDL.SDL_RenderCopy(_renderer, _texture, ref _screenRect, IntPtr.Zero));
            SDL.SDL_RenderPresent(_renderer);
            CheckSDLError(SDL.SDL_RenderClear(_renderer));
        }

        public void Close()
        {
            _windowOpened = false;

            SDL.SDL_FreeSurface(_screenPtr);
            SDL.SDL_DestroyTexture(_texture);
            SDL.SDL_DestroyRenderer(_renderer);
            SDL.SDL_DestroyWindow(_window);

            Console.WriteLine("Window closed");
        }

        public void Quit()
        {
            SDL.SDL_Quit();

            Console.WriteLine("WindowManager quit");
        }

        public void ProcessInput()
        {
            _keyPressed.Clear();

            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        _keyPressed.Add(InputKey.KEY_QUIT);
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_UP:
                                _keyPressed.Add(InputKey.KEY_UP);
                                break;

                            case SDL.SDL_Keycode.SDLK_DOWN:
                                _keyPressed.Add(InputKey.KEY_DOWN);
                                break;

                            case SDL.SDL_Keycode.SDLK_LEFT:
                                _keyPressed.Add(InputKey.KEY_LEFT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RIGHT:
                                _keyPressed.Add(InputKey.KEY_RIGHT);
                                break;

                            case SDL.SDL_Keycode.SDLK_SPACE:
                                _keyPressed.Add(InputKey.KEY_A);
                                break;

                            case SDL.SDL_Keycode.SDLK_LCTRL:
                                _keyPressed.Add(InputKey.KEY_B);
                                break;

                            case SDL.SDL_Keycode.SDLK_LSHIFT:
                                _keyPressed.Add(InputKey.KEY_X);
                                break;

                            case SDL.SDL_Keycode.SDLK_LALT:
                                _keyPressed.Add(InputKey.KEY_Y);
                                break;

                            case SDL.SDL_Keycode.SDLK_e:
                                _keyPressed.Add(InputKey.KEY_L1);
                                break;

                            case SDL.SDL_Keycode.SDLK_TAB:
                                _keyPressed.Add(InputKey.KEY_L2);
                                break;

                            case SDL.SDL_Keycode.SDLK_t:
                                _keyPressed.Add(InputKey.KEY_R1);
                                break;

                            case SDL.SDL_Keycode.SDLK_BACKSPACE:
                                _keyPressed.Add(InputKey.KEY_R2);
                                break;

                            case SDL.SDL_Keycode.SDLK_RCTRL:
                                _keyPressed.Add(InputKey.KEY_SELECT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RETURN:
                                _keyPressed.Add(InputKey.KEY_START);
                                break;

                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                _keyPressed.Add(InputKey.KEY_MENU);
                                break;
                        }

                        break;
                }
            }
        }

        public bool KeyPressed(InputKey key)
        {
            return _keyPressed.Contains(key);
        }

        private static void CheckSDLError(int error)
        {
            if (error < 0)
            {
                Console.WriteLine("Error: (SDL Error) " + SDL.SDL_GetError());
            }
        }
    }
}
