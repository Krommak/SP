using Leopotam.EcsLite;
using System.Collections.Generic;

namespace Game.Services
{
    public sealed class SpawningDataService
    {
        private Dictionary<EcsWorld, Dictionary<string, ISpawningData>> _datasByWorld;

        public SpawningDataService()
        {
            _datasByWorld = new Dictionary<EcsWorld, Dictionary<string, ISpawningData>>();
        }

        public void RegisterNewData(EcsWorld world, ISpawningData data)
        {
            if (_datasByWorld.ContainsKey(world) && !_datasByWorld[world].ContainsKey(data.GetType().Name))
            {
                _datasByWorld[world].Add(data.GetType().Name, data);
            }
            else
            {
                _datasByWorld.Add(world, new Dictionary<string, ISpawningData>());
                _datasByWorld[world].Add(data.GetType().Name, data);
            }
        }

        public bool TryChangeData(EcsWorld world, string dataName, IChangeSpawningData dataChanges)
        {
            if (!_datasByWorld.ContainsKey(world) || !_datasByWorld[world].ContainsKey(dataName))
                return false;

            _datasByWorld[world][dataName].ChangeData(dataChanges);
            return true;
        }

        public bool TryGetSpawningData(EcsWorld world, string dataName, out ISpawningData spawningData)
        {
            if(!_datasByWorld.ContainsKey(world) || !_datasByWorld[world].ContainsKey(dataName))
            {
                spawningData = null;
                return false;
            }
            else
            {
                spawningData = _datasByWorld[world][dataName];
                return true;
            }
        }
    }

    public interface ISpawningData
    {
        public abstract void ChangeData(IChangeSpawningData change);
    }

    public interface IChangeSpawningData
    {
    }
}