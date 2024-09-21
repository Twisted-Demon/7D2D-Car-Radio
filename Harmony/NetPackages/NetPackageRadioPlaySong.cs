using Harmony;
using UnityEngine;

public class NetPackageRadioPlaySong : NetPackage
{
    public int vehicleEntityId;
    public string songName;

    public NetPackageRadioPlaySong Setup(int _vehicleEntityId, string _songIndex)
    {
        this.vehicleEntityId = _vehicleEntityId;
        this.songName = _songIndex;
        return this;
    }

    public override void read(PooledBinaryReader _br)
    {
        this.vehicleEntityId = _br.ReadInt32();
        this.songName = _br.ReadString();
    }

    public override void write(PooledBinaryWriter _br)
    {
        base.write(_br);
        _br.Write(this.vehicleEntityId);
        _br.Write(this.songName);
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        if (_world.GetEntity(vehicleEntityId) is not EntityVehicle vehicle)
            return;
        
        var carRadio = vehicle.GetComponent<CarRadioComponent>();
        if (carRadio is null)
            return;

        carRadio.PlaySong(songName);
    }

    public override int GetLength() => 8 + 8;
}