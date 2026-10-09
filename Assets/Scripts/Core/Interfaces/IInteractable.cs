namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Anything a third-person player can interact with: NPCs, truck doors,
    /// fuel pumps, job boards, and so on.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Stable identifier used for analytics, save data and prompts.</summary>
        string InteractableId { get; }

        /// <summary>Short user-facing prompt, e.g. "Open door" or "Check jobs".</summary>
        string InteractionPrompt { get; }

        /// <summary>True when the interactable can currently be used by the given actor.</summary>
        bool CanInteract(IInteractor interactor);

        /// <summary>Performs the interaction on behalf of the given actor.</summary>
        void Interact(IInteractor interactor);
    }

    /// <summary>The entity (usually the player) that initiates an interaction.</summary>
    public interface IInteractor
    {
        /// <summary>Identifier of the acting entity.</summary>
        string InteractorId { get; }
    }
}
