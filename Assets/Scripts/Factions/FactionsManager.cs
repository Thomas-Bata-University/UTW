using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Factions
{
    public class FactionsManager : MonoBehaviour
    {
        private IFactionManager Manager;

        public UnityEvent isManagerInitialized;


        private static FactionsManager instance = null;

        private FactionsManager()
        {
        }

        public static FactionsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FactionsManager();
                }

                return instance;
            }
        }

        private void Start()
        {
            var factory = gameObject.AddComponent<FactionsManagerFactory>();
            Manager = factory.Create();
        }

        public void Initialize()
            => Manager.Initialize();
    }
}