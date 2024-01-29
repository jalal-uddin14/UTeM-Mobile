namespace UTeM_Mobile.Services
{
    public class ModalService
    {
        public static async Task PopAllModals()
        {
            try
            {
                while (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex) 
            {

            }
        }
    }
}
