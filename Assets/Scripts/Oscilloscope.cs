using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(GUITexture))]
public class Oscilloscope : MonoBehaviour
{


    public int width = 4096; // texture width 
    public int height = 4096; // texture height 
    public Color backgroundColor = Color.black;
    public Color waveformColor = Color.green;
    public int size = 4096; // size of sound segment displayed in texture

    public int refreshrate;

    private Color[] blank; // blank image array 
    private Texture2D texture;
    private float[] samplesX; // audio samples array
    private float[] samplesY;

    IEnumerator Start()
    {

        // create the samples array 
        samplesX = new float[size];
        samplesY = new float[size];


        // create the texture and assign to the guiTexture: 
        texture = new Texture2D(width, height);

        GetComponent<GUITexture>().pixelInset.Set(Screen.width / 2, Screen.height / 2, 0, 0);
        GetComponent<GUITexture>().texture = texture;

        // create a 'blank screen' image 
        blank = new Color[width * height];

        for (int i = 0; i < blank.Length; i++)
        {
            blank[i] = backgroundColor;
        }

        // refresh the display each 100mS 
        while (true)
        {
            GetCurWave();
            yield return new WaitForSeconds(1/refreshrate);
        }
    }

    void GetCurWave()
    {
        // clear the texture 
        texture.SetPixels(blank, 0);

        // get samples from channel 0 (left) 
        GetComponent<AudioSource>().GetOutputData(samplesX, 0);
        GetComponent<AudioSource>().GetOutputData(samplesY, 1);

        // draw the waveform 
        for (int i = 0; i < size; i++)
        {
            float pX = height * (samplesX[i] + 1f) / 2f;
            float pY = height * (samplesY[i] + 1f) / 2f;
            texture.SetPixel(Mathf.RoundToInt(pX), Mathf.RoundToInt(pY), waveformColor);
        } // upload to the graphics card 

        texture.Apply();
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        GetComponent<Image>().sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
    }
}