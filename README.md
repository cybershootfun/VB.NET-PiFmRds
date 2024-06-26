# VB.NET-PiFmRds
Using PiFmRds by Christophe Jacquet(https://github.com/ChristopheJacquet/PiFmRds), this application acts as an "SSH control panel" for your Raspberry Pi.
It allows you to change the frequency, program service (PS), and radio text (RT) settings. You can also ping your Raspberry Pi, connect to it, and start or stop PiFmRds.
Additionally, I have implemented a live temperature meter that can be started or stopped. The application also provides options to reboot or shut down your Raspberry Pi.

Features:

Change frequency, PS (8-character name), and RT settings
Ping your Raspberry Pi
Connect to the Raspberry Pi
Start/stop PiFmRds
Live temperature meter (start/stop)
Reboot or shut down the Raspberry Pi
Transfer audio files securely using SCP

Installation Guide:

Start by downloading PiFmRds from here(https://github.com/ChristopheJacquet/PiFmRds).
Next, acquire the main.py file, which can be found at python/main.py (for temperature monitoring).
Then, download the VB.NET project and customize the directories to fit your setup (including PiFmRds location, Python code location, etc.).
With all components in place and directories configured, you're all set to get started!

![image](https://github.com/cybershootfun/VB.NET-PiFmRds/assets/49350716/c08a3875-b517-4d0a-804b-0ca4e209b403)
![image](https://github.com/cybershootfun/VB.NET-PiFmRds/assets/49350716/0d6bfa17-eb80-432c-92b9-539db96d9a11)
![image](https://github.com/cybershootfun/VB.NET-PiFmRds/assets/49350716/e66afc6b-7fa5-48ed-96f7-cec647952d53)

Successfully tested on Raspberry Pi Zero W for transmitting.
Also, it works seamlessly with my mom's old radio for receiving :p
