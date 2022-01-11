
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OpenTree {
    public class OpenTreeSettings : GameParameters.CustomParameterNode {
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.CAREER | GameParameters.GameMode.SCIENCE; } }
        public override string Section { get { return "OpenTree"; } }
        public override string DisplaySection { get { return "OpenTree"; } }
        public override bool HasPresets { get { return false; } }
        public override int SectionOrder { get { return 0; } }
        public override string Title { get { return "#autoLOC_190662"; } }
        [GameParameters.CustomParameterUI("#autoLOC_900350", newGameOnly = true)]
        public string start = "Uncrewed";
        public override IList ValidValues(MemberInfo member) => new string[2] { "Uncrewed", "Crewed" };
    }
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class OpenTreeAddon : MonoBehaviour {
        private readonly bool careerOrScienceMode = HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX;
        private readonly Dictionary<string, string> stockUnlocks = new Dictionary<string, string>() {
            ["probeControlI"] = "flightControl",
            ["probeControlII"] = "advFlightControl",
            ["probeControlIII"] = "unmannedTech",
            ["probeControlIV"] = "advUnmanned",
            ["roverTechII"] = "fieldScience",
            ["structuralII"] = "generalConstruction"
        };
        public void Start() {
            if (careerOrScienceMode) {
                GameEvents.OnTechnologyResearched.Add(TechResearched);
                UnlockTech(HighLogic.CurrentGame.Parameters.CustomParams<OpenTreeSettings>().start.ToLower() + "Tech", 5);
            }
        }
        public void OnDisable() {
            if (careerOrScienceMode) GameEvents.OnTechnologyResearched.Remove(TechResearched);
        }
        private void UnlockTech(string techID, int scienceCost) {
            if (ResearchAndDevelopment.Instance.GetTechState(techID) == null)
                ResearchAndDevelopment.Instance.UnlockProtoTechNode(new ProtoTechNode { techID = techID, scienceCost = scienceCost });
        }
        private void TechResearched(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> action) {
            if (stockUnlocks.ContainsKey(action.host.techID) && action.target == RDTech.OperationResult.Successful) UnlockTech(stockUnlocks[action.host.techID], 1);
        }
    }
}