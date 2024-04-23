#!/usr/local

# 该代码暂时弃用，请使用run.sh

python_dir=/usr/local/PlayerCode/CAPI/python/PyAPI
python_main_dir=/usr/local/PlayerCode/CAPI/python
playback_dir=/usr/local/playback

if [ $EXPOSED -eq 1 ]; then
    nice -10 ./Server --ip 127.0.0.1 --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $TIME --mode $MODE --mapResource $MAP --url $URL --token $TOKEN --fileName $playback_dir/video --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
    server_pid=$!
else
    nice -10 ./Server --ip 127.0.0.1 --port 8888 --teamCount 2 --shipNum 4 --resultFileName $playback_dir/result --gameTimeInSecond $TIME --mode $MODE --mapResource $MAP --notAllowSpectator --url $URL --token $TOKEN --fileName $playback_dir/video --startLockFile $playback_dir/start.lock > $playback_dir/server.log 2>&1 &
    server_pid=$!
fi

if [ -f $playback_dir/start.lock ]; then
    ps -p $server_pid
    while [ $? -eq 0 ]
    do
        sleep 1
        ps -p $server_pid > /dev/null 2>&1
    done
    # result=$(cat /usr/local/playback/result.json)
    # score0=$(echo "$result" | grep -oP '(?<="Student":)\d+')
    # score1=$(echo "$result" | grep -oP '(?<="Tricker":)\d+')
    # curl $URL -X PUT -H "Content-Type: application/json" -H "Authorization: Bearer $TOKEN" -d '{"result":[{"team_id":0, "score":'${score0}'}, {"team_id":1, "score":'${score1}'}], "mode":'${MODE}'}'> $playback_dir/send.log 2>&1
    touch $playback_dir/finish.lock
    echo "Finish"
else
    echo "Failed to start game."
    touch $playback_dir/finish.lock
    touch temp.lock
    mv -f temp.lock $playback_dir/video.thuaipb
    kill -9 $server_pid
fi
