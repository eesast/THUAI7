#!/usr/local
python_dir=/usr/local/PlayerCode/CAPI/python/PyAPI
python_main_dir=/usr/local/PlayerCode/CAPI/python
playback_dir=/usr/local/playback
map_dir=/usr/local/map
mkdir -p $playback_dir

# initialize
if [[ "${MODE}" == "ARENA" ]]; then
    MODE=0
elif [[ "${MODE}" == "COMPETITION" ]]; then
    MODE=1
fi

# set default value
: "${TEAM_SEQ_ID:=0}"
: "${TEAM_LABELS:=1:2}"
: "${TEAM_LABEL:=1}"
: "${EXPOSED=1}"
: "${MODE=0}"
: "${TIME=10}"
: "${CONNECT_IP=172.17.0.1}"

get_current_team_label() {
    if [ $TEAM_SEQ_ID -eq $2 ]; then
        echo "find current team label: $1"
        current_team_label=$1
    fi
}

read_array() {
    callback=$1
    echo "read array: set callback command: $callback"
    
    IFS=':' read -r -a fields <<< "$2"

    count=0 # loop count

    for field in "${fields[@]}"
    do
        echo "parse field: $field"
        param0=$field
        
        # call command
        run_command="$callback $param0 $count"
        echo "Call Command: $run_command"
        $run_command

        count=$((count+1))
    done
}

if [ "$TERMINAL" = "SERVER" ]; then
    map_path=$map_dir/$MAP_ID.txt
    if [ $EXPOSED -eq 1 ]; then
        nice -10 ./Server --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $TIME --mode $MODE --mapResource $map_path --url $URL --token $TOKEN --fileName $playback_dir/video --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
        server_pid=$!
    else
        nice -10 ./Server --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $TIME --mode $MODE --mapResource $map_path --notAllowSpectator --url $URL --token $TOKEN --fileName $playback_dir/video --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
        server_pid=$!
    fi
    echo "server pid: $server_pid"
    ls $playback_dir

    sleep 10

    if [ -f $playback_dir/start.lock ]; then
        ps -p $server_pid
        while [ $? -eq 0 ]
        do
            sleep 1
            ps -p $server_pid > /dev/null 2>&1
        done
        result=$(cat /usr/local/playback/result.json)
        score0=$(echo "$result" | grep -oP '(?<="Team1":)\d+')
        score1=$(echo "$result" | grep -oP '(?<="Team2":)\d+')
        curl $URL -X PUT -H "Content-Type: application/json" -H "Authorization: Bearer $TOKEN" -d '{"result":[{"team_id":0, "score":'${score0}'}, {"team_id":1, "score":'${score1}'}], "mode":'${MODE}'}'> $playback_dir/send.log 2>&1
        touch $playback_dir/finish.lock
        echo "Finish"
    else
        echo "Failed to start game."
        touch $playback_dir/finish.lock
        touch temp.lock
        mv -f temp.lock $playback_dir/video.thuaipb
        kill -9 $server_pid
    fi

elif [ "$TERMINAL" = "CLIENT" ]; then
    echo "Client Mode! Team Label data - $TEAM_LABELS"
    
    # parse team label name
    read_array get_current_team_label $TEAM_LABELS

    if [ "$current_team_label" = "1" ]; then
        k=1
    elif [ "$current_team_label" = "2" ]; then
        k=2
    else
        echo "Error: Invalid Team Label"
        exit
    fi
    pushd /usr/local/code
    for i in {0..4}
    do
        if [ $i -eq 0 ]; then
            code_name=Home
            if [ -f "./$code_name.py" ]; then
                cp -r $python_main_dir $python_main_dir$i
                cp -f ./$code_name.py $python_main_dir$i/PyAPI/AI.py
                nice -0 python3 $python_main_dir$i/PyAPI/main.py -I
            elif [ -f "./$code_name" ]; then
                nice -0 ./$code_name -I 127.0.0.1 -P 8888 -p $j > $playback_dir/team$k-player$j.log 2>&1 &
            else
                echo "ERROR. $code_name is not found."
            fi
        else
            j=$i
            code_name=Ship$i
            if [ -f "./$code_name.py" ]; then
                cp -r $python_main_dir $python_main_dir$i
                cp -f ./$code_name.py $python_main_dir$i/PyAPI/AI.py
                nice -0 python3 $python_main_dir$i/PyAPI/main.py -I 127.0.0.1 -P 8888 -p $j > $playback_dir/team$k-player$j.log 2>&1 &
            elif [ -f "./$code_name" ]; then
                nice -0 ./$code_name -I 127.0.0.1 -P 8888 -p $j > $playback_dir/team$k-player$j.log 2>&1 &
            else
                echo "ERROR. $code_name is not found."
            fi
        fi
    done
    popd
else
    echo "VALUE ERROR: TERMINAL is neither SERVER nor CLIENT."
fi