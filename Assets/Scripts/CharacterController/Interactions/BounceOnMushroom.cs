using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using UnityEngine.Events;

namespace Interactions.Behaviors {
	[Serializable]
	[InteractionType("Misc/BounceOnMushroom")]
	public class BounceOnMushroom : CustomInteraction {
		public BounceOnMushroom(Interaction parent) : base(parent) { }

		CharacterController _characterController => GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
		
		public void Awake()
		{
			base.Interact(); // Invoke Unity Event
		}
	

		public override void EndInteraction() {

		}
	}
}