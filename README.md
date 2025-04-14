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

`dotnet ef database update`

In case you want to add a new migration, you can do it with the following command:
`dotnet ef migrations add <name of migration>`


## SMTP Configuration
This project uses https://app.brevo.com/ to send emails, to configure it set the following in your appsettings.json:

```
"EmailSettings": {
    "Sender": "202100518@estudantes.ips.pt",
    "SmtpServer": "smtp-relay.brevo.com",
    "SmtpPort": 587,
    "SmtpUser": "83df7a001@smtp-brevo.com",
    "SmtpPass": "t2sZdaJzDWEynA50",
    "UseSsl": true
}
```

The parameter "Sender" has to be configured as a authenticated sender in brevo, otherwise an error will be thrown.


## JWT Configuration
This project uses JWT for authentication, to configure it set the following in your appsettings.json:

```
"JwtSettings": {
    "Issuer": "<which entity generates the token>",
    "Audience": "<what is the token meant to give access to>",
    "Key": "<A generated key to encond the token>"
}
```

The "Key" parameter can be generated on a command line with the following `openssl rand -base64 32`.