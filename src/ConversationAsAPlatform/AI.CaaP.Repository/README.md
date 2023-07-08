# How to migrate


Migrations are located in AI.CaaP.Repository

From PackageManager Console, make sure the 'AI.CaaP.Repository' project is selected
Set the 'AI.CaaP.Repository' project as startup project


Prior: 
Install-Package Microsoft.EntityFrameworkCore.Tools

Add-Migration InitialCreate
Update-Database
Remove-Migration


dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update


# Seq logging using Docker

Download and install docker desktop

docker run -d --restart unless-stopped --name seq -e ACCEPT_EULA=Y -v C:\Temp\logs:/Data -p8081:80 datalust/seq:latest

dotnet add package Microsoft.EntityFrameworkCore.Sqlite



