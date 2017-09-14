using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax {
	public class ParallaxLayerGroup : MonoBehaviour {
		[Tooltip("Leave layers blank to automatically grab layers from nested objects")]
		[SerializeField]
		private List<ParallaxLayer> _layers;

		[Tooltip("On start will sync all objects to the current tracking object. Uses the transform position of this object to simulate" +
		         " the move. Useful for making a group of parallax layers match up with the tracking object on initial setup")]
		[SerializeField]
		private bool _syncToTrackingTarget;

		[Tooltip("Set the parallax speed relative to the closest and furthest layer")]
		[SerializeField]
		private bool _setRelativeParallaxSpeed = true;

		[ShowToggle("_setRelativeParallaxSpeed")]
		[Tooltip("Override settings for what is considered the furthest layer")]
		[SerializeField]
		private ParallaxLayerDistance _furthestLayer = new ParallaxLayerDistance { maxSpeed = new Vector2Data(1, 0) };

		[ShowToggle("_setRelativeParallaxSpeed")]
		[Tooltip("Override settings for what is considered the closest layer")]
		[SerializeField]
		private ParallaxLayerDistance _closestLayer = new ParallaxLayerDistance { maxSpeed = new Vector2Data(-0.2f, 0) };

		private void Awake () {
			if (_layers == null || _layers.Count == 0) {
				_layers = GetChildLayers().ToList();
			}
		}

		private void Start () {
			if (_setRelativeParallaxSpeed) {
				_furthestLayer.SetSpeedByFurthest(_layers);
				_closestLayer.SetSpeedByClosest(_layers);
			}

			if (_syncToTrackingTarget) {
				var target = ParallaxLayerController.Current.TrackingTarget;
				Debug.Assert(target != null, "ParallaxLayerController.TrackingTarget is null (not allowed)");

				SimulateLayersFromOriginToDestination(transform.position, target.transform.position);
			}
		}

		ParallaxLayer[] GetChildLayers () {
			return GetComponentsInChildren<ParallaxLayer>(true);
		}

		/// <summary>
		/// Restore the original starting location for all parallax layers
		/// </summary>
		public void RestoreOriginPositions () {
			foreach (var parallaxLayer in _layers) {
				if (parallaxLayer == null) continue;
				parallaxLayer.RestoreOriginPosition();
			}
		}

		/// <summary>
		/// Simulate moving layers via passing in a change
		/// </summary>
		/// <param name="change"></param>
		public void MoveLayers (Vector2 change) {
			foreach (var parallaxLayer in _layers) {
				if (parallaxLayer == null) continue;
				parallaxLayer.ParallaxUpdate(change);
			}
		}

		/// <summary>
		/// Relatively moves this parallax group from the origin to the destination. Runs a move simulation on
		/// all nested layers.
		/// </summary>
		public void SimulateLayersFromOriginToDestination (Vector2 origin, Vector2 destination) {
			var distance = destination - origin;
			MoveLayers(distance);
		}
	}
}

