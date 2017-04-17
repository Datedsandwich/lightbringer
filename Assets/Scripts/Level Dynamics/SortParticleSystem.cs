using UnityEngine;
using System.Collections;

public class SortParticleSystem : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public string layerName = "Particles";
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = layerName;
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
