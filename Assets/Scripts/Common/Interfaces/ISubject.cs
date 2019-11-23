using System;

namespace Common.Interfaces
{
	/// <summary>
	/// 设定类的接口
	/// </summary>
	/// <typeparam name="T">协变泛型</typeparam>
	public interface ISubject<out T>
	{
		void AddObserver(IObserver<T> observer);
		void RemoveObserver(IObserver<T> observer);
		void NotifyObservers();
	}
}