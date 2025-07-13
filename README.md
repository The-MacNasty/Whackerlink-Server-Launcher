# Whackerlink-Server-Launcher
Whackerlink Server Launcher Application


The Whackerlink Server Launcher, is a application that can be conbined with the Whackerlink Server "Whackerlink_v4" from the Whackerlink FiveM Radio creators.

Installation:
              1. Download the Files from this Repos releases as a zip.
              2. Unzip the files on your Machine that is hosting the Whackerlink Server
              3. Move or copy and paste the files into the same folder as your whackerlinkserver.exe.
              5. make sure your server config is in a folder called configs. (it can be named anything)

Source: If you are downloading the source code, Im assuming you know a little bit about C#. This is my first App I have ever built. I am still learning about coding and building resources so I can't help a whole lot on anything else. If you have questions about this App ask away I will try my best to help!

FILE STRUCTURE:
              Your file structure should look like this. (should work directly with an untouched release of WhackerLinkServer_R01.14.00.)

                Whackerlinkserver(main folder)
                        |__all the whackerlinkserver files
                        |__whackerlinkserver.exe
                        |__WhackerlinkServerLauncher.deps.json
                        |__WhackerLinkServerLauncher.dll
                        |__WhackerLinkServerLauncher.exe
                        |__WhackerLinkServerLauncher.runtimeconfig.json
                        |__configs(folder)
                                |__config.yml (can be named anything other than the auth and rid configs.)
                                |__auth_keys.yml
                                |__rid_acl.yml
                                
IDEOLOGY:
          The idea behind this app was to make it easier on the end user of Whackerlink, as someone who self-hosted the whackerlink server I did not like the hassel of having to open a
          Powershell window within file explorer and type in or copy and paste a command line each time I needed to start the W.L. server. This wasn't very oftem but there was time like when my
          machine needed to update and restart or there was an outage of some sort, just any reason the server instance would be terminated. So I built this application to make it easier to start.
          Just a doubleclick of the WhackerLinkSeerverLauncher.exe will do everything for you, it will also tell you which config you are using as it searches the config folder for one. it also logs
          in the logs folder. Thanks for looking and I hope it helps you out in the future!
