using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StarterAssets
{
	public interface IThirdPersonCharacterInput
	{
		Vector2 move { get; set; }
		Vector2 look { get; set; }
		bool jump { get; set; }
		bool sprint { get; set; }

		bool analogMovement { get;}
	}
}