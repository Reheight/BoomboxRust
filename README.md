# BoomboxRust
Allows you to change the station to any boombox via command.

## Commands
/stations - Shows you a detailed list of stations to use with your BoomBox.

/station <number> - Will play a selected station from the stations list.

/boombox <url> - Will play a custom audio source on the BoomBox.

| :boom: DANGER                                                                                                                                                                  |
|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| The /boombox URL feature should only be shared with trusted members as it can open up an attack vector and expose all of your members to attacks such as DDoS and IP scraping. |
  
## Permissions
boombox.stationsuse - (/stations, /station)
  
boombox.customurluse - (/boombox)
  
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
  }
}
```
| :warning: WARNING                                                              |
|:-------------------------------------------------------------------------------|
| The whitelist feature limits the domains usable for the /boombox <URL> feature |
