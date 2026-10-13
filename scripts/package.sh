#!/usr/bin/env bash
# Builds Media Bar Card for one Jellyfin version, zips it for a plugin repository
# and adds (or replaces) that version in manifest.json.
# Usage: scripts/package.sh            (plugin version comes from the .csproj <Version>)
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"

project="src/Jellyfin.Plugin.MediaBarCard"
jellyfin="10.11.11"
abi="${jellyfin}.0"
version="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$project/Jellyfin.Plugin.MediaBarCard.csproj")"
if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+\.0$ ]]; then
  echo "<Version> must be major.minor.patch.0, got '$version'" >&2
  exit 1
fi
tag="v${version%.0}"
out="$root/artifacts"
zip="$out/media-bar-card_${version%.0}.zip"

rm -rf "$out"
mkdir -p "$out"

node --check slideshowpure.js
dotnet publish "$project" -c Release -p:JellyfinVersion="$jellyfin" -o "$out/publish" --nologo

(cd "$out/publish" && zip -q -X "$zip" Jellyfin.Plugin.MediaBarCard.dll Jellyfin.Plugin.MediaBarCard.deps.json logo.png)
checksum="$(md5sum "$zip" | cut -d' ' -f1)"
timestamp="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
echo "Built $zip (md5 $checksum)"

python3 - "$root/manifest.json" "$version" "$abi" "$checksum" "$timestamp" "$tag" "$(basename "$zip")" <<'PY'
import json, os, sys
path, version, abi, checksum, timestamp, tag, zipname = sys.argv[1:]
repo = "https://github.com/diverseau/jellyfin-plugin-media-bar-card"
manifest = json.load(open(path)) if os.path.exists(path) else [{
    "guid": "8711b158-b490-4d5f-84b5-80abddce3e79",
    "name": "Media Bar Card",
    "overview": "Media Bar as a rounded, Netflix-style card with an artwork-coloured glow",
    "description": "A fork of Media Bar (MakD, IAmParadox27) whose default layout puts the featured slideshow on a rounded card lined up with the home rows, with a soft glow of each title's colours behind it, Play / More info buttons and previous / next buttons beside the card. Needs File Transformation. Uninstall the original Media Bar first.",
    "owner": "diverseau",
    "category": "General",
    "imageUrl": "https://raw.githubusercontent.com/diverseau/jellyfin-plugin-media-bar-card/main/src/logo.png",
    "versions": [],
}]
entry = {
    "version": version,
    "changelog": "%s/releases/tag/%s" % (repo, tag),
    "targetAbi": abi,
    "sourceUrl": "%s/releases/download/%s/%s" % (repo, tag, zipname),
    "checksum": checksum,
    "timestamp": timestamp,
    # File Transformation
    "dependencies": ["5e87cc92-571a-4d8d-8d98-d2d4147f9f90"],
}
plugin = manifest[0]
plugin["versions"] = [entry] + [v for v in plugin["versions"] if v["version"] != version]
with open(path, "w") as f:
    json.dump(manifest, f, indent=2)
    f.write("\n")
print("Updated manifest.json for", version)
PY
