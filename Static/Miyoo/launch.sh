#!/bin/sh
echo $0 $*
mydir=`pwd`
export LD_LIBRARY_PATH=$mydir/lib:$LD_LIBRARY_PATH
export SDL_VIDEODRIVER=mmiyoo
export SDL_AUDIODRIVER=mmiyoo
export EGL_VIDEODRIVER=mmiyoo
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
cd $(dirname "$0")

if [ -f /mnt/SDCARD/.tmp_update/script/stop_audioserver.sh ]; then
    /mnt/SDCARD/.tmp_update/script/stop_audioserver.sh
else
    killall audioserver
    killall audioserver.mod
fi

./DuskProject > DuskProject.log
