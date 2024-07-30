namespace DuskProject
{
    using DuskProject.Source;
    using SDL2;

    public class Program
    {
        private static int _updateFPS = 60;
        private static uint _lastUpdateTime;

        public static void Main()
        {
            WindowManager windowManager = WindowManager.GetInstance();
            windowManager.Init();
            windowManager.CreateWindow("Dusk Project");

            TextManager textManager = TextManager.GetInstance();
            textManager.Init();

            TitleManager titleManager = TitleManager.GetInstance();
            titleManager.Init();

            ExploreManager exploreManager = ExploreManager.GetInstance();
            exploreManager.Init();

            MazeWorldManager mazeWorldManager = MazeWorldManager.GetInstance();
            mazeWorldManager.Init();

            GameStateManager gameStateManager = GameStateManager.GetInstance();
            gameStateManager.Init();

            _lastUpdateTime = SDL.SDL_GetTicks();

            while (windowManager.WindowOpened())
            {
                /*Console.WriteLine("Game tick");*/

                // FPS limiter
                uint currentTime = SDL.SDL_GetTicks();
                uint timeDelta = _lastUpdateTime - currentTime;

                if (timeDelta < (uint)(1000 / _updateFPS))
                {
                    SDL.SDL_Delay((uint)(1000 / _updateFPS) - timeDelta);
                }

                _lastUpdateTime = currentTime;

                // Process input
                /*Console.WriteLine("Game tick: process input");*/
                windowManager.ProcessInput();

                // Check quit conditions
                /*Console.WriteLine("Game tick: check quit");*/
                if (windowManager.KeyPressed(InputKeys.KEY_MENU) ||
                    windowManager.KeyPressed(InputKeys.KEY_QUIT) ||
                    gameStateManager.Quitting())
                {
                    windowManager.Close();
                    continue;
                }

                // Update game logic
                /*Console.WriteLine("Game tick: logic");*/
                gameStateManager.Update();

                // Draw game state
                /*Console.WriteLine("Game tick: render");*/
                gameStateManager.Render();

                // Render screen
                /*Console.WriteLine("Game tick: display");*/
                windowManager.Display();
            }

            windowManager.Quit();
        }
    }
}
