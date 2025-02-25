using System.Collections;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
	public static IEnumerator WaitForAnimationCompletion(Animator animator, string animationName, int loopCount = 1)
	{
		yield return null;

		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

		while (!stateInfo.IsName(animationName))
		{
			yield return null;
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		}

		int completedLoops = 0;
		while (completedLoops < loopCount)
		{
			yield return null;
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);

			if (stateInfo.normalizedTime >= completedLoops + 1)
			{
				completedLoops++;
			}
		}
	}
}
