using Engine;
using System.Collections.Generic;

namespace Scripts
{
    public class Pool
    {
        public GameObject model;
        public Stack<GameObject> freeObjects = new Stack<GameObject>();
        public Stack<GameObject> usedObjects = new Stack<GameObject>();
        private void AddNewObject()
        {
            GameObject gameObject = GameObject.Create(name: "Pooled object");
            for (int i = 0; i < model.Components.Count; i++)
            {
                gameObject.AddComponent(model.Components[i].GetType());
            }
            gameObject.Awake();
            freeObjects.Push(gameObject);
        }
        public GameObject Request()
        {
            if (freeObjects.Count == 0)
            {
                AddNewObject();
            }
            GameObject gameObject = freeObjects.Pop();
            gameObject.Active = true;
            usedObjects.Push(gameObject);
            return gameObject;
        }
        public void Return(GameObject gameObject)
        {
            gameObject.Active = false;
            freeObjects.Push(gameObject);
        }
    }
}
