**🚨 Important: Update Your AppSettings!**
Before you build and run this project, make sure to update the appsettings.json file with your own configurations. Here are the key settings you need to change:

**JWT Settings**
Replace the values with your own secret key, issuer, and audience.

`"JWTSettings": {
    "Key": "YourSuperSecretKey",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience"
}`

**Connection Strings**
Update the DevConnection value with your actual database connection string.

`"ConnectionStrings": {
    "DevConnection": "YourDatabaseConnectionString"
}
`
