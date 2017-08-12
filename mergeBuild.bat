rem BRC build bat

rem Usage, in VS build events
rem il_merge.bat $(TargetName) $(TargetDir)

rem ilmerge path
set ilmerge=%~dp0\packages\ILMerge.2.14.1208\tools\ILMerge.exe

rem check if ilmerge exists
if not exist %ilmerge% goto ilmergenotfound

rem .NET reference paths
set "net_refpath_v452=%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2"
set "net_refpath_v45=%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"

rem Current reference path
set net_refpath=%net_refpath_v45%

set _workDir=%~2
set _targetName=%~1
set _filter=Microsoft.WindowsAPICodePack*.dll
REM set _filter=*.dll

echo net_refpath is %net_refpath%
echo:
echo Working dir: %_workDir%
echo TargetName: %_targetName%
echo:
rem merge Command
%ilmerge% /lib:"%net_refpath_v452%" /wildcards /targetplatform:v4 /target:winexe /out:%_workDir%%_targetName%.all.exe %_workDir%%_targetName%.exe %_workDir%%_filter%
echo:

REM %ilmerge% /lib:"%net_refpath_v452%" /wildcards /log /targetplatform:v4  /target:winexe /out:$(TargetDir)$(TargetName).all.exe $(TargetDir)$(TargetName).exe $(TargetDir)\*.dll

echo Cleanining old files:
echo:

rem Remove all subassemblies.
echo Deleting DLLs...
del %_filter%

rem Remove all .pdb files (except the new, combined pdb we just created).
echo Deleting PDBs...
ren "%_targetName%.all.pdb" "%_targetName%.all.pdb.temp"
del *.pdb
ren "%_targetName%.all.pdb.temp" "%_targetName%.all.pdb"

rem Remove xml documentation
echo Deleting XMLs...
if exist *.xml del *.xml else echo No .xml files found

echo Deleting old %_targetName%.exe
rem Delete the original, non-combined .exe.
del "%_targetName%.exe"

rem Rename the combined .exe and .pdb to the original project name we started with.
ren "%_targetName%.all.pdb" "%_targetName%.pdb"
ren "%_targetName%.all.exe" "%_targetName%.exe"

echo:
echo MERGE DONE!
goto:EOF

:error
echo Arg error, skipping merge...

:ilmergenotfound
echo %ilmerge% does not exist
