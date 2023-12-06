namespace UTeM_Mobile.Services
{
    public class ModalService
    {
        public static async Task PopAllModals()
        {
            try
            {
                do
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
                while (Application.Current.MainPage.Navigation.ModalStack.Count > 0);
            }
            catch (Exception ex) { }
        }
    }
}
