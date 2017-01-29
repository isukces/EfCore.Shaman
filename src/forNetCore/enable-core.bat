@echo off
copy EfCore.Shaman.Core.sln ..\
copy global.json  ..\

copy EfCore.Shaman\project.json ..\EfCore.Shaman\
copy EfCore.Shaman\EfCore.Shaman.xproj ..\EfCore.Shaman\

copy EfCore.Shaman.SqlServer\project.json ..\EfCore.Shaman.SqlServer\
copy EfCore.Shaman.SqlServer\EfCore.Shaman.SqlServer.xproj ..\EfCore.Shaman.SqlServer\

copy EfCore.Shaman.Tests\project.json ..\EfCore.Shaman.Tests\
copy EfCore.Shaman.Tests\EfCore.Shaman.Tests.xproj ..\EfCore.Shaman.Tests\

pause
