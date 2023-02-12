using UnityEngine;
using UnityEngine.UI;

namespace Utility.InvisibleGraphicExtension
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class InvisibleGraphic : Graphic
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}