using JazSharp.SpyLogic;

namespace JazSharp.Spies
{
    public class Spy : ISpy
    {
        private readonly SpyInfo _spyInfo;
        private readonly object _key;

        SpyInfo ISpy.SpyInfo => _spyInfo;
        object ISpy.Key => _key;

        public SpyAnd And => new SpyAnd(this, _spyInfo, _key);

        internal Spy(SpyInfo spyInfo, object key)
        {
            _spyInfo = spyInfo;
            _key = key;
        }

        public void Dispose()
        {
            _spyInfo.StopSpying(_key);
        }
    }
}
