public class Gaming_SettingData
{
    private static Gaming_SettingData _ins;

    public static Gaming_SettingData _Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = new Gaming_SettingData();
            }
            return _ins;
        }
    }

    public float bgmVolume = 1f;
    public float audioVolume = 1f;
}
