<h1 align="center">Media Bar Card</h1>
<p align="center">A fork of <a href="https://github.com/IAmParadox27/jellyfin-plugin-media-bar">Media Bar</a> that shows the featured slideshow as a rounded, streaming-service style card.</p>
<p align="center">
	<img alt="Logo" src="src/logo.png" width="200" />
</p>

## What's different from Media Bar

Everything Media Bar does (random or listed titles, trailers, filters, the gear menu, plugin settings) works the same. This fork adds a **Card** layout and makes it the default:

- **A rounded card** lined up with the home rows instead of a full-screen backdrop. It sits just under the header, wherever your theme puts it, and the rows start just under the card.
- **An ambient glow** behind the card, built from each title's own artwork colours. It changes with every slide and costs no extra downloads (it uses the blur preview Jellyfin already sends).
- **Play** and **More info** pill buttons, and a clean `Genre • Year • Runtime • Rating` line.
- **Previous / next** circle buttons beside the card (on the card's edges when the window is narrow).
- **Trailers** play inside the card's rounded frame.
- Phones and short windows keep Media Bar's marquee layout.

You can still switch to Marquee, Plate or Classic from the gear menu on the bar.

It also fixes some Media Bar issues:

- The "TV show" limit in the plugin settings is now passed to the slideshow (it was sent under a name the slideshow never read).
- Custom fresh / rotten tomato icons set in the plugin settings now reach the slideshow (they were written into the IMDb icon's slot).
- It only ever edits the real `index.html`, never Jellyfin's script files that happen to have "index-html" in their names.

## Install

**Requirements:** Jellyfin **10.11.11** and the [File Transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation) plugin.

1. **Uninstall the original Media Bar first.** Both draw the same slideshow, so having both gives you two bars.
2. In Jellyfin go to **Dashboard → Plugins → Manage repositories → +** and add:
   ```
   https://raw.githubusercontent.com/diverseau/jellyfin-plugin-media-bar-card/main/manifest.json
   ```
   File Transformation comes from IAmParadox27's repository: `https://www.iamparadox.dev/jellyfin/plugins/manifest.json`.
3. Install **Media Bar Card** from the catalogue and restart Jellyfin.
4. Hard-refresh the browser (Ctrl+Shift+R).

If you added custom CSS earlier to make Media Bar look like a card, remove it from **Dashboard → General → Branding**.

If a browser had picked a different layout from the gear menu before, that browser keeps its choice. Pick **Card** in the gear menu to switch it.

## Building

```
scripts/package.sh
```

This builds against Jellyfin 10.11.11, zips the plugin into `artifacts/` and updates `manifest.json`.

## Credits and license

Media Bar is the work of **[MakD](https://github.com/MakD/Jellyfin-Media-Bar)** and its contributors, packaged as a Jellyfin plugin by **[IAmParadox27](https://github.com/IAmParadox27/jellyfin-plugin-media-bar)**. Earlier inspiration came from [BobHasNoSoul](https://github.com/BobHasNoSoul/jellyfin-featured), [SethBacon](https://forum.jellyfin.org/u-sethbacon) and [TedHinklater](https://github.com/tedhinklater/Jellyfin-Featured-Content-Bar). This fork only adds the card layout and the fixes above.

The code is under the original [DBAD license](LICENSE.md), unchanged. Problems with the card layout belong here. Anything that also happens in the original Media Bar belongs upstream.
