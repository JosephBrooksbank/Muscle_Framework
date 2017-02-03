using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyKinectGame
{
    public class GameState
    {
        /**********************************************************************************************/
        // Static:

        public static event Action<string> OnStateActivated;
        public static event Action<string> OnStateDeactivated;
        public static event Action<string> OnStateConditionCompleted;

        public static GameState ActiveState;

        private static Dictionary<string, GameState> GameStates = new Dictionary<string, GameState>();

        /// <summary>
        /// Adds a new game state.
        /// </summary>
        /// <param name="name">The name of the state.</param>
        /// <param name="initHandler">This will be called only once the first time the state is entered.</param>
        /// <param name="enterHandler">This will be called every time the state is activated.</param>
        /// <param name="exitHandler">This will be called every time the state is deactivated.</param>
        /// <param name="updateHandler">This will be called every frame while the state is active.</param>
        public static void Add(string name, Action updateHandler = null, Action initHandler = null, Action enterHandler = null, Action exitHandler = null)
        {
            GameState state = new GameState()
            {
                Name = name,
                OnInit = initHandler,
                OnEnter = enterHandler,
                OnExit = exitHandler,
                OnUpdate = updateHandler
            };

            if (GameStates.ContainsKey(name))
            {
                GameStates[name] = state;
            }
            else
            {
                GameStates.Add(name, state);
            }
        }

        /// <summary>
        /// Sets the activate GameState.
        /// </summary>
        /// <param name="name">The name of the state to activate.</param>
        public static void Set(string name)
        {
            if (!GameStates.ContainsKey(name))
                return;

            if (ActiveState != null && ActiveState.IsActive)
                ActiveState.Deactivate();

            ActiveState = GameStates[name];
            ActiveState.Activate();
        }

        /// <summary>
        /// Returns the desired GameState if it exists.
        /// </summary>
        /// <param name="name">The name of the state to return.</param>
        public static GameState Get(string name)
        {
            if (!GameStates.ContainsKey(name))
                return null;

            return GameStates[name];
        }

        /// <summary>
        /// Returns the names of all the available states within a string array.
        /// </summary>
        public static string[] GetStates()
        {
            return GameStates.Keys.ToArray();
        }

        public static void Complete(string nextState)
        {
            if (ActiveState == null)
                return;
            if (ActiveState.completed)
                return;

            if (OnStateConditionCompleted != null)
            {
                OnStateConditionCompleted(ActiveState.Name);
            }

            Set(nextState);
        }

        /**********************************************************************************************/
        // Internal:

        /// <summary>
        /// Called only once the first time this state is entered.
        /// </summary>
        public Action OnInit;

        /// <summary>
        /// Called every time this state is activated.
        /// </summary>
        public Action OnEnter;

        /// <summary>
        /// Called every time this state is deactivated.
        /// </summary>
        public Action OnExit;

        /// <summary>
        /// Called every frame while this state is active.
        /// </summary>
        public Action OnUpdate;

        /// <summary>
        /// Returns if this state is currently active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return ActiveState == this;
            }
        }

        public string Name = string.Empty;

        private bool initialized = false;
        private bool completed = false;

        public void Activate()
        {
            if (!initialized)
            {
                initialized = true;

                if (OnInit != null)
                {
                    OnInit();
                }
            }

            if (OnEnter != null)
            {
                OnEnter();
            }

            if (OnStateActivated != null)
                OnStateActivated(Name);
        }

        public void Deactivate()
        {
            if (OnExit != null)
            {
                OnExit();
            }

            if (OnStateDeactivated != null)
                OnStateDeactivated(Name);
        }

        public GameState() { }
    }
}
