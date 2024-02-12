using System.Collections;
using UnityEngine;
using System;

public static class SaveLoadManager
{
    public static void SettingsSave(Settings settings)
    {
        PlayerPrefs.SetFloat("VolumeEffects", settings.EffectsVol);
        PlayerPrefs.SetFloat("VolumeMusic", settings.MusicVol);
        PlayerPrefs.SetFloat("VolumeAmbient", settings.AmbientVol);
        PlayerPrefs.SetFloat("SoundToggle", Convert.ToSingle(settings.SoundEnabled));
    }

    public static Settings SettingsLoad()
    {
        Settings settings = new();
        settings.EffectsVol = PlayerPrefs.GetFloat("VolumeEffects", 1f);
        settings.MusicVol = PlayerPrefs.GetFloat("VolumeMusic", 1f);
        settings.AmbientVol = PlayerPrefs.GetFloat("VolumeAmbient", 1f);
        settings.SoundEnabled = Convert.ToBoolean(PlayerPrefs.GetFloat("SoundToggle", 1f));

        return settings;
    }
}