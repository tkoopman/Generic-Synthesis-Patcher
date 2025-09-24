namespace GenericSynthesisPatcher.Exceptions
{
    public class GSPActionException : Exception
    {
        public GSPActionException (ProcessingKeys proKeys, string actionName)
            : base($"Error performing rule '{proKeys.Rule?.GetLogRuleID()}' action '{actionName}' on record '{proKeys.Record?.FormKey}'.\n - Versions: [{Global.Version}|{Global.MutagenVersion}|{Global.SynthesisVersion}]")
        {
            ProcessingKeys = proKeys;
            ActionName = actionName;
        }

        public GSPActionException (ProcessingKeys proKeys, string actionName, string message)
            : base($"Error performing rule '{proKeys.Rule?.GetLogRuleID()}' action '{actionName}' on record '{proKeys.Record?.FormKey}'.\n - Versions: [{Global.Version}|{Global.MutagenVersion}|{Global.SynthesisVersion}]\n - {message}")
        {
            ProcessingKeys = proKeys;
            ActionName = actionName;
        }

        public GSPActionException (ProcessingKeys proKeys, string actionName, Exception inner)
            : base($"Error performing rule '{proKeys.Rule?.GetLogRuleID()}' action '{actionName}' on record '{proKeys.Record?.FormKey}'.\n - Versions: [{Global.Version}|{Global.MutagenVersion}|{Global.SynthesisVersion}]", inner)
        {
            ProcessingKeys = proKeys;
            ActionName = actionName;
        }

        public GSPActionException (ProcessingKeys proKeys, string actionName, string message, Exception inner)
            : base($"Error performing rule '{proKeys.Rule?.GetLogRuleID()}' action '{actionName}' on record '{proKeys.Record?.FormKey}'.\n - Versions: [{Global.Version}|{Global.MutagenVersion}|{Global.SynthesisVersion}]\n - {message}", inner)
        {
            ProcessingKeys = proKeys;
            ActionName = actionName;
        }

        public string ActionName { get; }
        public ProcessingKeys ProcessingKeys { get; }
    }
}