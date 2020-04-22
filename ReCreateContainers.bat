ECHO OFF
ECHO Removing and recreating containers. Please wait..



SET errorfile=%cd%\error.log


REM Delete all running containers
ECHO.Running command : FOR /f "tokens=*" %%i IN ('docker container ps -aq') DO docker container stop %%i && docker container rm %%i & ECHO.
FOR /f "tokens=*" %%i IN ('docker container ps -aq') DO docker container stop %%i && docker container rm %%i 2>>%errorfile%

IF %errorlevel% NEQ 0 GOTO ERROR 


REM Delete all stopped containers
REM ECHO.Running command : FOR /f "tokens=*" %%i IN ('docker images --format "{{.ID}}"') DO docker rmi %%i    & ECHO.
REM FOR /f "tokens=*" %%i IN ('docker images --format "{{.ID}}"') DO docker rmi %%i   2>>%errorfile%
REM IF %errorlevel% NEQ 0 GOTO ERROR

REM run compose.yml
ECHO.Running command : docker-compose up  & ECHO.
docker-compose up  2>>%errorfile%
IF %errorlevel% NEQ 0 GOTO ERROR

GOTO OK

:ERROR
ECHO Program failed with error code %errorlevel%, please check error file for more error."
ECHO Error file created on the location %errorfile% 
GOTO END


:OK
ECHO "Containers recreated successfully"
ECHO For warning and non fatal error please check error file if created on the loaction %errorfile%"

GOTO END

:END

ECHO ON






