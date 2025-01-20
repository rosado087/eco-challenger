# eco-challenger

## Configure Project for developement

### Setup .Net Core DB Connection

1. Set the connection string in the appsettings.json file, such as the example below:

```
"ConnectionStrings": {
    "Default": "Data Source=192.168.1.71;Initial Catalog=EcoChallengerDB;User ID=sa;Password=RobloxFixe123!;TrustServerCertificate=True"
}
```

2. Install the Entity Framework command line tool if you don't have it yet
`dotnet tool install --global dotnet-ef`

And add it to the path variable if its not done automatically.

This is example is for Linux:
`export PATH="$PATH:/home/username/.dotnet/tools"`


3. Run the database migrations

`dotnet ef databae update`

In case you want to rebuild the migrations script (this shouldn't be needed), you can do it with the following commands:
`dotnet ef migrations remove`
`dotnet ef migrations add Inital`
