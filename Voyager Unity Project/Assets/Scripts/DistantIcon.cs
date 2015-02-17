using UnityEngine;
using System.Collections;

public class DistantIcon : MonoBehaviour
{

		public bool IconActive;
		public float cameraDistance;
		public float standardTargetDistance;
		public float parentRadius;
		public float multiplier = 3.0f;
		public Vector3 scale;
		public float renderDistanceMod = 30.72205695702f;
		public Vector3 parentScale;

		// Initializing the object variables
		void Start ()
		{
				float[] radius = {
						(float)transform.GetComponentInParent<OrbitalElements> ().orb_elements.radiusx,
						(float)transform.GetComponentInParent<OrbitalElements> ().orb_elements.radiusy,
						(float)transform.GetComponentInParent<OrbitalElements> ().orb_elements.radiusz
				};
				parentRadius = Mathf.Max (radius);												// determine max radius of parent
				standardTargetDistance = (parentRadius / Mathf.Tan (Mathf.PI / 6)) / 20000000;		// determine standard zoom distance
				IconActive = true;
				parentScale = transform.GetComponentInParent<Transform> ().lossyScale;
		}



		// Update is called once per frame
		void Update ()
		{
				cameraDistance = Vector3.Distance (Camera.main.transform.position, transform.position);

				if (cameraDistance < renderDistanceMod * standardTargetDistance) { // if the camera is close to the planet (within acceptable sight range (use the standardDistance*multiplier))
						IconActive = false;
						//Debug.Log ("Within non-render distance");
				} else {
						IconActive = true;
						scale.x = (0.003504237f / parentScale.x) * multiplier * cameraDistance;
						scale.y = (0.003504237f / parentScale.y) * multiplier * cameraDistance;
						scale.z = (0.003504237f / parentScale.z) * multiplier * cameraDistance;

						//THIS LINE CAUSES THE MOONS TO GO HAY-WIRE
						//transform.localScale = scale;

				}
				renderer.enabled = IconActive;	//render the icon if it should be active (beyond distance)
				collider.enabled = IconActive;	//collider is active if it should be active (beyond distance)
		}

}
