using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Core.Config
{
    public interface IConfigProvider : IService
    {
        UniTask LoadConfigAsync();
        T GetConfig<T>(ConfigType type) where T : BaseConfigScript;
    }
}
