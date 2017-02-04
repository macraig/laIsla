using System;
using UnityEngine;
using Assets.Scripts.Games;

namespace Assets.Scripts.Common.Dragger {
	abstract public class DraggerView : LevelView {
		abstract public void Dropped(DraggerHandler dropped, DraggerSlot where);

		abstract public bool CanDropInSlot(DraggerHandler dropper, DraggerSlot slot);
	}
}