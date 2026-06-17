#!/bin/bash

Arch="$1"
OutputPath="$2"

OutputArch="FluxGate-${Arch}"
FileName="FluxGate-${Arch}.zip"
CoreFileName="v2rayN-${Arch}.zip"
CoreRoot="v2rayN-${Arch}"
V2RAYN_VERSION="7.10.3"

rm -rf "$OutputArch" "$CoreRoot" "$FileName" "$CoreFileName"

wget -nv -O "$CoreFileName" "https://github.com/2dust/v2rayN/releases/download/$V2RAYN_VERSION/$CoreFileName"
unzip -q "$CoreFileName"
if [ ! -d "$CoreRoot" ] && [[ "$Arch" == *"-desktop" ]]; then
  CoreRoot="v2rayN-${Arch%-desktop}"
fi
mv "$CoreRoot" "$OutputArch"

cp -rf "$OutputPath"/. "$OutputArch"/
7z a -tZip "$FileName" "$OutputArch" -mx1
