@echo off 
copy ..\EfCore.Shaman.Core.sln .\
copy ..\global.json  .\

copy ..\EfCore.Shaman\project.json .\EfCore.Shaman\
copy ..\EfCore.Shaman\EfCore.Shaman.xproj .\EfCore.Shaman\

copy ..\EfCore.Shaman.SqlServer\project.json .\EfCore.Shaman.SqlServer\
copy ..\EfCore.Shaman.SqlServer\EfCore.Shaman.SqlServer.xproj .\EfCore.Shaman.SqlServer\

copy ..\EfCore.Shaman.Tests\project.json .\EfCore.Shaman.Tests\
copy ..\EfCore.Shaman.Tests\EfCore.Shaman.Tests.xproj .\EfCore.Shaman.Tests\



del ..\EfCore.Shaman.Core.sln
del ..\global.json

del ..\EfCore.Shaman\project.json
del ..\EfCore.Shaman\project.lock.json
del ..\EfCore.Shaman\EfCore.Shaman.xproj

del ..\EfCore.Shaman.SqlServer\project.json
del ..\EfCore.Shaman.SqlServer\project.lock.json
del ..\EfCore.Shaman.SqlServer\EfCore.Shaman.SqlServer.xproj

del ..\EfCore.Shaman.Tests\project.json
del ..\EfCore.Shaman.Tests\project.lock.json
del ..\EfCore.Shaman.Tests\EfCore.Shaman.Tests.xproj

pause
