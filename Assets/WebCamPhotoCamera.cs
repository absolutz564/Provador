using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class WebCamPhotoCamera : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage rawImage;
    public Text MyText;

    public Quaternion baseRotation;

    public static WebCamPhotoCamera Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        webCamTexture = null;
        WebCamDevice[] wdcs = WebCamTexture.devices;
        for (int n = 0; n < wdcs.Length; ++n)
        {
            if (wdcs[n].isFrontFacing)
            {
                webCamTexture = new WebCamTexture(wdcs[n].name);
            }
        }

        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture();
        }


        rawImage.texture = webCamTexture; //Add Mesh Renderer to the GameObject to which this script is attached to
        baseRotation = transform.rotation;

        webCamTexture.Play();

    }
    void Update()
    {
        //transform.rotation = baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.up);
    }
    public void Photo()
    {
        StartCoroutine(TakePhoto());
    }
    IEnumerator TakePhoto()  // Start this Coroutine on some button click
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes(Application.persistentDataPath + "photo.png", bytes);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "photo.png"));

        rawImage.texture = tex;
        webCamTexture.Stop();
        MyText.text = "imagem capturada";
    }
}