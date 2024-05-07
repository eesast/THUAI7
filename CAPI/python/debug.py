from typing import List
import subprocess
import time

commands = [
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 0 -t 0 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 1 -t 0 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 2 -t 0 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 3 -t 0 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 4 -t 0 -o",

    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 0 -t 1 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 1 -t 1 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 2 -t 1 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 3 -t 1 -o",
    "python PyAPI/main.py -I 127.0.0.1 -P 8888 -p 4 -t 1 -o",
]

processes: List[subprocess.Popen] = []
for cmd in commands:
    processes.append(subprocess.Popen(cmd, shell=True))
    time.sleep(0.5)

for process in processes:
    process.wait()
