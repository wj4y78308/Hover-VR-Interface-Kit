﻿using System.Linq;
using Hover.Common.Input;
using Hover.Common.State;
using Hover.Cursor.Custom;
using Hover.Cursor.Input;
using UnityEngine;

namespace Hover.Cursor.State {

	/*================================================================================================*/
	public class CursorState : ICursorState {

		public CursorType Type { get; private set; }
		public bool IsInputAvailable { get; private set; }
		public Vector3 Position { get; private set; }
		public float Size { get; private set; }
		public float DisplayStrength { get; private set; }

		private readonly IInputCursor vInputCursor;
		private readonly ICursorSettings vSettings;
		private readonly Transform vBaseTx;

		private IBaseItemInteractionState[] vInteractItems;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public CursorState(IInputCursor pInputCursor, ICursorSettings pSettings, Transform pBaseTx) {
			vInputCursor = pInputCursor;
			vSettings = pSettings;
			vBaseTx = pBaseTx;
			vInteractItems = new IBaseItemInteractionState[0];

			Type = vInputCursor.Type;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public float GetMaxHighlightProgress() {
			return vInteractItems
				.Select(x => x.MaxHighlightProgress)
				.DefaultIfEmpty(0)
				.Max();
		}

		/*--------------------------------------------------------------------------------------------*/
		public float GetMaxSelectionProgress() {
			return vInteractItems
				.Select(x => x.SelectionProgress)
				.DefaultIfEmpty(0)
				.Max();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Vector3 GetWorldPosition() {
			return vBaseTx.TransformPoint(Position);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void UpdateAfterInput(float pDisplayStren, IBaseItemInteractionState[] pInteractItems) {
			DisplayStrength = pDisplayStren;
			vInteractItems = pInteractItems;

			IsInputAvailable = vInputCursor.IsAvailable;
			Size = vInputCursor.Size;

			Position = vInputCursor.Position+
				vInputCursor.Rotation*Vector3.back*vSettings.CursorForwardDistance;
		}

	}

}
