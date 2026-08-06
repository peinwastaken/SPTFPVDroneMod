using EFT.UI;
using System;

namespace FPVDroneModClient.Helpers
{
    public static class ActionsHelper
    {
        public static void CreateAction(this AvailableInteractionState actionsReturn, string name, Action action)
        {
            InteractionAction newAction = new InteractionAction
            {
                Name = name,
                Disabled = false,
                Action = action
            };

            actionsReturn.Actions.Add(newAction);
        }
    }
}

