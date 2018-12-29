namespace ValveKeyValue.Abstraction
{
    internal interface IParsingVisitationListener : IVisitationListener
    {
        void DiscardCurrentObject();
        IParsingVisitationListener GetMergeListener();
        IParsingVisitationListener GetAppendListener();
    }
}
