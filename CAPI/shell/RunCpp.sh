#!/usr/bin/env bash

./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 0 -t 0 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 1 -t 0 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 2 -t 0 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 3 -t 0 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 4 -t 0 -o&

./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 0 -t 1 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 1 -t 1 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 2 -t 1 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 3 -t 1 -o&
./CAPI/cpp/build/capi -I 127.0.0.1 -P 8888 -p 4 -t 1 -o&