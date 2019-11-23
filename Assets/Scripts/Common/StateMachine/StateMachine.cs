using System;
using Common.StateMachine.Interfaces;

namespace Common.StateMachine
{
	/// <summary>
	/// 状态控制机
	/// 执行<c>ChangeState</c>时会触发当前状态的<c>OnStateExit</c>方法，之后触发新状态的<c>OnStateEnter</c>方法
	/// <c>UpdateState</c>方法由<c>ServerBehaviour</c>每帧触发一次，执行状态的<c>OnStateUpdate</c>方法
	/// </summary>
	[Serializable]
	public class StateMachine : IStateMachine
	{
		private IState currentState;

		public virtual void ChangeState(IState newState)
		{
			if (newState == null) throw new ArgumentException("New state cannot be null!");
			currentState?.OnStateExit();
			currentState = newState;
			currentState.OnStateEnter();
		}

		public virtual void UpdateState()
		{
			currentState?.OnStateUpdate();
		}

		public Type CurrentStateType => currentState?.GetType();
	}
}