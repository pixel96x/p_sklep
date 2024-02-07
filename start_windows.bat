@ECHO off
SET PATH=lib\ncurses;%PATH%
ECHO %PATH%
dotnet run --project .\src\UI\
