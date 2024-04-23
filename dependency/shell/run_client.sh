#!/usr/local

# 该代码暂时弃用，请使用run.sh

python_dir=/usr/local/PlayerCode/CAPI/python/PyAPI
python_main_dir=/usr/local/PlayerCode/CAPI/python
playback_dir=/usr/local/playback

for k in {1..2}
do
    pushd /usr/local/team$k

    for i in {1..5}
    do
        j=$((i - 1))
        if [ -f "./player$i.py" ]; then
            cp -r $python_main_dir $python_main_dir$i
            cp -f ./player$i.py $python_main_dir$i/PyAPI/AI.py
            nice -0 python3 $python_main_dir$i/PyAPI/main.py -I 127.0.0.1 -P 8888 -p $j > $playback_dir/team$k-player$j.log 2>&1 &
        elif [ -f "./capi$i" ]; then
            nice -0 ./capi$i -I 127.0.0.1 -P 8888 -p $j > $playback_dir/team$k-player$j.log 2>&1 &
        else
            echo "ERROR. $i is not found."
        fi
    done

    popd
done
