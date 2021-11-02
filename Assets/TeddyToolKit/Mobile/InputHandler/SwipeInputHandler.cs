using System.Collections;
using System.Collections.Generic;
using TeddyToolKit.Core;
using UnityEngine;

namespace TeddyToolKit.Mobile.InputHandler
{
	public class SwipeInputHandler : RunnableBehaviour
	{
		/// <summary>
		/// Contains all the information about this specific swipe, such as points along the swipe
		/// </summary>
		public class Swipe
		{
			/// <summary>
			/// The list of points along the swipe, added to each frame.
			/// </summary>
			public readonly List<Vector2> positions = new List<Vector2>();
			/// <summary>
			/// The position the swipe started from.
			/// </summary>
			public readonly Vector2 initialPosition;
			/// <summary>
			/// The finger id associated with this swipe.
			/// </summary>
			public readonly int fingerId;

			public Swipe(Vector2 initialPosition, int fingerId)
			{
				this.initialPosition = initialPosition;
				this.fingerId = fingerId;
				positions.Add(initialPosition);
			}
		}

		/// <summary>
		/// The count of how many swipes are in progress.
		/// </summary>
		public int SwipeCount => swipes.Count;

		public TrailRenderer trail;
		private Coroutine trailCoroutine;

		// Contains all the swipes currently being processed, each key is the corresponding fingerId
		private Dictionary<int, Swipe> swipes = new Dictionary<int, Swipe>();

		/// <summary>
		/// Attempts to retrieve the relevant swipe information relating to the passed ID.
		/// </summary>
		/// <param name="index">The fingerID we are attempting to get the swipe for.</param>
		/// <returns>The corresponding swipe if it exists, otherwise null.</returns>
		public Swipe GetSwipe(int index)
		{
			swipes.TryGetValue(index, out Swipe swipe);
			return swipe;
		}

		protected override void OnSetup(params object[] parameters) { }

		protected override void OnRun(params object[] parameters)
		{
			if(Input.touchCount > 0)
			{
				// Loop through all touches being processed by Unity
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == TouchPhase.Began)
					{
						// This is the first frame this touch is detected, so put it in the dictionary
						// as a swipe
						swipes.Add(touch.fingerId, new Swipe(touch.position, touch.fingerId));
						//activate trail for visualisation
						trail.gameObject.SetActive(true);
						trail.startWidth = 200;
						trail.endWidth = 100;
						trail.gameObject.transform.position = touch.position;
					}
					else if(touch.phase == TouchPhase.Moved && swipes.TryGetValue(touch.fingerId, out Swipe swipe))
					{
						// This touch moved so add the position to the swipe
						swipe.positions.Add(touch.position);
						//move the trail
						//trailCoroutine = StartCoroutine(DrawTrail(touch.position));

						trail.gameObject.transform.position = touch.position;
						
					}
					else if((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && swipes.TryGetValue(touch.fingerId, out swipe))
					{
						// The swipe has ended so remove it from the dictionary
						swipes.Remove(swipe.fingerId);
						//disable the trail
						//trail.gameObject.SetActive(false);
						//StopCoroutine(trailCoroutine);
					}
				}
			}
		}

		private IEnumerator DrawTrail(Vector2 trailpos)
		{
			while (true)
			{
				trail.gameObject.transform.position = trailpos;
				yield return null;
			}
		}
	}
}