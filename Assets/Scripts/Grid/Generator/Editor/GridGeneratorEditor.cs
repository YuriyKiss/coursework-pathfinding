using UnityEditor;
using UnityEngine;

namespace ThetaStar.Grid.Generator
{
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : Editor
    {
        private GridGenerator _generator;

        private void Awake()
        {
            _generator = (GridGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Grid"))
            {
                _generator.RegenerateGrid();
            }
        }
    }
}
