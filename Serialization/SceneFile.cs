using Engine;
using System.Collections.Generic;

namespace Engine
{
    public struct SceneFile
    {
        public List<GameObject> GameObjects;
        public List<Scripts.Component> Components;
		public int gameObjectNextID;

		public SceneFile CreateForOneGameObject(GameObject go)
        {
            return new SceneFile() { GameObjects = new List<GameObject>() { go }, Components = go.Components };
        }
    }
}
