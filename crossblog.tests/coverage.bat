@echo off

dotnet clean
dotnet build /p:DebugType=Full
dotnet minicover instrument --workdir ../ --assemblies crossblog.tests/**/bin/**/*.dll --sources crossblog/**/*.cs --exclude-sources crossblog/Migrations/**/*.cs --exclude-sources crossblog/*.cs --exclude-sources crossblog\Domain\CrossBlogDbContext.cs

dotnet minicover reset --workdir ../

dotnet test --no-build
dotnet minicover uninstrument --workdir ../
dotnet minicover report --workdir ../ --threshold 70