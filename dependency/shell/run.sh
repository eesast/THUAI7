#!/usr/local
python_dir=/usr/local/PlayerCode/CAPI/python/PyAPI
python_main_dir=/usr/local/PlayerCode/CAPI/python
playback_dir=/usr/local/output
map_dir=/usr/local/map
mkdir -p $playback_dir

# initialize
if [[ "${MODE}" == "ARENA" ]]; then
    MODE_NUM=2
elif [[ "${MODE}" == "COMPETITION" ]]; then
    MODE_NUM=1
fi
# set default value
: "${TEAM_SEQ_ID:=0}"
: "${TEAM_LABELS:=Team:Team}"
: "${TEAM_LABEL:=Team}"
: "${EXPOSED=1}"
: "${MODE_NUM=2}"
: "${GAME_TIME=600}"
: "${CONNECT_IP=172.17.0.1}"

# get_current_team_label() {
#     if [ $TEAM_SEQ_ID -eq $2 ]; then
#         echo "find current team label: $1"
#         current_team_label=$1
#     fi
# }

# read_array() {
#     callback=$1
#     echo "read array: set callback command: $callback"
    
#     IFS=':' read -r -a fields <<< "$2"

#     count=0 # loop count

#     for field in "${fields[@]}"
#     do
#         echo "parse field: $field"
#         param0=$field
        
#         # call command
#         run_command="$callback $param0 $count"
#         echo "Call Command: $run_command"
#         $run_command

#         count=$((count+1))
#     done
# }

function retry_command {
    local command="$1"
    local max_attempts=5
    local attempt_num=1
    local sleep_seconds=10

    while [ $attempt_num -le $max_attempts ]; do
        echo "Attempt $attempt_num / $max_attempts to run command: $command"

        eval $command &
        local PID=$!

        sleep $sleep_seconds

        if kill -0 $PID 2>/dev/null; then
            echo "Failed to connect to server. Retrying..."
            ((attempt_num++))
        else
            echo "Connected to server successfully."
            return 0
        fi
    done

    echo "Failed to connect to server after $max_attempts attempts."
    return 1
}



if [ "$TERMINAL" = "SERVER" ]; then
    map_path=$map_dir/$MAP_ID.txt
    if [ $EXPOSED -eq 1 ]; then
        nice -10 ./Server --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $GAME_TIME --mode $MODE_NUM --mapResource $map_path --url $SCORE_URL --token $TOKEN --fileName $playback_dir/playback --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
        server_pid=$!
    else
        nice -10 ./Server --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $GAME_TIME --mode $MODE_NUM --mapResource $map_path --notAllowSpectator --url $SCORE_URL --token $TOKEN --fileName $playback_dir/playback --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
        server_pid=$!
    fi

    echo "server pid: $server_pid"
    ls $playback_dir

    echo "SCORE URL: $SCORE_URL"
    echo "FINISH URL: $FINISH_URL"

    echo "waiting..."
    sleep 60 # wait connection time
    echo "watching..."

    if [ ! -f $playback_dir/start.lock ]; then
        echo "Failed to start game."
        touch temp.lock
        mv -f temp.lock $playback_dir/playback.thuaipb
        kill -9 $server_pid
        finish_payload='{"status": "Crashed", "scores": [0, 0]}'
        curl $FINISH_URL -X POST -H "Content-Type: application/json" -H "Authorization: Bearer $TOKEN" -d "${finish_payload}" > $playback_dir/send.log 2>&1
    else
        echo "Game is started."
        ps -p $server_pid
        while [ $? -eq 0 ]
        do
            sleep 1
            ps -p $server_pid > /dev/null 2>&1
        done
        echo "Server is down."
    fi

elif [ "$TERMINAL" = "CLIENT" ]; then
    echo "Client Mode! Team Label data - $TEAM_LABELS"

    k=$TEAM_SEQ_ID
    pushd /usr/local/code
        for i in {0..4}
        do
            if [ $i -eq 0 ]; then
                code_name=Team
                if [ -f "./$code_name.py" ]; then
                    echo "find ./$code_name.py"
                    cp -r $python_main_dir $python_main_dir$i
                    cp -f ./$code_name.py $python_main_dir$i/PyAPI/AI.py
                    command="nice -0 python3 $python_main_dir$i/PyAPI/main.py -I $CONNECT_IP -P $PORT -t $k -p $i > $playback_dir/team$k-$code_name.log 2>&1 &"
                    retry_command "$command" > $playback_dir/client$k-$i.log &
                elif [ -f "./$code_name" ]; then
                    echo "find ./$code_name"
                    command="nice -0 ./$code_name -I $CONNECT_IP -P $PORT -t $k -p $i > $playback_dir/team$k-$code_name.log 2>&1 &"
                    retry_command "$command" > $playback_dir/client$k-$i.log &
                else
                    echo "ERROR. $code_name is not found."
                fi
            else 
                code_name=Ship$i
                if [ -f "./$code_name.py" ]; then
                    echo "find ./$code_name.py"
                    cp -r $python_main_dir $python_main_dir$i
                    cp -f ./$code_name.py $python_main_dir$i/PyAPI/AI.py
                    command="nice -0 python3 $python_main_dir$i/PyAPI/main.py -I $CONNECT_IP -P $PORT -t $k -p $i > $playback_dir/team$k-$code_name.log 2>&1 &"
                    retry_command "$command" > $playback_dir/client$k-$i.log &
                elif [ -f "./$code_name" ]; then
                    echo "find ./$code_name"
                    command="nice -0 ./$code_name -I $CONNECT_IP -P $PORT -t $k -p $i > $playback_dir/team$k-$code_name.log 2>&1 &"
                    retry_command "$command" > $playback_dir/client$k-$i.log &
                else
                    echo "ERROR. $code_name is not found."
                fi
            fi
        done
        sleep $((GAME_TIME+90))
    popd
else
    echo "VALUE ERROR: TERMINAL is neither SERVER nor CLIENT."
fi
