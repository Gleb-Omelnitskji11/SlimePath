namespace Core.ObjectPool
{
    public class PooledObject<T> where T : class
    {
        private IObjectPooler _pooler;
        private string _poolKey;

        public void SetPoolableData(IObjectPooler pooler, string poolKey)
        {
            _pooler = pooler;
            _poolKey = poolKey;
        }
        
        public void Deactivate(T objectToSet, bool byManual)
        {
            _pooler.Release<T>(_poolKey, objectToSet );
        }
    }
}