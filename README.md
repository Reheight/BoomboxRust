# BoomboxRust
Allows you to change the station to any boombox via command.

## Commands
/stations - Shows you a detailed list of stations to use with your BoomBox.

/station <number> - Will play a selected station from the stations list.

/boombox <url> - Will play a custom audio source on the BoomBox.
  
/addstation "name" "URL" - Adds stations to the preset stations.

/removestation "name" "URL" - Removes stations from the preset stations.

| :boom: DANGER                                                                                                                                                                  |
|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| The /boombox URL feature should only be shared with trusted members as it can open up an attack vector and expose all of your members to attacks such as DDoS and IP scraping. |
  
## Permissions
boombox.stationsuse - (/stations, /station)
  
boombox.customurluse - (/boombox)
  
boombox.admin - Allows you to use any URL for /boombox and bypass the whitelist while also allowing you to use every command.
  
## Configuration
```json
{
  "Whitelist": true,
  "Whitelisted Domains": [
    "stream.zeno.fm"
  ],
  "Boombox Deployed Require Power": true,
  "Preset Stations": {
    "Country Hits": "http://crystalout.surfernetwork.com:8001/KXBZ_MP3",
    "Todays Hits": "https://rfcmedia.streamguys1.com/MusicPulsePremium.mp3",
    "Pop Hits": "https://rfcmedia.streamguys1.com/newpophitspremium.mp3"
  },
  "Deployed Boombox Never Decays": false,
  "Handheld Boombox Never Breaks": false,
  "Microphone Stand Never Breaks": false
}
```
| :warning: WARNING                                                              |
|:-------------------------------------------------------------------------------|
| The whitelist feature limits the domains usable for the /boombox <URL> feature |
