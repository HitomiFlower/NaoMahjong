namespace GamePlay.Client.Controller.GameState
{
	/// <summary>
	/// 比赛开始之前的准备状态
	/// <para>初始化CurrentRoundStatus, 点数和玩家姓名
	/// 并广播ClientReady方法</para>
	/// 参考:<see cref="ClientState"/>
	/// </summary>
	public class GamePrepareState : ClientState
	{
		public int[] Points;
		public string[] Names;

		public override void OnClientStateEnter()
		{
			// assign round status
			controller.AssignRoundStatus(CurrentRoundStatus);
			// update data
			CurrentRoundStatus.UpdatePoints(Points);
			CurrentRoundStatus.UpdateNames(Names);
			// send ready message
			ClientBehaviour.Instance.ClientReady();
		}

		public override void OnClientStateExit()
		{
		}

		public override void OnStateUpdate()
		{
		}
	}
}