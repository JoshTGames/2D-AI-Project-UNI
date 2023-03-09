using System.Collections.Generic;
using System;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Sensors;
using JoshGames.AI.Steering;
using JoshGames.CharacterController;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Character{
    [RequireComponent(typeof(AIMemory), typeof(SteeringContextSolver))]
    public abstract class AIBrain : CharacterBase{
        [Tooltip("If enabled, will show all attached gizmos for this entity")][SerializeField] bool showGizmos;        
        [Tooltip("This is settings for this character... It will dictate how this agent will behave")][SerializeField] public AISettings agentProfile;
        [Tooltip("This is what the agent will remember")][SerializeField] public AIMemory memory;

        #region INVENTORY SETTINGS
        [HideInInspector] public List<int> potentialSelectableItemsFromInventory = new List<int>();
        #endregion
        #region LOCOMOTION SETTINGS
        [Header("Locomotion Settings")]
        [Tooltip("Lesser the value, the closer the agent will arrive at a given position")] public float agentToTargetDelta = .5f; // Distance between agent and target before agent stops moving
        [Tooltip("(FOR AGENT FLEEING), The agent will stop fleeing past this value from target")] public float agentFromTargetDelta = Mathf.Infinity; // Distance between agent and target before agent stops moving
        public bool isSeeking = true; // If false, the agent will flee from whatever its "target" is
        [HideInInspector] public Vector3 moveDirection; // This will be what the agent uses to dictate movement. The move nodes will access this and manipulate it as needed.
        [HideInInspector] public Vector3 positionOfInterest; // This will be what the agent looks at! (Should equivalate to the active position of an object and perhaps a direction) This value should also be read from in other scripts such as the rotation of the vision cone. It should be written to whenever the agent should be looking at something.
        [Tooltip("Calculates the direction an agent should go based on the given behaviours parsed into it.")] public SteeringContextSolver steeringSolver;
        [HideInInspector] public List<SteeringBase> steeringBehaviours = new List<SteeringBase>(); // This is a list of behaviours which are parsed into the solver above which inevitably calculate the direction the agent will move in.
        
        [Header("Patrolling Settings")]
        [SerializeField] Color patrolColour = Color.red;
        [Tooltip("")][SerializeField] List<Vector2> patrolPointPositions;
        int patrolPointIndex = -1;
        public int PatrolPointIndex{ // This value controls the patrol index this agent may move towards
            get{ return patrolPointIndex; }
            set{ patrolPointIndex = Mathf.Clamp(value, 0, patrolPointPositions.Count-1); } // Clamps the new value appropriately to the size of the patrol point list
        }
        #endregion
        #region SENSOR LOGIC
        [Serializable] public struct SensorSettings{
            [Tooltip("The range obstacles will be detected at")] public float collisionCheckRadius; // Greater this value, the greater the checking radius
            public Collider2D mainCollider;
        }
        [Header("Sensor Settings")]
        public SensorSettings sensorSettings;
        public AISensor_Vision vision;
        [Tooltip("Every x seconds, the sensors will be iterated through and checked for updates")][SerializeField] float sensorCheckingFrequency = 0.5f; // This is to ensure Sensors are not running too fast
        public LayerMask whatIsObstacles;
        [HideInInspector] public List<Transform> entitiesInSight = new List<Transform>(); // A list of all entities which are inside the view of the agent
        [HideInInspector] public Transform selectedTarget = null; // This is the chosen target to pursue
        [HideInInspector] public Collider2D[] obstacles; // All obstacles in the proximity
        [HideInInspector] public Collider2D thisCollider;

        [HideInInspector] public int? itemToDropIndexCache;
        [HideInInspector] public Inventory_ItemStats itemToPickUpCache;

        void CheckSight(){            
            entitiesInSight = vision.GetTargets(agentProfile.entitiesOfInterest, agentProfile.ignoreInteractionLayer, transform);        
            obstacles = vision.GetObstacles(transform, whatIsObstacles, sensorSettings.collisionCheckRadius);
        }
        #endregion
        #region BEHAVIOUR-TREE LOGIC
        [Header("BehaviourTree Settings")]
        [Tooltip("Every x seconds, the tree will be iterated through and checked for updates")] public float scanTreeFrequency = 0.5f; // This is to ensure AI isnt be run too fast

        BTNode rootNode; // THIS NODE IS THE TOP NODE; ALL BEHAVIOURS WILL BE CHILDREN TO THIS NODE IN ORDER TO FUNCTION

        /// <summary>
        /// This function is intended to be overridden by inheriting scripts. They will Create the tree inside those scripts and then this script will automatically run them.
        /// </summary>
        /// <returns>The parent node which will run everything</returns>
        protected abstract BTNode CreateTree();
        /// <summary>
        /// This function is what actually iterates through all the nodes.
        /// </summary>
        void ScanTree(){
            if(rootNode == null){ 
                Debug.LogWarning($"{transform.name} {this.name} script has a null rootNode... Please fix this for this script to operate!");
                return; 
            }
            switch(rootNode.Evaluate()){
                case NodeState.Success:
                    break;
                case NodeState.Fail:
                    break;
                case NodeState.Running:
                    break;
            }
        }
        #endregion


        protected void Awake() {
            // SETUP
            if(!memory){ memory = GetComponent<AIMemory>(); }
            if(!thisCollider){ thisCollider = GetComponent<Collider2D>(); }  
            positionOfInterest = transform.position + (Vector3)SteeringBase.directionsAroundCharacter[UnityEngine.Random.Range(0, SteeringBase.directionsAroundCharacter.Length-1)];
            profile.maxHealth = agentProfile.maxHealth;
            profile.health = agentProfile.health;
            profile.speed = agentProfile.speed;
            profile.defaultHeldItems = agentProfile.defaultHeldItems;      
            profile.blood = agentProfile.blood.GetComponent<ParticleSystem>();    
            profile.deathFX = agentProfile.deathFX.GetComponent<ParticleSystem>();    
            // -----
        }

        /// <summary>
        /// This initialises the AI on start
        /// </summary>
        protected override void Start() {            
            base.Start();            

            rootNode = CreateTree();
            InvokeRepeating("CheckSight", 0f, sensorCheckingFrequency);
            InvokeRepeating("ScanTree", 0f, scanTreeFrequency);            
        }

        /// <summary>
        /// Movement is handled in FixedUpdate because its better working hand in hand with the physics engine... It means we can do things such as replay the actions the player did in x amount of time too
        /// </summary>
        private void FixedUpdate() {
            Move(moveDirection, 1.5f, 0); // CREATE A DATA FILE FOR MANIPULATING AGENT SETTINGS E.G: Speed
        }        


        #region PATROLING LOGIC
        /// <summary>
        /// This function will get the nearest position from the agent to the patrol position it may want to be at...
        /// </summary>
        /// <param name="comparingPosition">This should be the position of the active agent.</param>
        /// <returns>An index the agent can use to then find a position it should be at</returns>
        public int GetNearestPatrolPosition(Vector2 comparingPosition){
            int nearestPositionIndex = 0;
            for(int i = 0; i < patrolPointPositions.Count; i++){
                float comparingDistance = Vector2.Distance(patrolPointPositions[i], comparingPosition);
                float nearestDistance = Vector2.Distance(patrolPointPositions[nearestPositionIndex], comparingPosition);

                if(comparingDistance > nearestDistance){ continue; }
                nearestPositionIndex = i;
            }
            return nearestPositionIndex;
        }                
        /// <summary>
        /// This function updates the patrolPointIndex variable and also returns the next position index the agent should be at
        /// </summary>
        /// <returns>the index value to a position list</returns>
        public int GetNextPatrolPosition() => patrolPointIndex = (patrolPointIndex + 1) % patrolPointPositions.Count;
        /// <summary>
        /// This function gets the position in a list and returns that position back to the calling function
        /// </summary>
        /// <param name="_index">The index needed to find a position in the list of positions</param>
        /// <returns>The position the agent should be at</returns>
        public Vector2? GetPatrolPosition(int _index) => patrolPointPositions[_index];
        /// <summary>
        /// This returns a length of the patrol list
        /// </summary>
        /// <returns>An integer holding the length of the list</returns>
        public int GetPatrolLength() => patrolPointPositions.Count;
        #endregion


        /// <summary>
        /// /// This function will add a task to memory for processing by the agent in the future
        /// </summary>
        /// <param name="transform">This is the object the task will be assigned around</param>
        /// <param name="task">This is the task which will drive the behaviour</param>
        int? AddTaskToMemory(float? taskLifetime, float priority, AISettings.TaskPriorities taskPriority, Transform transform, BTNode task){
            if(task == null){ return null; } // If task holds no value...  
            // Create new memory data...
            memory.activeMemory.Add(
                new AIMemory.Memory(
                    taskLifetime,
                    priority,
                    taskPriority,
                    transform.position,
                    transform,
                    task
                )
            );
            return memory.activeMemory.Count;
        }


        /// <summary>
        /// This function negates health off the character
        /// </summary>
        /// <param name="damage">The damage to be inflicted on this character</param>
        public override void Damage(float damage, Vector3? hittersPosition = null){ 
            base.Damage(damage, hittersPosition);
            profile.health = Mathf.Clamp(profile.health - damage, 0, profile.maxHealth); 

            if(hittersPosition == null){ return; }
            BTNode task = null;
            // HOSTILE
            // Make AI face the hitter
            BTLookAt LOOK_AT = new BTLookAt(this, (Vector3)hittersPosition);
            BTWait WAIT = new BTWait(this, 0.5f);
            int rndInvestigateVal = UnityEngine.Random.Range(agentProfile.directionsToInvestigate.min, 3 + 1);      
            BTInvestigate INVESTIGATE = new BTInvestigate(this, 2, 5, .25f, .75f, rndInvestigateVal, transform.position + ((Vector3)hittersPosition - transform.position).normalized, true);
            BTSequence HOSTILE_CHECK = new BTSequence(new List<BTNode>{LOOK_AT, WAIT, INVESTIGATE});

            BTMove FLEE = new BTMove(this, transform.position + ((Vector3)hittersPosition - transform.position).normalized, agentFromTargetDelta, false);
            BTLookAt FACETARGET = new BTLookAt(this, (Vector3)hittersPosition);
            BTSequence FLEE_CHECK = new BTSequence(new List<BTNode>{FLEE, FACETARGET, WAIT});
            BTSelector FLEE_TASK = new BTSelector(new List<BTNode>{FLEE_CHECK}); // THIS WILL HOUSE A VARIETY OF TASKS THE AI COULD DO... E.G. Consume food to gain hp then flee
            
            BTDiceRollHealth CHANCE_TO_FLEE = new BTDiceRollHealth(this); // Check if health is less than a chance value (Re calculates every time it is hit!)
            BTSequence FLEE_ON_LOW_HEALTH = new BTSequence(new List<BTNode>{CHANCE_TO_FLEE, FLEE_TASK}); // If health is less than chance value, flee!

            NodeState hostileFlee = CHANCE_TO_FLEE.Evaluate();
            task = FLEE_TASK;
            foreach(AISettings.Behaviours behaviour in agentProfile.behaviours){
                if(behaviour.behaviourFor == AISettings.Behaviour.Hostile){
                    task = (hostileFlee == NodeState.Fail)? HOSTILE_CHECK : FLEE_ON_LOW_HEALTH;
                    break;
                }
            }

            // GET TASK SETTINGS
            float? taskLifetime = null;
            float taskPriority = 0;
            foreach(AISettings.Priorities tPriority in agentProfile.priorities){
                if(tPriority.task == AISettings.TaskPriorities.Position){ 
                    taskLifetime = tPriority.timeInMemory;
                    taskPriority = tPriority.priority * ((hostileFlee == NodeState.Fail)? 1.25f : Mathf.Infinity);
                }
            }
            AddTaskToMemory(taskLifetime, taskPriority, AISettings.TaskPriorities.Position, transform, task);
        }

        public override void GetHealth(out float _health, out float _maxHealth){
            _health = profile.health;
            _maxHealth = profile.maxHealth;
        }
        

        private void OnDrawGizmos() {
            if(!showGizmos){ return; }
            Gizmos.color = patrolColour;
            foreach(Vector2 pos in patrolPointPositions){
                Gizmos.DrawWireSphere(pos, .1f);
            }
        }
        private void OnDrawGizmosSelected() {
            if(!showGizmos){ return; }
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sensorSettings.collisionCheckRadius);
        }
    }
}