using Playback;
using Protobuf;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlaybackController : MonoBehaviour
{
    // Start is called before the first frame update

    public static int studentNum = 4;
    private static int classroomNum = 10;
    public static int propNum = 10;
    public static int hiddenGateNum = 4;

    byte[] bytes = null;

    IEnumerator WebReader(string uri)
    {
        //if (!fileName.EndsWith(PlayBackConstant.ExtendedName))
        //{
        //    fileName += PlayBackConstant.ExtendedName;
        //}
        //var uri = new Uri(Path.Combine(Application.streamingAssetsPath, fileName));
        //Debug.Log(uri.AbsoluteUri);

        //UnityWebRequest request = UnityWebRequest.Get(uri.AbsoluteUri);
        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.timeout = 5;
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            filename = null;
            playSpeed = 1;
            isMap = true;
            isInitial = false;
            studentScore = trickerScore = 0;
            SceneManager.LoadScene("Playback");
            yield break;
        }
        bytes = request.downloadHandler.data;
    }

    void Start()
    {
        filename = "C:\\Users\\陈宇瀚\\Downloads\\playback.thuaipb";
        StartCoroutine(WebReader(filename));
        timer = frequency;
        isMap = true;
        isInitial = false;
        //StudentController_Playback.studentCount = 0;
        //ClassroomProgress_Playback.ClassroomCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime * playSpeed;

        try
        {
            if (timer < 0 && bytes != null)
            {
                if (reader == null)
                {
                    try
                    {
                        StopAllCoroutines();
                        reader = new MessageReader(bytes);
                    }
                    catch (FileFormatNotLegalException)
                    {
                        filename = null;
                        playSpeed = -1;
                        isMap = true;
                        isInitial = false;
                        SceneManager.LoadScene("Playback");
                    }
                    Debug.Log("reader created");
                }
                timer = frequency;
                var responseVal = reader.ReadOne();
                Debug.Log($"{responseVal}");
                if (responseVal == null)
                {
                    filename = null;
                    playSpeed = -1;
                    isMap = true;
                    isInitial = false;
                    SceneManager.LoadScene("GameEnd");
                }
                else if (isMap)
                {
                    map = responseVal.ObjMessage[0].MapMessage;
                    isMap = false;
                }
                else if (!isInitial)
                {
                    Receive(responseVal);
                    // for (int i = 0; i < studentNum; i++)
                    // {
                    //     switch (Student[i].StudentType)
                    //     {
                    //         case StudentType.Athlete:
                    //             Instantiate(student1, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student1_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         case StudentType.StraightAStudent:
                    //             Instantiate(student2, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student2_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         case StudentType.Teacher:
                    //             Instantiate(student3, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student3_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         case StudentType.Sunshine:
                    //             Instantiate(student4, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student4_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         case StudentType.TechOtaku:
                    //             Instantiate(student5, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student5_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         case StudentType.Robot:
                    //             Instantiate(student6, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //             Instantiate(student6_icon, new Vector3(60f, -10f * i + 41f, 0), new Quaternion(0, 0, 0, 0));
                    //             break;
                    //         default: Instantiate(student1, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0)); break;
                    //     }
                    // }
                    // switch (tricker.TrickerType)
                    // {
                    //     case TrickerType.Assassin:
                    //         Instantiate(tricker1, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //         Instantiate(tricker1_icon, new Vector3(60f, 2.7f, 0), new Quaternion(0, 0, 0, 0));
                    //         break;
                    //     case TrickerType.ANoisyPerson:
                    //         Instantiate(tricker2, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //         Instantiate(tricker2_icon, new Vector3(60f, 2.7f, 0), new Quaternion(0, 0, 0, 0));
                    //         break;
                    //     case TrickerType.Klee:
                    //         Instantiate(tricker3, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //         Instantiate(tricker3_icon, new Vector3(60f, 2.7f, 0), new Quaternion(0, 0, 0, 0));
                    //         break;
                    //     case TrickerType.Idol:
                    //         Instantiate(tricker1, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0));
                    //         Instantiate(tricker1_icon, new Vector3(60f, 2.7f, 0), new Quaternion(0, 0, 0, 0));
                    //         break;
                    //     default: Instantiate(tricker1, new Vector3(0f, 0f, 10.0f), new Quaternion(0, 0, 0, 0)); break;
                    // }
                    isInitial = true;
                }
                else
                {
                    Receive(responseVal);
                }
            }

        }
        catch (NullReferenceException)
        {
            filename = null;
            playSpeed = 1;
            isMap = true;
            isInitial = false;
            SceneManager.LoadScene("Playback");
        }

    }

    private void Receive(MessageToClient message)
    {
        int studentCnt = 0;
        int classroomCnt = 0;
        //Debug.Log(message.ToString());
        time = message.AllMessage.GameTime;
        foreach (var messageOfObj in message.ObjMessage)
        {
            // switch (messageOfObj.MessageOfObjCase)
            // {
            //     case MessageOfObj.MessageOfObjOneofCase.StudentMessage:
            //         Student[studentCnt] = messageOfObj.StudentMessage; studentCnt++; break;
            //     case MessageOfObj.MessageOfObjOneofCase.TrickerMessage:
            //         tricker = messageOfObj.TrickerMessage; break;
            //     case MessageOfObj.MessageOfObjOneofCase.ClassroomMessage:
            //         classroom[classroomCnt] = messageOfObj.ClassroomMessage; classroomCnt++; break;
            //     case MessageOfObj.MessageOfObjOneofCase.PropMessage:
            //         break;
            //     case MessageOfObj.MessageOfObjOneofCase.HiddenGateMessage:
            //         break;
            //     default: break;
            // }
        }
    }

    public static string filename;
    MessageReader reader;
    float frequency = 0.05f;
    float timer;
    public static int playSpeed = 1;

    public static MessageOfMap map;
    // public static MessageOfStudent[] Student = new MessageOfStudent[studentNum];
    // public static MessageOfTricker tricker = new MessageOfTricker();
    // public static MessageOfClassroom[] classroom = new MessageOfClassroom[classroomNum];
    // public static MessageOfProp[] prop = new MessageOfProp[propNum];
    // public static MessageOfHiddenGate[] hiddenGate = new MessageOfHiddenGate[hiddenGateNum];
    public GameObject student1, student2, student3, student4, student5, student6;
    public GameObject tricker1;
    public GameObject tricker2;
    public GameObject tricker3;
    public GameObject student1_icon, student2_icon, student3_icon, student4_icon, student5_icon, student6_icon;
    public GameObject tricker1_icon, tricker2_icon, tricker3_icon;
    public static bool isMap;
    public static bool isInitial;
    public static int time;
    public static int studentScore;
    public static int trickerScore;
}