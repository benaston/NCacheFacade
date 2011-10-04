namespace NCacheFacade
{
    using System;

    /// <summary>
    /// NOTE: BA; check why this needs to be serializable.
    /// </summary>
    [Serializable]
    public class FakeFriendlyNameCreationStrategy : IFriendlyNameCreationStrategy
    {
        public string GenerateFriendlyName(params object[] args)
        {
            return String.Empty;
        }
    }
}