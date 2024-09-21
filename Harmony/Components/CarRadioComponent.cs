using Harmony.NetPackages;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = System.Random;

namespace Harmony;

public class CarRadioComponent : MonoBehaviour
{
    private AudioSource? _audioSource;
    public EntityVehicle? entityVehicle;

    private readonly Queue<string> _songQueue = new();
    public string currentSong = "";

    private bool _isOn = false;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.65f;
    }

    public void OnOffClient()
    {
        _isOn = !_isOn;
        
        switch (_isOn)
        {
            case false:
                _audioSource!.Pause();
                break;
            case true:
                _audioSource!.Play();
                break;
        }
    }

    private void Update()
    {
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            if(_isOn & (!_audioSource!.isPlaying || currentSong == ""))
                NextSongPressed();
        }
        
        var player = entityVehicle!.world.GetLocalPlayers()[0];
        if (!entityVehicle!.IsAttached(player))
            return;
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
            OnOffServer();
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            NextSongPressed();
    }

    public void Shuffle()
    {
        var numberOfSongs = SAssetBundleManager.Instance.Songs!.Count;
        var songIndexList = new List<string>();
        _songQueue.Clear();

        for (var i = 0; i < numberOfSongs; i++)
        {
            songIndexList.Add(SAssetBundleManager.Instance.Songs.Keys.ToList()[i]);
        }

        var random = new Random();
        var n = numberOfSongs;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            (songIndexList[k], songIndexList[n]) = (songIndexList[n], songIndexList[k]);
        }

        songIndexList.Remove(currentSong);
        
        foreach (var index in songIndexList)
        {
            _songQueue.Enqueue(index);
        }
    }

    private void NextSongPressed()
    {
        //check if we are server, if NOT server - send request to play next song
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(
                (NetPackage) NetPackageManager.GetPackage<NetPackageRadioNextSong>()
                    .Setup(entityVehicle!.entityId));
        }
        else
        {
            PlayNextSong();
        }
        
    }

    public void PlayNextSong()
    {
        if(_songQueue.Count == 0)
            Shuffle();
        
        currentSong = _songQueue.Dequeue();
        PlaySong(currentSong);

        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(
                NetPackageManager.GetPackage<NetPackageRadioPlaySong>()
                    .Setup(entityVehicle!.entityId, currentSong), _allButAttachedToEntityId: entityVehicle.entityId);
    }

    public void PlaySong(string songName)
    {
        var clipToPlay = SAssetBundleManager.Instance.Songs![songName];
        
        _audioSource!.clip = clipToPlay;
        _audioSource!.Play();
        _isOn = true;

        var localPlayer = GameManager.Instance.m_World.GetLocalPlayers()[0];
        GameManager.ShowTooltip(localPlayer, $"Now Playing: {songName}");
    }
    
    public void OnOffServer()
    {
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(
                (NetPackage) NetPackageManager.GetPackage<NetPackageRadioOnOff>()
                    .Setup(entityVehicle!.entityId, 2));
        }
        else
        {
            OnOffClient();
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(
                (NetPackage) NetPackageManager.GetPackage<NetPackageRadioOnOff>()
                    .Setup(entityVehicle!.entityId, 2), _allButAttachedToEntityId: entityVehicle.entityId);
        }
            
    }
    
}