namespace DuskProject
{
    using DuskProject.Source;
    using DuskProject.Source.Creatures;
    using DuskProject.Source.Enums;
    using SDL2;

    public class Program
    {
        private static int _updateFPS = 60;
        private static uint _lastUpdateTime;

        public static void Main()
        {
            // Create singletons
            WindowManager windowManager = WindowManager.GetInstance();
            ResourceManager resourceManager = ResourceManager.GetInstance();
            SoundManager soundManager = SoundManager.GetInstance();
            TextManager textManager = TextManager.GetInstance();
            GameStateManager gameStateManager = GameStateManager.GetInstance();
            WorldManager worldManager = WorldManager.GetInstance();
            ItemManager itemManager = ItemManager.GetInstance();
            Avatar avatar = Avatar.GetInstance();

            // Initialization
            windowManager.CreateWindow("Dusk Project");
            resourceManager.Init(windowManager.ScreenSurfacePtr);
            soundManager.Init();
            textManager.Init(resourceManager.LoadImage("Data/images/interface/boxy_bold.png"));
            gameStateManager.Init();
            itemManager.Init();
            worldManager.Init();
            avatar.Init();

            gameStateManager.MainMenu();

            _lastUpdateTime = SDL.SDL_GetTicks();

            while (windowManager.WindowOpened)
            {
                // FPS limiter
                uint currentTime = SDL.SDL_GetTicks();
                uint timeDelta = _lastUpdateTime - currentTime;

                if (timeDelta < (uint)(1000 / _updateFPS))
                {
                    SDL.SDL_Delay((uint)(1000 / _updateFPS) - timeDelta);
                }

                _lastUpdateTime = currentTime;

                // Process input
                windowManager.ProcessInput();

                // Check quit conditions
                if (windowManager.KeyPressed(InputKey.KEY_MENU) ||
                    windowManager.KeyPressed(InputKey.KEY_QUIT) ||
                    gameStateManager.Quitting())
                {
                    _ = gameStateManager.Save();
                    windowManager.Close();
                    continue;
                }

                // Update game logic
                gameStateManager.Update();

                // Draw game state
                gameStateManager.Render();

                // Render screen
                windowManager.Display();
            }

            soundManager.Quit();
            windowManager.Quit();
        }
    }
}
