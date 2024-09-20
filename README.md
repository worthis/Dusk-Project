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

- Build SDL2-CS
- Build Dusk Project and Run :)

### How to run on Miyoo Mini Plus

- Publish to folder
- Copy files from the folder to `SDCARD/App/Dusk_Project/`
- Run the game from App menu MMP

### Hacks and their reasoning

`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`

- Prevents dotnet from complaining about globalization library

`export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$mydir/lib/`

- Appends required libraries to the required enviroment variable

## Credits & attribution

- Used [original art](https://opengameart.org/content/first-person-dungeon-crawl-art-pack) was made by [Clint Bellanger](http://clintbellanger.net) for the original [Heroine Dusk](http://heroinedusk.com) game ([CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/))
- The music is by [Yubatake](http://opengameart.org/users/yubatake) (CC-BY 3.0)
- Used the great 16 color palette made by DawnBringer at [PixelJoint](http://www.pixeljoint.com/forum/forum_posts.asp?TID=12795)
- Used [First Person Dungeon Crawl Enemies Remixed](https://opengameart.org/content/heroine-dusk-first-person-dungeon-crawl-enemies-remixed) by Stephen "Redshrike" Challener with Goblin concept by Justin Nichol ([CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/))

## Thanks

- [Clint Bellanger](http://clintbellanger.net) for [Heroine Dusk](http://heroinedusk.com) game
- [LostQasar](https://github.com/LostQuasar) for SDL2 C# Example