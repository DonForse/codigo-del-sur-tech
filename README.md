# codigo-del-sur-tech
 
This API should be able to run as it is.

It contains a local db file that should already be initialized.

if for some reason the db is not detected on a new computer, try to run this commands from the Package Manager Console in VS

    dotnet tool install --global dotnet-ef

    Add-Migration TestMigration -Project CodigoDelSurApi.Infrastructure -StartupProject CodigoDelSurApi.API

I used IIS to run it locally, run the API project...

-----

When trying to authenticate, remember to add the word "Bearer" in the Authorize in Swagger after a successfull login


The way it works is that the authorize box must be filled like this:
Bearer {token}