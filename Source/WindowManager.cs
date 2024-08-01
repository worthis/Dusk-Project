namespace DuskProject.Source
{
    using System;
    using System.Runtime.InteropServices;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
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

        private List<InputKeys> _keyPressed = new List<InputKeys>();

        private WindowManager()
        {
            if (_window == IntPtr.Zero)
            {
                CheckSDLErr(SDL.SDL_Init(SDL.SDL_INIT_VIDEO));
            }
        }

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

        public int GetWindowWidth()
        {
            return _windowWidth;
        }

        public int GetWindowHeight()
        {
            return _windowHeight;
        }

        public IntPtr GetRenderer()
        {
            return _renderer;
        }

        public void CreateWindow(string title)
        {
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

        public void Draw(ImageResource image, int srcX, int srcY, int srcW, int srcH, int dstX, int dstY)
        {
            SDL.SDL_Rect src = new()
            {
                x = srcX,
                y = srcY,
                w = srcW,
                h = srcH,
            };

            SDL.SDL_Rect dst = new()
            {
                x = dstX,
                y = dstY,
                w = srcW,
                h = srcH,
            };

            CheckSDLErr(SDL.SDL_BlitSurface(image.GetImage(), ref src, _screenPtr, ref dst));
        }

        public void Draw(ImageResource image)
        {
            Draw(image, 0, 0, image.Width, image.Height, 0, 0);
        }

        public void Display()
        {
            CheckSDLErr(SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, _screenSurface.pixels, _screenSurface.pitch));
            CheckSDLErr(SDL.SDL_RenderCopy(_renderer, _texture, ref _screenRect, IntPtr.Zero));
            SDL.SDL_RenderPresent(_renderer);
            CheckSDLErr(SDL.SDL_RenderClear(_renderer));
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

        public bool WindowOpened()
        {
            return _windowOpened;
        }

        public void ProcessInput()
        {
            _keyPressed.Clear();

            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        _keyPressed.Add(InputKeys.KEY_QUIT);
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_UP:
                                _keyPressed.Add(InputKeys.KEY_UP);
                                break;

                            case SDL.SDL_Keycode.SDLK_DOWN:
                                _keyPressed.Add(InputKeys.KEY_DOWN);
                                break;

                            case SDL.SDL_Keycode.SDLK_LEFT:
                                _keyPressed.Add(InputKeys.KEY_LEFT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RIGHT:
                                _keyPressed.Add(InputKeys.KEY_RIGHT);
                                break;

                            case SDL.SDL_Keycode.SDLK_SPACE:
                                _keyPressed.Add(InputKeys.KEY_A);
                                break;

                            case SDL.SDL_Keycode.SDLK_LCTRL:
                                _keyPressed.Add(InputKeys.KEY_B);
                                break;

                            case SDL.SDL_Keycode.SDLK_LSHIFT:
                                _keyPressed.Add(InputKeys.KEY_X);
                                break;

                            case SDL.SDL_Keycode.SDLK_LALT:
                                _keyPressed.Add(InputKeys.KEY_Y);
                                break;

                            case SDL.SDL_Keycode.SDLK_e:
                                _keyPressed.Add(InputKeys.KEY_L1);
                                break;

                            case SDL.SDL_Keycode.SDLK_TAB:
                                _keyPressed.Add(InputKeys.KEY_L2);
                                break;

                            case SDL.SDL_Keycode.SDLK_t:
                                _keyPressed.Add(InputKeys.KEY_R1);
                                break;

                            case SDL.SDL_Keycode.SDLK_BACKSPACE:
                                _keyPressed.Add(InputKeys.KEY_R2);
                                break;

                            case SDL.SDL_Keycode.SDLK_RCTRL:
                                _keyPressed.Add(InputKeys.KEY_SELECT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RETURN:
                                _keyPressed.Add(InputKeys.KEY_START);
                                break;

                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                _keyPressed.Add(InputKeys.KEY_MENU);
                                break;
                        }

                        break;
                }
            }
        }

        public bool KeyPressed(InputKeys key)
        {
            return _keyPressed.Contains(key);
        }

        private static void CheckSDLErr(int err)
        {
            if (err < 0)
            {
                Console.WriteLine("SDL Error Occured: " + SDL.SDL_GetError());
            }
        }
    }
}
