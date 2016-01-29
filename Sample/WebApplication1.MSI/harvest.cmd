@echo off
set WIX=C:\Program Files (x86)\WiX Toolset v3.10
set HARVEST_PATH=..\publish\WebApplication1
set DIRID=INSTALLDIR
set GROUP=WebApplication1_Project
set VAR=var.WebApplication1PublishDir
set OUTPUT_PATH=.\
set SUBFOLDERID=WebApplication1_Project
set XSL_FILE=TransformsourceInHeatGeneratedFiles.xsl

echo Publish your project to %HARVEST_PATH%
pause

rem delete stuff you don't need
rem
rem add here stuff you want to change later e.g. a global.json, 
rem else wix will complain about multple files for one directory location
rem
rem del "%HARVEST_PATH%"\*.pdb

"%WIX%\bin\heat.exe" dir "%HARVEST_PATH%" -nologo -dr %DIRID% -cg hrv_%GROUP% -ag -sf -srd -suid -svb6 -scom -sreg -template fragment -indent 2 -t %XSL_FILE% -out "%OUTPUT_PATH%\hrv_%SUBFOLDERID%.wxs" 

pause

rem
rem manual harvest idea from
rem 
rem http://windows-installer-xml-wix-toolset.687559.n2.nabble.com/Heat-command-line-vs-HarvestDirectory-task-td7593917.html
rem 