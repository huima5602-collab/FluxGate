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

mkdir -p "$OutputArch"
cp -rf "$OutputPath"/. "$OutputArch"/
rm -rf "$OutputArch/bin"
cp -rf "$CoreRoot/bin" "$OutputArch/bin"

if [ -e "$OutputArch/v2rayN.exe" ]; then
  echo "Unexpected v2rayN.exe in FluxGate package" >&2
  exit 1
fi

7z a -tZip "$FileName" "$OutputArch" -mx1
