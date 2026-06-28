using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : UIPanel
{
    public Button cameraButton1;
    public Button cameraButton2;
    public Button cameraButton3;
    public RawImage cameraViewImage;
    public RenderTexture cameraView1;
    public RenderTexture cameraView2;
    public RenderTexture cameraView3;

    private bool isBound = false;

    public override void Show()
    {
        base.Show();
        if (!isBound)
        {
            BindCameraButtons();
            isBound = true;
        }
        SwitchCamera(1);
    }

    private void BindCameraButtons()
    {
        cameraButton1?.onClick.AddListener(() => SwitchCamera(1));
        cameraButton2?.onClick.AddListener(() => SwitchCamera(2));
        cameraButton3?.onClick.AddListener(() => SwitchCamera(3));
    }

    private void SwitchCamera(int cameraIndex)
    {
        if (cameraViewImage == null) return;

        RenderTexture targetTexture = cameraIndex switch
        {
            1 => cameraView1,
            2 => cameraView2,
            3 => cameraView3,
            _ => cameraView1
        };

        cameraViewImage.texture = targetTexture;
    }
}
