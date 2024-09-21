using Harmony;

public class NetPackageRadioOnOff : NetPackage
{
    public int vehicleEntityId;
    public int songIndex;

    public NetPackageRadioOnOff Setup(int _vehicleEntityId, int _songIndex)
    {
        this.vehicleEntityId = _vehicleEntityId;
        this.songIndex = _songIndex;
        return this;
    }
    
    public override void read(PooledBinaryReader _br)
    {
        this.vehicleEntityId = _br.ReadInt32();
        this.songIndex = _br.ReadInt32();
    }

    public override void write(PooledBinaryWriter _br)
    {
        base.write(_br);
        _br.Write(this.vehicleEntityId);
        _br.Write(this.songIndex);
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        var vehicle = _world.GetEntity(vehicleEntityId) as EntityVehicle;
        if (vehicle == null)
            return;
        
        var carRadio = vehicle!.GetComponent<CarRadioComponent>();
        if (carRadio is null)
            return;
        
        carRadio.OnOffClient();
        
        if(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(
                (NetPackage) NetPackageManager.GetPackage<NetPackageRadioOnOff>()
                    .Setup(vehicleEntityId, 2), _allButAttachedToEntityId: vehicleEntityId);
    }

    public override int GetLength() => 8 + 8;
}