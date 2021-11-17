namespace HelpMyStreet.Cache
{
    public enum RefreshBehaviour : byte
    {
        DontWaitForFreshData = 1,
        WaitForFreshData = 2,
        DontRefreshData = 3
    }
}
