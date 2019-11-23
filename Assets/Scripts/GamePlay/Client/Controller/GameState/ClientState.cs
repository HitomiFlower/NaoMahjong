using Common.StateMachine.Interfaces;
using UnityEngine;
using GamePlay.Client.Model;

namespace GamePlay.Client.Controller.GameState
{
	/// <summary>
	/// 继承了<c>IState</c>的客户端状态基类
	/// 有controller的实例
	/// </summary>
	/// <remarks>
	/// 除了从IState继承的状态函数，增加了ClientStateEnter和ClientStateExit方法
	/// </remarks>
	public abstract class ClientState : IState
	{
		public ClientRoundStatus CurrentRoundStatus;
		protected ViewController controller;

		public void OnStateEnter()
		{
			Debug.Log($"Client enters {GetType().Name}");
			controller = ViewController.Instance;
			OnClientStateEnter();
		}

		public void OnStateExit()
		{
			Debug.Log($"Client exits {GetType().Name}");
			OnClientStateExit();
		}

		public abstract void OnStateUpdate();

		public abstract void OnClientStateEnter();
		public abstract void OnClientStateExit();
	}
}