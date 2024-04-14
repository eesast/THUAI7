#!/usr/bin/env bash

python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 0 -t 0 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 1 -t 0 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 2 -t 0 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 3 -t 0 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 4 -t 0 -o&

python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 0 -t 1 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 1 -t 1 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 2 -t 1 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 3 -t 1 -o&
python ./CAPI/python/PyAPI/main.py -I 127.0.0.1 -P 8888 -p 4 -t 1 -o&