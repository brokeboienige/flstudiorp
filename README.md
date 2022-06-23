# FL Studio Rich Presence
A custom non-official Discord rich presence for Fruity Loops Studio.

If you have any problem you can always open a [issue](https://github.com/brokeboienige/flstudiorp/issues/new).

## Features
| Feature | Status |
| ------------- | ------------- |
| Show project name | ✅  |
| Show project BPM | ✅  |
| Show project timestamp | ✅  |
| Show master waveform | ✅  |

## Notes
- Actually it works just for the `FL64.exe`. I'm currently working on the 32 bit version.
- Tested on FL Studio v20.7.2 only, let me know if it wont work for your FL version.
## Installing
- Download the most recent build [here](https://github.com/brokeboienige/flstudiorp/releases/latest) or build it yourself as described on [how to build](#building)
- Grab the `flstudiorp.exe` file and copy it to somewhere you like.
- Right-click it and create a shortcut.
- Press Win+R and type `shell:startup`.
- Copy the shortcut to the folder that opened and thats it, you are all set!
- This makes the `flstudiorp.exe` start with Windows and stay aways watching if FL is open or not.
- Dont worry, it doens't take much RAM or CPU.
## Building
Install the dependencies:
```
pip install -r requirements.txt
```

Build using pyinstaller:
```
pyinstaller --noconsole --onefile flstudiorp.py --icon=flstudiorp.ico --version-file=flstudiorp-versioninfo.txt
```

Now install as described on [how to install](#installing)
