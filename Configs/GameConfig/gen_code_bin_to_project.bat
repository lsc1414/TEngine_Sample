Cd /d %~dp0
echo %CD%

set WORKSPACE=../..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set DATA_OUTPATH=%WORKSPACE%/UnityProject/Assets/AssetRaw/Configs/bytes/
set CODE_OUTPATH=%WORKSPACE%/UnityProject/Assets/GameScripts/HotFix/GameProto/GameConfig/

echo f| xcopy /s /e /i /y "%CONF_ROOT%\CustomTemplate\ConfigSystem.cs" "%WORKSPACE%\UnityProject\Assets\GameScripts\HotFix\GameProto\ConfigSystem.cs"


dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%CODE_OUTPATH% ^
    -x outputDataDir=%DATA_OUTPATH% ^
    -x l10n.provider=default ^
    -x "l10n.textFile.path=%CONF_ROOT%\Datas\l10n\#Localization.xlsx" ^
    -x l10n.textFile.keyFieldName=key 

echo f| xcopy /s /e /i /y "%CONF_ROOT%\CustomTemplate\GameConfig\l10n\LocalizationExt.cs" "%WORKSPACE%\UnityProject\Assets\GameScripts\HotFix\GameProto\GameConfig\l10n\LocalizationExt.cs"
pause

