using System.Collections;
using UnityEngine;

public class FPS_Counter : MonoBehaviour
{
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;
    public GUIStyle style;
 
    void OnEnable(){
        StartCoroutine(FPS());
    }
 
    private IEnumerator FPS() {
        for(;;){
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
           
            // Display it
            fps = string.Format("FPS: {0}" , Mathf.RoundToInt(frameCount / timeSpan));
        }
    }
 
    void OnGUI(){
        GUI.Label(new Rect(Screen.width / 2,30,200,70), fps, style);
    }
}
