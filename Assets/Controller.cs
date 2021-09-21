using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using epoching.easy_qr_code;
public class Controller : MonoBehaviour
{
    public GameObject LoginObject;
    public GameObject CameraObject;

    public InputField InputName;
    public InputField InputEmail;

    public Button LoginButton;

    public RawImage FaceImage;
    public GameObject MaleMenu;
    public GameObject FemaleMenu;


    public int FileCounter = 0;
    public Camera Cam;

    public GameObject QRCodeMenu;

    public RawImage PicImage;

    public ImageUploader imageUploader;

    public Generate_qr_code generate_Qr_Code;
    public void SavePrintToQrCode(Texture image)
    {
        QRCodeMenu.SetActive(true);
        PicImage.texture = image;
    }
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        Texture texture = ScreenCapture.CaptureScreenshotAsTexture();
        // do something with texture

        // cleanup
        SavePrintToQrCode(texture);
        yield return new WaitForEndOfFrame();
        imageUploader.SendPictureToServer(texture as Texture2D);

        generate_Qr_Code.url = imageUploader.url + imageUploader.movedFolder + imageUploader.endpoint;
        generate_Qr_Code.on_generate_btn();
        //UnityEngine.Object.Destroy(texture);
    }


    public void CamCapture()
    {
        StartCoroutine(RecordFrame());
        //PicImage.texture = ScreenCapture.CaptureScreenshotAsTexture();
        //RenderTexture currentRT = RenderTexture.active;
        //RenderTexture.active = Cam.targetTexture;

        //Cam.Render();

        //Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        //Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        //Image.Apply();
        //RenderTexture.active = currentRT;

        //SavePrintToQrCode(Image);

        //var Bytes = Image.EncodeToPNG();
        //Destroy(Image);

        //File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + FileCounter + ".png", Bytes);
        //FileCounter++;
    }

    public void SkipLogin()
    {
        LoginObject.SetActive(false);
        CameraObject.SetActive(true);
    }

    public void EnableCloset()
    {
        MaleMenu.SetActive(true);
        FaceImage.texture = WebCamPhotoCamera.Instance.rawImage.texture;
    }

    public void CheckFields()
    {

    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

}
