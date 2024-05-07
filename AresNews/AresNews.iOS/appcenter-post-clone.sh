#!/usr/bin/env bash

export RemoveANDROIDProjects=true

find "$APPCENTER_SOURCE_DIRECTORY" -iname '*.sln' -type f -print0 | while IFS= read -r -d '' SLN_PATH; do

if [ -z "$SLN_PATH" ]; then

echo "No Solution Found. Exiting Script."

exit

else

echo "SLN_PATH = $SLN_PATH"

fi

if [ -z "$RemoveANDROIDProjects" ]; then

echo "Do Not Remove Android Projects"

else

echo "Searching for Android Projects"

find "$APPCENTER_SOURCE_DIRECTORY" -iname '*Android*.csproj' -type f -print0 | while IFS= read -r -d '' path; do

echo "Removing $path from $SLN_PATH"

dotnet sln "$SLN_PATH" remove "$path" || true

done

fi

done