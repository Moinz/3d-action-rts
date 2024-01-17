using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace CM.WorldGrid
{
    [Overlay(typeof(SceneView), "World Grid")]
    [Icon("BA")]
    public class WorldGridToolOverlay : ToolbarOverlay
    {
        WorldGridToolOverlay() : base(
            CollapseButton.id,
            CollapseAllButton.id)
        {}
    }
}