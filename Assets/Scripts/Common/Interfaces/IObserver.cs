namespace Common.Interfaces
{
	/// <summary>
	/// 观察者接口
	/// </summary>
	/// <typeparam name="T">逆变泛型</typeparam>
	public interface IObserver<in T>
	{
		void UpdateStatus(T subject);
	}
}