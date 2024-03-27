using System.Collections;
using UnityEngine;
using System;

//Этот класс отвечает за сохрание и загрузку настроек
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

    public static void SaveProgresData(ProgressData progress)
    {
        PlayerPrefs.SetFloat("PositionX",progress.PlayerPosition.x);
        PlayerPrefs.SetFloat("PositionY", progress.PlayerPosition.y);
        PlayerPrefs.SetFloat("PositionZ", progress.PlayerPosition.z);
        PlayerPrefs.SetString("CompleteEncounters", progress.CompleteEncounters);
    }

    public static ProgressData LoadProgres()
    {
        ProgressData progress = new();
        progress.PlayerPosition = new Vector3(PlayerPrefs.GetFloat("PositionX", 15.16f), PlayerPrefs.GetFloat("PositionY", 4.201f), PlayerPrefs.GetFloat("PositionZ", 175.8f));
        progress.CompleteEncounters = PlayerPrefs.GetString("CompleteEncounters", "");
        return progress;
    }
}