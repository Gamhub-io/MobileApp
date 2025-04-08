#!/usr/bin/env bash
echo "Launching ios pre-build script"
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
if [ -z "$monitoring_key" ]
then
    echo "You need define the monitoring_key variable in App Center"
    exit
fi

APP_CONSTANT_FILE=$project_folder/App/Core/AppConstant.cs

## Install google services
"<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>API_KEY</key>
	<string>"$firebase_api_key"</string>
	<key>GCM_SENDER_ID</key>
	<string>"$firebase_GCM_sender_ID"</string>
	<key>PLIST_VERSION</key>
	<string>"$firebase_plist_version"</string>
	<key>BUNDLE_ID</key>
	<string>com.bricefriha.aresgaming</string>
	<key>PROJECT_ID</key>
	<string>"$firebase_project_id"</string>
	<key>STORAGE_BUCKET</key>
	<string>"$firebase_storage_bucket"</string>
	<key>IS_ADS_ENABLED</key>
	<false></false>
	<key>IS_ANALYTICS_ENABLED</key>
	<false></false>
	<key>IS_APPINVITE_ENABLED</key>
	<true></true>
	<key>IS_GCM_ENABLED</key>
	<true></true>
	<key>IS_SIGNIN_ENABLED</key>
	<true></true>
	<key>GOOGLE_APP_ID</key>
	<string>"$google_app_id"</string>
</dict>
</plist>">> $project_folder/App/Platforms/iOS/GoogleService-Info.plist

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating AppConstant.cs"
    echo "---"
    echo "DiscordClientId = "$discord_client_id
    sed -i '' 's#DiscordClientId = Environment.GetEnvironmentVariable("discord_client_id")#DiscordClientId = "'$discord_client_id'"#g' $APP_CONSTANT_FILE
    echo "ApiHost = "$api_host
    sed -i '' 's#ApiHost = Environment.GetEnvironmentVariable("api_host")#ApiHost = "'$api_host'"#g' $APP_CONSTANT_FILE
    echo "MonitoringKey = "$monitoring_key
    sed -i '' 's#MonitoringKey = Environment.GetEnvironmentVariable("monitoring_key")#MonitoringKey = "'$monitoring_key'"#g' $APP_CONSTANT_FILE
    echo "File content:"
    cat $APP_CONSTANT_FILE
else
    echo "Incorrect file: "$APP_CONSTANT_FILE
    exit
fi