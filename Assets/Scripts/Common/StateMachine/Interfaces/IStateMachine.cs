
namespace Common.StateMachine.Interfaces
{
	/// <summary>
	/// 状态机接口
	/// <c>UpdateState</c>方法由<c>ServerBehaviour</c>每帧触发一次，执行状态的<c>OnStateUpdate</c>方法
	/// </summary>
	public interface IStateMachine
	{
		void ChangeState(IState newState);
		void UpdateState();
	}
}