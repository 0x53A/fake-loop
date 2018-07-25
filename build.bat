
@echo off

setlocal

cd "%~dp0a_Build"
dotnet restore "build.csproj"

if "%*." == "." (
	echo dotnet fake -v build "build.fsx" --root "%~dp0\"
	dotnet fake build "build.fsx" --root "%~dp0\"
)else (
	REM we have arguments ...
	REM the first argument is the target list
	echo dotnet fake -v build "build.fsx" --root "%~dp0\" --multi-target %*
	dotnet fake build "build.fsx" --root "%~dp0\" --multi-target %*
)