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

namespace GamePlay.Client.Controller
{
	public class ViewController : MonoBehaviour
	{
		public static ViewController Instance
		{
			get;
			private set;
		}

		public BoardInfoManager BoardInfoManager;
		public YamaManager YamaManager;
		public TableTilesManager TableTilesManager;
		public PlayerInfoManager PlayerInfoManager;
		public HandPanelManager HandPanelManager;
		public TimerController TurnTimeController;
		public PlayerEffectManager PlayerEffectManager;
		public InTurnPanelManager InTurnPanelManager;
		public OutTurnPanelManager OutTurnPanelManager;
		public MeldSelectionManager MeldSelectionManager;
		public WaitingPanelManager[] WaitingPanelManagers;
		public ReadyHintManager ReadyHintManager;
		public RoundDrawManager RoundDrawManager;
		public PointSummaryPanelManager PointSummaryPanelManager;
		public PointTransferManager PointTransferManager;
		public GameEndPanelManager GameEndPanelManager;
		public LocalSettingManager LocalSettingManager;
		private ClientRoundStatus CurrentRoundStatus;

		private void OnEnable()
		{
			Instance = this;
		}

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

		public void ShowInTurnPanels(InTurnOperation[] operations, int bonusTurnTime)
		{
			var settings = CurrentRoundStatus.LocalSettings;
			var richied = CurrentRoundStatus.GetRichiStatus(0);
			var lastDraw = (Tile)CurrentRoundStatus.GetLastDraw(0);
			// auto discard when richied or set to qie
			if ((settings.Qie || richied) && operations.All(op => op.Type == InTurnOperationType.Discard))
			{
				if (richied) HandPanelManager.LockTiles();
				Timing.RunCoroutine(AutoDiscard(lastDraw, bonusTurnTime));
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

		private IEnumerator<float> AutoDiscard(Tile tile, int bonusTimeLeft)
		{
			yield return Timing.WaitForSeconds(MahjongConstants.AutoDiscardDelayAfterRichi);
			ClientBehaviour.Instance.OnDiscardTile(tile, true, bonusTimeLeft);
		}

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
				// handle dont-open
				for (int i = 0; i < operations.Length; i++)
				{
					var operation = operations[i];
					if (operation.Type == OutTurnOperationType.Chow
					    || operation.Type == OutTurnOperationType.Pong
					    || operation.Type == OutTurnOperationType.Kong)
						operations[i] = new OutTurnOperation {Type = OutTurnOperationType.Skip};
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

			OutTurnPanelManager.SetOperations(operations);
			TurnTimeController.StartCountDown(CurrentRoundStatus.GameSetting.BaseTurnTime, bonusTurnTime, () =>
			{
				Debug.Log("Time out! Automatically skip this turn");
				ClientBehaviour.Instance.OnSkipOutTurnOperation(0);
				OutTurnPanelManager.Close();
			});
			return true;
		}

		public float ShowEffect(int placeIndex, PlayerEffectManager.Type type)
		{
			return PlayerEffectManager.ShowEffect(placeIndex, type);
		}

		public IEnumerator<float> RevealHandTiles(int placeIndex, PlayerHandData handData)
		{
			yield return Timing.WaitForSeconds(MahjongConstants.HandTilesRevealDelay);
			TableTilesManager.OpenUp(placeIndex);
			TableTilesManager.SetHandTiles(placeIndex, handData.HandTiles);
		}
	}
}