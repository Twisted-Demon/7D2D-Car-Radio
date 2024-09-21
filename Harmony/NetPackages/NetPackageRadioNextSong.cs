namespace Harmony.NetPackages;

public class NetPackageRadioNextSong : NetPackage
{
    public int vehicleEntityId;

    public NetPackageRadioNextSong Setup(int _vehicleEntityId)
    {
        this.vehicleEntityId = _vehicleEntityId;
        return this;
    }
    public override void read(PooledBinaryReader _reader)
    {
        this.vehicleEntityId = _reader.ReadInt32();
    }

    public override void write(PooledBinaryWriter _writer)
    {
        base.write(_writer);
        _writer.Write(this.vehicleEntityId);
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        var vehicleEntity = _world.GetEntity(vehicleEntityId);
        if (vehicleEntity is null)
            return;

        var radio = vehicleEntity.GetComponent<CarRadioComponent>();
        if (radio is null)
            return;

        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            radio.PlayNextSong();
        }
    }

    public override int GetLength() => 0;
}