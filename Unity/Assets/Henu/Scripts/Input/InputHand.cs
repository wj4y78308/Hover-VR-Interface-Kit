﻿using Leap;
using UnityEngine;

namespace Henu.Input {

	/*================================================================================================*/
	public class InputHand {

		public bool IsLeft { get; set; }
		public Vector3 Center { get; set; }
		public Quaternion Rotation { get; set; }
		public float GrabStrength { get; set; }
		public float PalmTowardEyes { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public InputHand(Hand pHand) {
			IsLeft = pHand.IsLeft;
			Center = pHand.PalmPosition.ToUnityScaled();
			Rotation = InputPoint.CalcQuaternion(pHand.Basis);
			GrabStrength = pHand.GrabStrength;
			PalmTowardEyes = Vector3.Dot(pHand.PalmNormal.ToUnity(), Vector3.down);
		}

	}

}