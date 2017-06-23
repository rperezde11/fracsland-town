using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This abstract class is used to control a human
/// and it's animations
/// </summary>
public abstract class HumanNPC : LivingBeing {

    public enum NPC_ANIMATION
    {
        RUN = 0,
        WALK_FRONT = 1,
        WALK_BACK = 2,
        WALK_RIGHT = 3,
        WALK_LEFT = 4,
        IDLE_1 = 5,
        IDLE_2 = 6,
        ATTACK_OR_MINE = 7,
        ATTACK_OR_MINE2 = 8,
        LUMBERING = 9,
        SAWING = 10,
        BUILD_1 = 11,
        BUILD_2 = 12,
        DIGGING = 13,
        USE = 14,
        GATHER = 15,
        TALK = 16,
        HIT1 = 17,
        HIT2 = 18,
        DEATH = 19,
        JUMP = 20
    }
			
	protected Animation _animation;
	protected List<string> _animations;

    protected NPC_ANIMATION state = NPC_ANIMATION.IDLE_1;

	public abstract void SelfSetup();
	public abstract void Setup();
	public abstract void UpdateHuman();
	protected MovementManager _movementManager;

	protected int i = 0;

    void Awake()
    {
        _animation = GetComponent<Animation>();
        _animations = new List<string>();
		_movementManager = GetComponent<MovementManager> ();
        foreach (AnimationState state in _animation)
        {
            _animations.Add(state.name);
        }

		SelfSetup ();
    }

    void Start ()
    {
		Setup();	
	}
	
	void Update ()
    {
		UpdateHuman ();
	}

	public void PlayAnimation(NPC_ANIMATION animation)
    {
        _animation.Play(_animations[(int)animation], PlayMode.StopAll);
        state = animation;
    }		
}
