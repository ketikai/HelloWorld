@echo off
setlocal enabledelayedexpansion

set baseName=Localization

set sourceDir=%1
set targetDir=%1!baseName!

rmdir /s /q "%targetDir%"
mkdir "%targetDir%"

for /d %%d in ("%sourceDir%\*") do (
    set subDirName=%%~nxd
    set condition=true
    if "!subDirName!"=="!baseName!" set condition=false
    if "!subDirName!"=="Configs" set condition=false
    if "!subDirName!"=="Drivers" set condition=false
    if "!subDirName!"=="Fonts" set condition=false
    if "!subDirName!"=="Logs" set condition=false
    if "!subDirName!"=="Tools" set condition=false
    if "!condition!"=="true" (
        set sourceSubDir=%sourceDir%!subDirName!

        echo Moving !subDirName! to !targetDir!
        move /Y "!sourceSubDir!" "!targetDir!"
    )
)

endlocal