﻿namespace DuskProject.Source.Input
{
    using DuskProject.Source.Enums;
    using SDL2;

    public class MiyooInput : InputBase
    {
        public override void ProcessInput()
        {
            KeyPressedList.Clear();

            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        KeyPressedList.Add(InputKey.KEY_QUIT);
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_UP:
                                KeyPressedList.Add(InputKey.KEY_UP);
                                break;

                            case SDL.SDL_Keycode.SDLK_DOWN:
                                KeyPressedList.Add(InputKey.KEY_DOWN);
                                break;

                            case SDL.SDL_Keycode.SDLK_LEFT:
                                KeyPressedList.Add(InputKey.KEY_LEFT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RIGHT:
                                KeyPressedList.Add(InputKey.KEY_RIGHT);
                                break;

                            case SDL.SDL_Keycode.SDLK_SPACE:
                                KeyPressedList.Add(InputKey.KEY_A);
                                break;

                            case SDL.SDL_Keycode.SDLK_LCTRL:
                                KeyPressedList.Add(InputKey.KEY_B);
                                break;

                            case SDL.SDL_Keycode.SDLK_LSHIFT:
                                KeyPressedList.Add(InputKey.KEY_X);
                                break;

                            case SDL.SDL_Keycode.SDLK_LALT:
                                KeyPressedList.Add(InputKey.KEY_Y);
                                break;

                            case SDL.SDL_Keycode.SDLK_e:
                                KeyPressedList.Add(InputKey.KEY_L1);
                                break;

                            case SDL.SDL_Keycode.SDLK_TAB:
                                KeyPressedList.Add(InputKey.KEY_L2);
                                break;

                            case SDL.SDL_Keycode.SDLK_t:
                                KeyPressedList.Add(InputKey.KEY_R1);
                                break;

                            case SDL.SDL_Keycode.SDLK_BACKSPACE:
                                KeyPressedList.Add(InputKey.KEY_R2);
                                break;

                            case SDL.SDL_Keycode.SDLK_RCTRL:
                                KeyPressedList.Add(InputKey.KEY_SELECT);
                                break;

                            case SDL.SDL_Keycode.SDLK_RETURN:
                                KeyPressedList.Add(InputKey.KEY_START);
                                break;

                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                KeyPressedList.Add(InputKey.KEY_MENU);
                                break;
                        }

                        break;
                }
            }
        }
    }
}
