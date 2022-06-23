# developed by brokeboienige
# greetz r1serclub n pantufas snorlax de gang <3
import datetime
from pypresence import Presence
import time
import pymem
import logging
logging.basicConfig(filename='flstudiorp-error.log', level=logging.DEBUG, 
                    format='%(asctime)s %(levelname)s %(name)s %(message)s', filemode="w",)
logger=logging.getLogger(__name__)

app_id = "988310374002589716"

#https://stackoverflow.com/questions/70618975/python-get-windowtitle-from-process-id-or-process-name
import win32gui
import win32process
import ctypes

EnumWindows = ctypes.windll.user32.EnumWindows
EnumWindowsProc = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.POINTER(ctypes.c_int), ctypes.POINTER(ctypes.c_int))
GetWindowText = ctypes.windll.user32.GetWindowTextW
GetWindowTextLength = ctypes.windll.user32.GetWindowTextLengthW
IsWindowVisible = ctypes.windll.user32.IsWindowVisible
def get_hwnds_for_pid(pid):
    def callback(hwnd, hwnds):
        #if win32gui.IsWindowVisible(hwnd) and win32gui.IsWindowEnabled(hwnd):
        _, found_pid = win32process.GetWindowThreadProcessId(hwnd)

        if found_pid == pid:
            hwnds.append(hwnd)
        return True
    hwnds = []
    win32gui.EnumWindows(callback, hwnds)
    return hwnds 

def getWindowTitleByHandle(hwnd):
    length = GetWindowTextLength(hwnd)
    buff = ctypes.create_unicode_buffer(length + 1)
    GetWindowText(hwnd, buff, length + 1)
    return buff.value

#https://www.theamplituhedron.com/articles/How-to-replicate-the-Arduino-map-function-in-Python-for-Raspberry-Pi/
def _map(x, in_min, in_max, out_min, out_max):
    return int((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min)

def main():
    while True:
        try:
            pm = pymem.Pymem('FL64.exe')
            break
        except:
            print("Waiting for FL Studio")
            time.sleep(5)
    while True:
        try:
            RPC = Presence(app_id)
            RPC.connect()
            break
        except:
            print("Waiting for Discord")
            time.sleep(5)
    start = int(time.time())
    last = ""
    while True:
        try:
            hwnds = get_hwnds_for_pid(pm.process_id)
            for hwnd in hwnds:
                if IsWindowVisible(hwnd):
                    title = getWindowTitleByHandle(hwnd)
                    break
            if title.find("-") != -1:
                title = title.split("-")[0]+" - "
            else:
                title = "untitled.flp - "
            base = pymem.process.module_from_name(pm.process_handle, f"_FLEngine_x64.dll").lpBaseOfDll
            
            ms = int(pm.read_int(base + 0xDDF5D8))
            bpm = str(pm.read_int(base + 0xC9DF88))
            dt = datetime.datetime.fromtimestamp(ms/1000.0)
            master = int(pm.read_int(base + 0xD77330))

            if bpm[-3:] == "000":
                bpm = bpm[:-3]
            else:
                bpm = f"{bpm[:-3]}.{bpm[-3:]}"

            if master < 1000000000:
                master = 1000000000
            master_wave = _map(master, 1000000000, 1070000000, 0, 20)
            wave = "█"*master_wave
            wave = wave.ljust(20, " ")
            wave = wave [:15]
            if not "░▒▓"+wave+f"{title}{bpm} BPM {dt.minute:02d}:{dt.second:02d}" == last:
                last = "░▒▓"+wave+f"{title}{bpm} BPM {dt.minute:02d}:{dt.second:02d}"
                RPC.update(large_image = "fl", pid = pm.process_id, state = "░▒▓"+wave, details = f"{title}{bpm} BPM | {dt.minute:02d}:{dt.second:02d}", large_text = "FL Studio RP - Developed by @brokeboienige", start=start)
        except Exception as e:
            logger.error(e)
            RPC.close()
            print("FL or Discord was closed Waiting...")
            main()
            break
main()