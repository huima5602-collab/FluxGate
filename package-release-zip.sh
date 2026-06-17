#!/bin/bash

Arch="$1"
OutputPath="$2"

OutputArch="FluxGate-${Arch}"
FileName="FluxGate-${Arch}.zip"
CoreFileName="v2rayN-${Arch}.zip"
CoreRoot="v2rayN-${Arch}"

rm -rf "$OutputArch" "$CoreRoot" "$FileName" "$CoreFileName"

wget -nv -O "$CoreFileName" "https://github.com/2dust/v2rayN-core-bin/raw/refs/heads/master/$CoreFileName"
unzip -q "$CoreFileName"
mv "$CoreRoot" "$OutputArch"

cp -rf "$OutputPath"/. "$OutputArch"/
7z a -tZip "$FileName" "$OutputArch" -mx1
