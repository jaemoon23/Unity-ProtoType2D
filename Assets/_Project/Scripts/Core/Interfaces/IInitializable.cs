using Cysharp.Threading.Tasks;

/// <summary>
/// 비동기 초기화가 필요한 매니저들이 구현하는 인터페이스
/// </summary>
public interface IInitializable
{
    /// <summary>
    /// 초기화 완료 여부
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// 비동기 초기화 수행
    /// </summary>
    UniTask InitializeAsync();
}
