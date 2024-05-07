#!/usr/bin/env bash
echo "Launching Android pre-build script"
if [ -z "$discord_client_id" ]
then
    echo "You need define the discord_client_id variable in App Center"
    exit
fi
if [ -z "$api_host" ]
then
    echo "You need define the api_host variable in App Center"
    exit
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/AresNews/AresNews/Core/AppConstant.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating AppConstant.cs"
    echo "---"
    echo "DiscordClientId = "$discord_client_id
    sed -i '' 's#DiscordClientId = Environment.GetEnvironmentVariable("discord_client_id")#DiscordClientId = "'$discord_client_id'"#g' $APP_CONSTANT_FILE
    echo "ApiHost = "$api_host
    sed -i '' 's#ApiHost = Environment.GetEnvironmentVariable("api_host")#ApiHost = "'$api_host'"#g' $APP_CONSTANT_FILE
    echo "File content:"
    cat $APP_CONSTANT_FILE
else
    echo "Incorrect file: "$APP_CONSTANT_FILE
    exit
fi