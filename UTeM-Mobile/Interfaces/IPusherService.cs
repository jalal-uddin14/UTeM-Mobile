namespace UTeM_Mobile.Interfaces
{
    public interface IPusherService
    {
        Task SubscribeGuardChannelAsync();
        void GuardActivityListener(object sender);
    }
}
