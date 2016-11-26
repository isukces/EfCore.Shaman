@echo off
set fullpath=%1
shift

set KEY=%fullpath%\key.snk
echo looking for %KEY%
if exist %KEY% (
	exit \b 0
)
echo file %KEY% doesn't exist 

set SEARCH=%ProgramFiles(x86)%\Microsoft SDKs\Windows


For /R "%SEARCH%" %%G IN (sn.exe) do (
	if exist "%%G" (
		echo running
 		echo %%G -k %KEY%
	 	"%%G" -k %KEY%
		exit /b 0
	)
)
echo Unable to find sn.exe
exit /B 1

