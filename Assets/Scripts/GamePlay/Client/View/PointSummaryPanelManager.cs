﻿using System.Collections;
using System.Collections.Generic;
using GamePlay.Client.Controller;
using GamePlay.Client.Model;
using GamePlay.Client.View.SubManagers;
using Mahjong.Logic;
using Mahjong.Model;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamePlay.Client.View
{
	public class PointSummaryPanelManager : MonoBehaviour
	{
		public Text PlayerNameText;
		public SummaryHandTileManager HandTileManager;
		public WinTypeManager WinTypeManager;
		public DoraPanelManager DoraPanelManager;
		public PointInfoManager PointInfoManager;
		public YakuRankManager YakuRankManager;
		public Button ConfirmButton;
		public CountDownController ConfirmCountDownController;

		public void ShowPanel(SummaryPanelData data, UnityAction callback)
		{
			ConfirmButton.onClick.RemoveAllListeners();
			ConfirmButton.onClick.AddListener(() =>
			{
				ConfirmCountDownController.StopCountDown();
				callback();
			});
			gameObject.SetActive(true);
			PlayerNameText.text = data.PlayerName;
			HandTileManager.SetHandTiles(data.HandInfo.HandTiles, data.HandInfo.OpenMelds, data.HandInfo.WinningTile);
			WinTypeManager.SetType(data.HandInfo.IsTsumo);
			var uraDora = data.HandInfo.IsRichi ? data.HandInfo.UraDoraIndicators : null;
			DoraPanelManager.SetDoraIndicators(data.HandInfo.DoraIndicators, uraDora);
			// yaku list, total point and yaku rank
			Timing.RunCoroutine(YakuListCoroutine(data.PointInfo, data.TotalPoints, data.HandInfo.IsRichi, callback));
		}

		private IEnumerator<float> YakuListCoroutine(PointInfo point, int totalPoints, bool richi, UnityAction callback)
		{
			yield return Timing.WaitUntilDone(
				Timing.RunCoroutine(PointInfoManager.SetPointInfo(point, totalPoints, richi)));
			yield return Timing.WaitForSeconds(MahjongConstants.SummaryPanelDelayTime);
			YakuRankManager.ShowYakuRank(point);
			ConfirmCountDownController.StartCountDown(MahjongConstants.SummaryPanelWaitingTime, callback);
		}

		public int StopCountDown()
		{
			return ConfirmCountDownController.StopCountDown();
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}