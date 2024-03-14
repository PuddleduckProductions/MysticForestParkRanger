using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Behaviors
{
	[Serializable, InteractionType("Plants/VineGrowth")]
	public class VineGrowthInteraction : CustomInteraction
	{
		public VineGrowthInteraction(Interaction parent)
			: base(parent) { }

		public Material targetMaterial; // Assign this via the Inspector
		public float transitionDuration = 5f; // Duration from 0 to 1
		private float _elapsedTime = 0f; // To track the time since the interaction started
		private bool _shrinkIsActive = false; // To track whether the interaction is active


		public override void Interact() 
		{
			_shrinkIsActive = true;
			_elapsedTime = 0f; // Reset the time for each interaction
			
			targetMaterial.SetFloat("_GrowthAmount", 1f); // Ensure it's set to 1 at the start
		}

		public override bool Update()
		{
			// >> SHRINK ROUTINE
			if (_shrinkIsActive)
			{
				_elapsedTime += Time.deltaTime; // Update the elapsed time

				// Calculate the current growth amount based on the elapsed time
				float currentGrowthAmount = Mathf.Lerp(1f, 0f, _elapsedTime / transitionDuration);
				targetMaterial.SetFloat("_GrowthAmount", currentGrowthAmount);

				// Check if the transition is complete
				if (_elapsedTime >= transitionDuration)
				{
					_shrinkIsActive = false; // Stop updating the growth amount
					targetMaterial.SetFloat("_GrowthAmount", 0f); // Ensure it's set to 0 at the end
				}
				return true;
			}
			return false;
		}
	}
}
