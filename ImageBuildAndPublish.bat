ECHO OFF
ECHO Starting build and deployment process. Please wait..


REM Set current time and date

SET errorfile=%cd%\error.log

SET repository_name=engaze

SET tag_name=latest

SET image_name=%repository_name%/eventosubscriber:%tag_name%

SET container_name=eventosubscriber
SET user_name=engaze
SET password=LetsBuild

REM for /F "usebackq delims=" %A in (`docker conatiner ls`) do echo %A

REM delete all the associated containers
ECHO.Running command : docker container rm -f %container_name% & ECHO. 
docker container rm -f %container_name% 2>>%errorfile% 
IF %errorlevel% NEQ 0 IF %errorlevel% NEQ 1 GOTO ERROR REM avoid error like no such container name


REM delete the image
ECHO.Running command : docker image rm -f %image_name% & ECHO.
docker image rm -f %image_name% 2>>%errorfile%
IF %errorlevel% NEQ 0 GOTO ERROR


REM REM build the image
ECHO.Running command : docker build -t %image_name%  & ECHO.
docker build -t %image_name% .  2>>%errorfile%
IF %errorlevel% NEQ 0 GOTO ERROR 


REM login to docker registery
docker login -u=%user_name% -p=%password%
IF %errorlevel% NEQ 0 GOTO ERROR


REM push the image
ECHO.Running command : docker push %image_name%  & ECHO.
docker push %image_name% 2>>%errorfile%
IF %errorlevel% NEQ 0 GOTO ERROR


GOTO OK

:ERROR
ECHO Program failed with error code %errorlevel%, please check error file for more error."
ECHO Error file created on the location %errorfile% 
GOTO END


:OK
ECHO "Build and deployement successful"
ECHO For warning and non fatal error please check error file if created on the loaction %errorfile%"
ECHO Error file created on the location %errorfile% 
GOTO END

:END

ECHO ON






