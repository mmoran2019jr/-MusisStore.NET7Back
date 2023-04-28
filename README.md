# musicstoreapi
Este proyecto forma parte del curso de MitoCode .NET 7 FullStack y Angular 15.
El cual se encuentra en la [web oficial de MitoCode](https://mitocode.com/netfs.html)

## Referencias de comandos utiles para programar con .NET CLI (Command Line Interface) #

## Crear un proyecto de libreria de clases 
`dotnet new classlib -o MusicStore.Entities`

_La plantilla "Biblioteca de clases" se creo correctamente._

## Listar los archivos de una carpeta

`ls`

## Agregar un proyecto a una solucion existente

`dotnet sln add .\MusicStore.Entities\`

_Se ha agregado el proyecto "MusicStore.Entities\MusicStore.Entities.csproj" a la solución._

`dotnet sln add .\MusicStore.DataAccess\`

_Se ha agregado el proyecto "MusicStore.DtaAccess\MusicStore.DataAccess.csproj" a la solución._

## Consultar que proyectos estan dentro de la solucion

`dotnet sln .\MusicStore.sln list`

## Agregar un paquete nuget a un proyecto existente

`dotnet add package Microsoft.EntityFrameworkCore --version 7.0.2`

## Agregar una referencia de proyecto 
`dotnet add reference ..\MusicStore.Entities\`

_Se ha agregado la referencia "..\MusicStore.Entities\MusicStore.Entities.csproj" al proyecto._

## Comprobar si se tiene instalado Entity Framework Core Tools

`dotnet ef`

_Deberia mostrarse el resultado siguiente:_

                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

Entity Framework Core .NET Command-line Tools 7.0.2

## Instalar EF Core Tools de manera global en el equipo

`dotnet tool install dotnet-ef --global`

_La herramienta "dotnet-ef" ya está instalada._

## Actualizar EF Core Tools

`dotnet tool update dotnet-ef --global`

_La herramienta "dotnet-ef" se reinstaló con la versión estable más reciente (versión "7.0.2")._

## Crear una migracion con EF

`dotnet ef migrations add Initial-Migration --startup-project .\MusicStore\ --project .\MusicStore.DataAccess\`

_Donde el parametro **--startup-project** es el proyecto que contiene la cadena de conexion y el parametro **--project** es el que contiene la clase con el DbContext._

## Aplicar una migracion
`dotnet ef database update --startup-project .\MusicStore\ --project .\MusicStore.DataAccess\`

_Esto hará que se cree la base de datos en la cadena de conexión de no existir._

## Crear un archivo gitignore para la solucion

`dotnet new gitignore`

_La plantilla "archivo gitignore de dotnet" se creó correctamente._

## Comandos para crear un migration bundle

Este comando creara el archivo ejecutable del bundle en una carpeta distinta a la fuente, mostrando el detalle de la operacion y optimizada para Windows de 64 bits, se utiliza el parametro --force para sobrescribir el archivo en caso de existir ya en la carpeta.

`dotnet ef migrations bundle --project .\MusicStore.DataAccess\ --startup-project .\MusicStore\ -o C:\Servidor\bundlemusicstore.exe --verbose --self-contained -r win-x64 --force`