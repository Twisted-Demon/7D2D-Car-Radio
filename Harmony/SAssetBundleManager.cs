 using UnityEngine;

public class SAssetBundleManager
{
    private static SAssetBundleManager? _instance;
    
    public static SAssetBundleManager Instance
    {
        get { return _instance ??= new SAssetBundleManager(); }
    }
    private SAssetBundleManager()
    {
        
    }

    public void Initialize()
    {
        Data = AssetBundle.LoadFromFile("Mods/CarRadio/Resources/Music.unity3d");
        var loadedSongs = Data.LoadAllAssets<AudioClip>();

        foreach (var song in loadedSongs)
        {
            song.LoadAudioData();
            Songs.Add(song.name, song);
            Debug.Log($"Loaded {song.name}.");
        }
        
        Debug.Log($"Loaded {Songs.Count} songs");
    }

    public AssetBundle? Data;
    public readonly Dictionary<string, AudioClip> Songs = new();
}