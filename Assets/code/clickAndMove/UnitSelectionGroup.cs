using System.Collections.Generic;
using UnityEngine;


    [RequireComponent(typeof(ClickControl))]
    public class UnitSelectionGroup : MonoBehaviour {

        private ClickControl UnitMouseOver;

        private static List<BaseUnitAI> allUnitsOnMap;
        private static List<BaseUnitAI> selectedUnits;
    
        // Use this for initialization
        void Start () {
            allUnitsOnMap = new List<BaseUnitAI>();
            selectedUnits = new List<BaseUnitAI>();

            UnitMouseOver = this.GetComponent<ClickControl>();
        }
	
        // Update is called once per frame
        void Update () {
            if(UnitMouseOver.validPoint)
            {
                HandleUnitSelection();
                HandleOrderDelegation();
            }
        }

        public void RegisterUnit(BaseUnitAI me)
        {
            allUnitsOnMap.Add(me);
        }

        public void DeregisterUnit(BaseUnitAI me)
        {
            allUnitsOnMap.Remove(me);
        }

        private void HandleUnitSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var thing = UnitMouseOver.thingClicked;
                if (thing.GetComponent<BaseUnitAI>() == null)
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.WasDeSelected();
                    }
                    selectedUnits = new List<BaseUnitAI>();
                }
            }

            if (Input.GetMouseButton(0))
            {
                var mousedSoldier = UnitMouseOver.thingClicked.GetComponent<BaseUnitAI>();
                if (mousedSoldier != null)
                {
                    if (!selectedUnits.Contains(mousedSoldier))
                    {
                        selectedUnits.Add(mousedSoldier);
                        mousedSoldier.WasSelected();
                        Debug.Log("Unit selected");
                    }
                }
            }
        }

        private void HandleOrderDelegation()
        {
            if (Input.GetMouseButtonUp(1))
            {
                //interestingly, we want to *not* click a soldier this way
                var mousedSoldier = UnitMouseOver.thingClicked.GetComponent<BaseUnitAI>();
                if (mousedSoldier == null)
                {
                    foreach(var unit in selectedUnits)
                    {
                        unit.RecieveOrderTo(UnitMouseOver.clickedPoint);
                        Debug.Log("Set orders to " + selectedUnits.Count.ToString());
                    }
                }
            }
        }

    }

