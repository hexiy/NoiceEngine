using Engine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Engine
{
    public struct SceneFile
    {
        public List<GameObject> GameObjects;
        public List<Scripts.Component> Components;
		public int gameObjectNextID;

		public static SceneFile CreateForOneGameObject(GameObject go)
        {
            return new SceneFile() { GameObjects = new List<GameObject>() { go }, Components = go.Components };
        }
    }
}
