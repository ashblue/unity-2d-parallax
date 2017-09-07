﻿using UnityEngine;
using System.Collections;

namespace Adnc {
	[System.Serializable]
	public abstract class MinMaxBase<T> {
		public T min;
		public T max;

		public abstract T GetRandom ();
	}
}
