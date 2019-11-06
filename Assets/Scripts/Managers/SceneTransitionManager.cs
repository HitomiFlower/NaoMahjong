using UnityEngine;

namespace Managers
{
	[RequireComponent(typeof(Animator))]
	public class SceneTransitionManager : MonoBehaviour
	{
		public Animator animator;
		private static readonly int Start = Animator.StringToHash("start");
		private static readonly int End = Animator.StringToHash("end");

		public void FadeIn()
		{
			animator.SetTrigger(Start);
		}

		public void FadeOut()
		{
			animator.SetTrigger(End);
		}
	}
}