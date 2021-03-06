using System;
using System.Diagnostics;
using Hover.Cast.Custom.Standard;
using Hover.Cast.State;
using Hover.Common.Custom;
using Hover.Common.Items;
using Hover.Common.State;
using UnityEngine;

namespace Hover.Cast.Display.Standard {

	/*================================================================================================*/
	public class UiPalmNavBackRenderer : MonoBehaviour, IUiItemRenderer {

		public const float InnerRadius = 0;
		public const float OuterRadius = UiPalmRenderer.InnerRadius-UiHoverMeshSlice.EdgeThick-0.005f;

		private static readonly Texture2D IconTex = Resources.Load<Texture2D>("Parent");

		protected IHovercastMenuState vMenuState;
		protected IBaseItemState vItemState;
		protected ISelectableItem vSelItem;
		protected ItemVisualSettingsStandard vSettings;

		protected UiHoverMeshSlice vHoverSlice;
		protected GameObject vIcon;
		protected Stopwatch vEnabledAnim;
		protected bool vPrevEnabled;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual void Build(IHovercastMenuState pMenuState, IBaseItemState pItemState,
													float pArcAngle, IItemVisualSettings pSettings) {
			vMenuState = pMenuState;
			vItemState = pItemState;
			vSelItem = (vItemState.Item as ISelectableItem);
			vSettings = (ItemVisualSettingsStandard)pSettings;

			////

			vHoverSlice = new UiHoverMeshSlice(gameObject);
			vHoverSlice.DrawOuterEdge = true;
			vHoverSlice.Resize(InnerRadius, OuterRadius, pArcAngle);

			////

			vIcon = GameObject.CreatePrimitive(PrimitiveType.Quad);
			vIcon.name = "Icon";
			vIcon.transform.SetParent(gameObject.transform, false);
			vIcon.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up)*
				Quaternion.FromToRotation(Vector3.right, Vector3.up);
			vIcon.renderer.sharedMaterial = new Material(Shader.Find("Unlit/AlphaSelfIllum"));
			vIcon.renderer.sharedMaterial.color = Color.clear;
			vIcon.renderer.sharedMaterial.mainTexture = IconTex;
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void Update() {
			float high = vItemState.MaxHighlightProgress;
			bool showEdge = (vItemState.IsNearestHighlight && !vItemState.IsSelectionPrevented && 
				vSelItem.AllowSelection);
			float edge = (showEdge ? high : 0);
			float select = vItemState.SelectionProgress;
			float alpha = Math.Max(0, 1-(float)Math.Pow(1-vMenuState.DisplayStrength, 2));
			float enabledAnimProg = GetEnabledAnimProgress();

			if ( vSelItem.IsEnabled && vMenuState.NavBackStrength > select ) {
				select = vMenuState.NavBackStrength;
				edge = select;
			}

			select = 1-(float)Math.Pow(1-select, 1.5f);

			Color colBg = vSettings.BackgroundColor;
			Color colEdge = vSettings.EdgeColor;
			Color colHigh = vSettings.HighlightColor;
			Color colSel = vSettings.SelectionColor;
			Color colIcon = vSettings.ArrowIconColor;

			colBg.a *= alpha*Mathf.Lerp(0.333f, 1, enabledAnimProg);
			colEdge.a *= edge*alpha;
			colHigh.a *= high*alpha;
			colSel.a *= select*alpha;
			colIcon.a *= (vItemState.MaxHighlightProgress*0.75f + 0.25f)*alpha*enabledAnimProg;

			vHoverSlice.UpdateBackground(colBg);
			vHoverSlice.UpdateEdge(colEdge);
			vHoverSlice.UpdateHighlight(colHigh, high);
			vHoverSlice.UpdateSelect(colSel, select);

			vIcon.renderer.sharedMaterial.color = colIcon;
			vIcon.transform.localScale = Vector3.one*vSettings.TextSize*0.75f*
				UiItemSelectRenderer.ArcCanvasScale;
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void HandleChangeAnimation(bool pFadeIn, int pDirection, float pProgress) {
			//do nothing...
		}

		/*--------------------------------------------------------------------------------------------*/
		public void UpdateHoverPoints(IBaseItemPointsState pPointsState, Vector3 pCursorWorldPos) {
			vHoverSlice.UpdateHoverPoints(pPointsState);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private float GetEnabledAnimProgress() {
			if ( vSelItem.IsEnabled != vPrevEnabled ) {
				vEnabledAnim = Stopwatch.StartNew();
				vPrevEnabled = vSelItem.IsEnabled;
			}

			float prog = (vSelItem.IsEnabled ? 1 : 0);

			if ( vEnabledAnim != null ) {
				float ms = (float)vEnabledAnim.Elapsed.TotalMilliseconds;
				prog = Math.Min(1, ms/UiArc.LevelChangeMilliseconds);
				prog = 1-(float)Math.Pow(1-prog, 3);

				if ( prog >= 1 ) {
					vEnabledAnim = null;
				}

				if ( !vSelItem.IsEnabled ) {
					prog = 1-prog;
				}
			}

			return prog;
		}

	}

}
