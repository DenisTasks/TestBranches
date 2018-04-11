using BLL.EntitesDTO;
using ViewModel.Models;

namespace ViewModel.Helpers
{
    public class OpenWindowMessage
    {
        public WindowType Type { get; set; }
        public string Argument { get; set; }
        public int SecondsToShow { get; set; }
        public AppointmentModel Appointment { get; set; }
        public UserDTO User { get; set; }
    }
}