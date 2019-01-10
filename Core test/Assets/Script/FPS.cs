using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    string LOG_FILE_PATH = @"/log/";
    FileStream fs;
    StreamWriter sw;
    protected const float updateInterval = 0.5f;
    public bool displayFPS = true;
    protected int framesCount;
    protected int totalframesCount = 0;
    protected float framesTime;
    protected float totalframesTime = 0;
    protected Canvas canvas;
    protected Text text;
    void Awake()
    {


        fs = new FileStream(Application.streamingAssetsPath + LOG_FILE_PATH + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".txt", FileMode.Create);
        sw = new StreamWriter(fs);
        text = GetComponent<Text>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        framesCount++;
        totalframesCount++;
        framesTime += Time.unscaledDeltaTime;
        totalframesTime += Time.unscaledDeltaTime;

        if (framesTime > updateInterval)
        {
            if (text != null)
            {
                if (displayFPS)
                {
                    float fps = framesCount / framesTime;
                    text.text = string.Format("{0:F2} FPS", fps);
                    if (totalframesCount <= 1000 && framesTime != 0)
                    {
                        sw.WriteLine(fps);
                    }
                }
                else
                {
                    text.text = "";
                }
            }
            framesCount = 0;
            framesTime = 0;
        }
        if (totalframesCount == 1000)
        {
            sw.WriteLine(totalframesTime);
            print("done!");
        }
    }
    private void OnDestroy()
    {

        sw.Flush();
        sw.Close();
        fs.Close();
    }
}
