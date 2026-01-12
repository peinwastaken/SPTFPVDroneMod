#if !UNITY_EDITOR
using System;

namespace FPVDroneModClient.Helpers
{
    public static class ActionsHelper
    {
        public static void CreateAction(this ActionsReturnClass actionsReturn, string name, Action action)
        {
            ActionsTypesClass newAction = new ActionsTypesClass
            {
                Name = name,
                Disabled = false,
                Action = action
            };

            actionsReturn.Actions.Add(newAction);
        }
    }
}
#endif