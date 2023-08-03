namespace UTeM_Mobile.Models
{
    public class PopMessage
    {
        public string Type { get; set; }
        public string Heading { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string NavigateTo { get; set; }
        public bool HasNavigate { get; set; }

        public static PopMessage GetMessage(string heading, string title = "", string message = "")
        {
            return new PopMessage
            {
                Heading = heading,
                Title = title,
                Message = message,
                NavigateTo = "",
                HasNavigate = false
            };
        }
        public static PopMessage GetNavigationMessage(string type, string heading, string navigateTo, string title = "",  string message = "")
        {
            return new PopMessage
            {
                Type = type,
                Heading = heading,
                Title = title,
                Message = message,
                NavigateTo = navigateTo,
                HasNavigate = true
            };
        }
        public static PopMessage GetExceptionMessage(string heading = "Error", string message = "")
        {
            return new PopMessage
            {
                Heading = heading,
                Title = "Internal error occured.",
                Message = message,
                NavigateTo = "",
                HasNavigate = false
            };
        }
    }
}
