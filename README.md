# Dusk Project

It is a basic dungeon crawl made using an old aesthetic.
The game's world is set in a fantasy human realm where the sun has not returned in several days. Evil forces are using the safety of night to invade. You are a serf woman takes up arms to fight against the darkness.

Dusk Project (temp name) is basically a C# port of [Clint Bellanger](http://clintbellanger.net) wonderful [Heroine Dusk](http://heroinedusk.com) game.

### Publish Settings

- Runtime: linux-arm
- Deployment mode: Self-contained
- File publish options:
  - Publish single file
  - Trim unused code

### How to run on Windows

- Just Build and Run :)

### How to run on Miyoo Mini Plus

- Publish to folder
- Copy files from the folder to `SDCARD/App/Dusk_Project/`
- Run the game from App menu MMP

### Hacks and their reasoning

`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`

- Prevents dotnet from complaining about globalization library

`export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$mydir/lib/`

- Appends required libraries to the required enviroment variable

## Thanks
- [LostQasar](https://github.com/LostQuasar) for SDL2 C# Example