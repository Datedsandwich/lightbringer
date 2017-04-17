using UnityEngine;
using System.Collections;

public class AutoDestroyParticleSystem : MonoBehaviour
{
	// Data Members ------------------------------------------
	private ParticleSystem particleSystem;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
	// -------------------------------------------------------
	public void Update()
	{
		if(particleSystem.isPlaying)
		{
			// If the particle system is playing, return. We have nothing to do here.
			return;
		}
		// otherwise EXTERMINATE! 
		Destroy(gameObject);
	}
	// End Methods -------------------------------------------------------------
}
