using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamePlay.Client.Model;
using GamePlay.Client.View;
using GamePlay.Server.Model;
using Mahjong.Logic;
using Mahjong.Model;
using MEC;
using UnityEngine;
using Utils;

namespace GamePlay.Client.Controller
{
	/// <summary>
	/// 客户端麻将比赛的UI控制器
	/// </summary>
	public class ViewController : Singleton<ViewController>
	{
		/// <summary>
		/// 麻将棋盘信息显示
		/// </summary>
		public BoardInfoManager BoardInfoManager;

		/// <summary>
		/// 牌山控制器
		/// </summary>
		public YamaManager YamaManager;

		/// <summary>
		/// 牌桌手牌控制器（包括打出去的牌）
		/// </summary>
		public TableTilesManager TableTilesManager;

		/// <summary>
		/// 玩家信息（姓名）
		/// </summary>
		public PlayerInfoManager PlayerInfoManager;

		/// <summary>
		/// 玩家手牌显示UI
		/// </summary>
		public HandPanelManager HandPanelManager;

		/// <summary>
		/// 回合时间显示UI
		/// </summary>
		public TimerController TurnTimeController;

		/// <summary>
		/// 玩家特效（吃碰杠等）控制UI
		/// </summary>
		public PlayerEffectManager PlayerEffectManager;

		/// <summary>
		/// 玩家回合内的控制（自摸，杠，拔北等）按钮控制UI
		/// </summary>
		public InTurnPanelManager InTurnPanelManager;

		/// <summary>
		/// 玩家回合外的控制（吃碰杠等）按钮控制UI
		/// </summary>
		public OutTurnPanelManager OutTurnPanelManager;

		/// <summary>
		/// 听牌时的对子/顺子选择
		/// </summary>
		public MeldSelectionManager MeldSelectionManager;

		/// <summary>
		/// 流局时的听牌/未听牌的显示UI
		/// </summary>
		public WaitingPanelManager[] WaitingPanelManagers;

		/// <summary>
		/// 听牌指示器
		/// </summary>
		public ReadyHintManager ReadyHintManager;

		/// <summary>
		/// 流局指示器
		/// </summary>
		public RoundDrawManager RoundDrawManager;

		/// <summary>
		/// 回合结束分数显示UI
		/// </summary>
		public PointSummaryPanelManager PointSummaryPanelManager;

		/// <summary>
		/// 分数结算UI
		/// </summary>
		public PointTransferManager PointTransferManager;

		/// <summary>
		/// 游戏结束排行榜UI
		/// </summary>
		public GameEndPanelManager GameEndPanelManager;

		/// <summary>
		/// 本地自动控制（理牌，和牌，不鸣牌，摸切）
		/// </summary>
		public LocalSettingManager LocalSettingManager;

		private ClientRoundStatus CurrentRoundStatus;

		/// <summary>
		/// 初始化比赛状态，将Manager与麻将牌加入到观察者名单以便更新其状态
		/// </summary>
		/// <param name="status">比赛状态</param>
		public void AssignRoundStatus(ClientRoundStatus status)
		{
			CurrentRoundStatus = status;
			status.AddObserver(BoardInfoManager);
			status.AddObserver(YamaManager);
			status.AddObserver(TableTilesManager);
			status.AddObserver(PlayerInfoManager);
			status.AddObserver(HandPanelManager);
			status.AddObserver(PointTransferManager);
			status.AddObserver(ReadyHintManager);
			// add tiles as observer
			foreach (var tile in HandPanelManager.HandTiles)
			{
				status.AddObserver(tile);
			}

			status.AddObserver(HandPanelManager.LastDrawTile);
			status.LocalSettings.AddObserver(LocalSettingManager);
		}

		/// <summary>
		/// 显示玩家回合内的状态栏
		/// </summary>
		/// <param name="operations">当前玩家可用的操作</param>
		/// <param name="bonusTurnTime">玩家额外的时间</param>
		public void ShowInTurnPanels(InTurnOperation[] operations, int bonusTurnTime)
		{
			var settings = CurrentRoundStatus.LocalSettings;
			var richied = CurrentRoundStatus.GetRichiStatus(0);
			var lastDraw = (Tile)CurrentRoundStatus.GetLastDraw(0);
			// auto discard when richied or set to qie
			if ((settings.Qie || richied) && operations.All(op => op.Type == InTurnOperationType.Discard))
			{
				if (richied) HandPanelManager.LockTiles();
				Timing.RunCoroutine(CoAutoDiscard(lastDraw, bonusTurnTime));
				InTurnPanelManager.Close();
				return;
			}

			// check settings
			if (settings.He)
			{
				// handle auto-win
				int index = System.Array.FindIndex(operations, op => op.Type == InTurnOperationType.Tsumo);
				if (index >= 0)
				{
					ClientBehaviour.Instance.OnTsumoButtonClicked(operations[index]);
					return;
				}
			}

			// not richied, show timer and panels
			CurrentRoundStatus.CalculatePossibleWaitingTiles();
			CurrentRoundStatus.ClearWaitingTiles();
			InTurnPanelManager.SetOperations(operations);
			TurnTimeController.StartCountDown(CurrentRoundStatus.GameSetting.BaseTurnTime, bonusTurnTime, () =>
			{
				Debug.Log("Time out! Automatically discarding last drawn tile");
				CurrentRoundStatus.SetRichiing(false);
				ClientBehaviour.Instance.OnDiscardTile(lastDraw, true, 0);
				InTurnPanelManager.Close();
			});
		}

		/// <summary>
		/// 自动弃牌
		/// </summary>
		/// <param name="tile">要弃的牌</param>
		/// <param name="bonusTimeLeft">剩余的额外时间</param>
		private IEnumerator<float> CoAutoDiscard(Tile tile, int bonusTimeLeft)
		{
			yield return Timing.WaitForSeconds(MahjongConstants.AutoDiscardDelayAfterRichi);
			ClientBehaviour.Instance.OnDiscardTile(tile, true, bonusTimeLeft);
		}

		/// <summary>
		/// 显示回合外的玩家动作响应UI
		/// </summary>
		/// <param name="operations">当前玩家可用的操作</param>
		/// <param name="bonusTurnTime">可用的额外时间</param>
		/// <returns>显示UI则返回true,否则返回false</returns>
		public bool ShowOutTurnPanels(OutTurnOperation[] operations, int bonusTurnTime)
		{
			if (operations == null || operations.Length == 0)
			{
				Debug.LogError("Received with no operations, this should not happen");
				ClientBehaviour.Instance.OnSkipOutTurnOperation(bonusTurnTime);
				return false;
			}

			var settings = CurrentRoundStatus.LocalSettings;
			if (settings.He)
			{
				// handle auto-win
				int index = System.Array.FindIndex(operations, op => op.Type == OutTurnOperationType.Rong);
				if (index >= 0)
				{
					ClientBehaviour.Instance.OnOutTurnButtonClicked(operations[index]);
					return false;
				}
			}

			if (settings.Ming)
			{
				// handle dont-open 关闭鸣牌
				for (int i = 0; i < operations.Length; i++)
				{
					var operation = operations[i];
					if (operation.Type == OutTurnOperationType.Chow
					    || operation.Type == OutTurnOperationType.Pong
					    || operation.Type == OutTurnOperationType.Kong)
					{
						operations[i] = new OutTurnOperation {Type = OutTurnOperationType.Skip};
					}
				}
			}

			// if all the operations are skip, automatically skip this turn.
			if (operations.All(op => op.Type == OutTurnOperationType.Skip))
			{
				Debug.Log("Only operation is skip, skipping turn");
				ClientBehaviour.Instance.OnSkipOutTurnOperation(bonusTurnTime);
				OutTurnPanelManager.Close();
				return false;
			}

			// Time out
			OutTurnPanelManager.SetOperations(operations);
			TurnTimeController.StartCountDown(CurrentRoundStatus.GameSetting.BaseTurnTime, bonusTurnTime, () =>
			{
				Debug.Log("Time out! Automatically skip this turn");
				ClientBehaviour.Instance.OnSkipOutTurnOperation(0);
				OutTurnPanelManager.Close();
			});
			return true;
		}

		/// <summary>
		/// 显示特效
		/// </summary>
		/// <param name="placeIndex">玩家序号，玩家自己序号为0，逆时针依次加1</param>
		/// <param name="type">特效类型</param>
		/// <returns>特效的动画时间</returns>
		public float ShowEffect(int placeIndex, PlayerEffectManager.Type type)
		{
			return PlayerEffectManager.ShowEffect(placeIndex, type);
		}

		/// <summary>
		/// 显示玩家手牌
		/// </summary>
		/// <param name="placeIndex">玩家序号，玩家自己序号为0，逆时针依次加1</param>
		/// <param name="handData">手牌信息</param>
		public IEnumerator<float> RevealHandTiles(int placeIndex, PlayerHandData handData)
		{
			yield return Timing.WaitForSeconds(MahjongConstants.HandTilesRevealDelay);
			TableTilesManager.OpenUp(placeIndex);
			TableTilesManager.SetHandTiles(placeIndex, handData.HandTiles);
		}
	}
}