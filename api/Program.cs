using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using api.Data.Settings;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

CredentialProfile credProfile;
AWSCredentials awsCredentials;
var sharedFile = new SharedCredentialsFile();
var awsSettings = builder.Configuration.GetSection("AWSOptions").Get<AWSOptions>();
if (sharedFile.TryGetProfile(awsSettings.Profile, out credProfile) &&
    AWSCredentialsFactory.TryGetAWSCredentials(credProfile, sharedFile, out awsCredentials))
{
    var client = new Amazon.DynamoDBv2.AmazonDynamoDBClient(awsCredentials, credProfile.Region);
    services.AddSingleton<Amazon.DynamoDBv2.IAmazonDynamoDB>(client);
    services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
}

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
